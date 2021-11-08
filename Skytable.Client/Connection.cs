//  Copyright (c) 2021 Martin Molin

//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Net.Sockets;
using System.Collections.Generic;
using Skytable.Client.Querying;
using Skytable.Client.Parsing;
using System.Threading.Tasks;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Skytable.Client
{
    /// <summary>A database connection over Skyhash/TCP.</summary>
    public class Connection : IConnection, IDisposable
    {
        /// <summary>Gets the host that this connection is connected to.</summary>
        public string Host { get; }

        /// <summary>Gets the port that this connection is connected to.</summary>
        public ushort Port { get; }

        /// <summary>Gets the entity that this connection is connected to.</summary>
        public string Entity { get; private set; } = "default:default";

        /// <summary>Gets the connection state.</summary>
        public bool IsConnected => _client.Connected;

        private string _certPath;        
        private const ushort BUF_CAP = 4096;
        private const int WSAENOTCONN = 10057;
        private TcpClient _client;
        private Stream _stream;
        private List<byte> _buffer;
        
        /// <summary>
        /// Create a new connection to a Skytable instance hosted on the provided host and port with Tls disabled.
        /// Call <see cref="Connection.Connect"/> or <see cref="Connection.ConnectAsync"/> to connect after creating the connection.
        ///</summary>
        /// <Param name="host">The host which is running Skytable.</Param>
        /// <Param name="port">The port which the host is running Skytable.</Param>
        public Connection(string host, ushort port)
            : this (host, port, string.Empty) { }

        /// <summary>
        /// Create a new connection to a Skytable instance hosted on the provided host and port with Tls enabled.
        /// Call <see cref="Connection.Connect"/> or <see cref="Connection.ConnectAsync"/> to connect after creating the connection.
        ///</summary>
        /// <Param name="host">The host which is running Skytable.</Param>
        /// <Param name="port">The port which the host is running Skytable. This has to be configured as a secure port in Skytable.</Param>
        /// <Param name="certPath">Path to the certificate file.</Param>
        public Connection(string host, ushort port, string certPath)
        {
            Host = host;
            Port = port;
            _client = new TcpClient();
            _buffer = new List<Byte>(BUF_CAP);
            _certPath = certPath;
        }

        /// <summary>
        /// Open a connection to the Host:Port specified in the constructor.
        /// If a Certificate path is specified an attempt will be made to set up a secure connection.
        /// </summary>
        public void Connect()
        {
            _client.Connect(Host, Port);
            _stream = _client.GetStream();

            if (!string.IsNullOrEmpty(_certPath))
                AuthenticateSsl();
        }

        /// <summary>
        /// Open a connection asynchronously to the Host:Port specified in the constructor.
        /// If a Certificate path is specified an attempt will be made to set up a secure connection.
        /// </summary>
        public async Task ConnectAsync()
        {
            await _client.ConnectAsync(Host, Port);
            _stream = _client.GetStream();

            if (!string.IsNullOrEmpty(_certPath))
                await AuthenticateSslAsync();
        }

        /// <summary>Close the connection.</summary>
        public void Close()
        {
            _client.Close();
        }

        /// <summary>Dispose of the connection. This will close the connection.</summary>
        public void Dispose()
        {
            Close();
        }

        private void AuthenticateSsl()
        {
            var sslStream = new SslStream(
                _client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                new LocalCertificateSelectionCallback(SelectCertificate)
                );

            try
            {
                sslStream.AuthenticateAsClient(Host);
                _stream = sslStream;
            }
            catch (Exception)
            {
                _client.Close();
                throw;
            }
        }

        private async Task AuthenticateSslAsync()
        {
            var sslStream = new SslStream(
                _client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                new LocalCertificateSelectionCallback(SelectCertificate)
                );

            try
            {
                await sslStream.AuthenticateAsClientAsync(Host);
                _stream = sslStream;
            }
            catch (Exception)
            {
                _client.Close();
                throw;
            }
        }

        private X509Certificate SelectCertificate(
            object sender,
            string targetHost,
            X509CertificateCollection localCertificates,
            X509Certificate remoteCertificate,
            string[] acceptableIssuers)
        {
            var cert = new X509Certificate(_certPath);
            return cert;
        }

        private static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            
            // Allow self signed certificates for now.
            if (chain.ChainStatus.Length == 1)
            {
                if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || certificate.Subject == certificate.Issuer)
                {
                    if (chain.ChainStatus[0].Status == X509ChainStatusFlags.UntrustedRoot)
                        return true;
                }
            }

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        /// <summary>
        /// This function will write a <see cref="Query"/> to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public SkyResult<Response> RunSimpleQuery(Query query)
        {
            if (!IsConnected)
                throw new NotConnectedException();

            query.WriteTo(_stream);

            while (true)
            {
                var buffer = new byte[1024];
                var read = _stream.Read(buffer, 0, 1024);
                if (read == 0)
                    throw new Exception("ConnectionReset");

                _buffer.AddRange(buffer[..read]);

                var result = ParseResponse();
                if (result.IsOk)
                    return result;

                switch (result.Error)
                {
                    case ParseError.NotEnough:
                        continue; // We need to read again to get the complete response.
                    case ParseError.UnexpectedByte:
                    case ParseError.BadPacket:
                        _buffer.Clear();
                        return result;
                    case ParseError.DataTypeParseError:
                    case ParseError.UnknownDataType:
                    case ParseError.Empty:
                        return result;
                }
            }
        }

        /// <summary>
        /// This function will write a <see cref="Query"/> asynchronously to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public async Task<SkyResult<Response>> RunSimpleQueryAsync(Query query)
        {
            if (!IsConnected)
                throw new NotConnectedException();

            await query.WriteToAsync(_stream);

            while (true)
            {
                var buffer = new byte[1024];
                var read = await _stream.ReadAsync(buffer, 0, 1024);
                if (read == 0)
                    throw new Exception("ConnectionReset");

                _buffer.AddRange(buffer[..read]);

                var result = ParseResponse();
                if (result.IsOk)
                    return result;

                switch (result.Error)
                {
                    case ParseError.NotEnough:
                        continue; // We need to read again to get the complete response.
                    case ParseError.UnexpectedByte:
                    case ParseError.BadPacket:
                        _buffer.Clear();
                        return result;
                    case ParseError.DataTypeParseError:
                    case ParseError.UnknownDataType:
                    case ParseError.Empty:
                        return result;
                }
            }
        }

        private SkyResult<Response> ParseResponse()
        {
            // The connection was possibly reset
            if (_buffer.Count == 0)
                return SkyResult<Response>.Err(ParseError.Empty);

            var parser = new Parser(_buffer);
            var (result, forward_by) = parser.Parse();
            _buffer.RemoveRange(0, forward_by);
            return result;
        }

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public SkyResult<Response> Get(string key)
        {
            var query = new Query();
            query.Push("get");
            query.Push(key);
            return RunSimpleQuery(query);
        }

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public async Task<SkyResult<Response>> GetAsync(string key)
        {
            var query = new Query();
            query.Push("get");
            query.Push(key);
            return await RunSimpleQueryAsync(query);
        }

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        public SkyResult<T> Get<T>(string key) where T: Skyhash, new()
        {
            var query = new Query();
            query.Push("get");
            query.Push(key);
            var response = RunSimpleQuery(query);
            if (response.IsOk)
                return SkyResult<T>.Ok(new T().From<T>(response.Item));
            return SkyResult<T>.Err(response.Error);
        }

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        public async Task<SkyResult<T>> GetAsync<T>(string key) where T: Skyhash, new()
        {
            var query = new Query();
            query.Push("get");
            query.Push(key);
            var response = await RunSimpleQueryAsync(query);
            if (response.IsOk)
                return SkyResult<T>.Ok(new T().From<T>(response.Item));
            return SkyResult<T>.Err(response.Error);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public SkyResult<Response> Set(string key, string value)
        {
            var query = new Query();
            query.Push("set");
            query.Push(key);
            query.Push(value);
            return RunSimpleQuery(query);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public async Task<SkyResult<Response>> SetAsync(string key, string value)
        {
            var query = new Query();
            query.Push("set");
            query.Push(key);
            query.Push(value);
            return await RunSimpleQueryAsync(query);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public SkyResult<Response> Set(string key, Skyhash value)
        {
            var query = new Query();
            query.Push("set");
            query.Push(key);
            query.Push(value.Into());
            return RunSimpleQuery(query);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public async Task<SkyResult<Response>> SetAsync(string key, Skyhash value)
        {
            var query = new Query();
            query.Push("set");
            query.Push(key);
            query.Push(value.Into());
            return await RunSimpleQueryAsync(query);
        }

        /// <summary>
        /// This function will create an USET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public SkyResult<Response> USet(string key, string value)
        {
            var query = new Query();
            query.Push("uset");
            query.Push(key);
            query.Push(value);
            return RunSimpleQuery(query);
        }

        /// <summary>
        /// This function will create an USET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public async Task<SkyResult<Response>> USetAsync(string key, string value)
        {
            var query = new Query();
            query.Push("uset");
            query.Push(key);
            query.Push(value);
            return await RunSimpleQueryAsync(query);
        }

        /// <summary>
        /// This function will create an USET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public SkyResult<Response> USet(string key, Skyhash value)
        {
            var query = new Query();
            query.Push("uset");
            query.Push(key);
            query.Push(value.Into());
            return RunSimpleQuery(query);
        }

        /// <summary>
        /// This function will create an USET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public async Task<SkyResult<Response>> USetAsync(string key, Skyhash value)
        {
            var query = new Query();
            query.Push("uset");
            query.Push(key);
            query.Push(value.Into());
            return await RunSimpleQueryAsync(query);
        }

        /// <summary>
        /// This function will create a USE <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public SkyResult<Response> Use(string keyspace, string table)
        {
            var entity = string.Join(':', keyspace, table);
            var query = new Query();
            query.Push("use");
            query.Push(entity);
            var result = RunSimpleQuery(query);
            if (result.IsOk) // TODO: Check if the Response Element is Okay.
                Entity = entity;

            return result;
        }

        /// <summary>
        /// This function will create a USE <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public async Task<SkyResult<Response>> UseAsync(string keyspace, string table)
        {
            var entity = string.Join(':', keyspace, table);
            var query = new Query();
            query.Push("use");
            query.Push(entity);
            var result = await RunSimpleQueryAsync(query);
            if (result.IsOk) // TODO: Check if the Response Element is Okay.
                Entity = entity;

            return result;
        }
    }
}
