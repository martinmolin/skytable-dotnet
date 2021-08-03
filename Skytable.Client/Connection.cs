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
    public class Connection
    {
        /// <summary>Gets the host of this connection.</summary>
        public string Host { get; }

        private string _certPath;        
        private const ushort BUF_CAP = 4096;
        private TcpClient _client;
        private Stream _stream;
        private List<byte> _buffer;
        
        /// <summary>Create a new connection to a Skytable instance hosted on the provided host and port with Tls disabled.</summary>
        /// <Param name="host">The host which is running Skytable.</Param>
        /// <Param name="port">The port which the host is running Skytable.</Param>
        public Connection(string host, ushort port)
        {
            Host = host;
            _client = new TcpClient(host, port);
            _buffer = new List<Byte>(BUF_CAP);
            _stream = _client.GetStream();
        }

        /// <summary>Create a new connection to a Skytable instance hosted on the provided host and port with Tls enabled.</summary>
        /// <Param name="host">The host which is running Skytable.</Param>
        /// <Param name="port">The port which the host is running Skytable. This has to be configured as a secure port in Skytable.</Param>
        /// <Param name="certPath">Path to the certificate file.</Param>
        public Connection(string host, ushort port, string certPath)
        {
            Host = host;
            _client = new TcpClient(host, port);
            _buffer = new List<Byte>(BUF_CAP);
            _certPath = certPath;
            AuthenticateSsl();
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
        public Response RunSimpleQuery(Query query)
        {
            query.WriteTo(_stream);

            while (true)
            {
                var buffer = new byte[1024];
                var read = _stream.Read(buffer, 0, 1024);
                if (read == 0)
                    throw new Exception("ConnectionReset");

                _buffer.AddRange(buffer[..read]);
                return ParseResponse();
            }
        }

        /// <summary>
        /// This function will write a <see cref="Query"/> asynchronously to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public async Task<Response> RunSimpleQueryAsync(Query query)
        {
            await query.WriteToAsync(_stream);

            while (true)
            {
                var buffer = new byte[1024];
                var read = await _stream.ReadAsync(buffer, 0, 1024);
                if (read == 0)
                    throw new Exception("ConnectionReset");

                _buffer.AddRange(buffer[..read]);
                return ParseResponse();
            }
        }

        private Response ParseResponse()
        {
            // The connection was possibly reset
            if (_buffer.Count == 0)
                throw new ParseException(ParseError.Empty);

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
        public Response Get(string key)
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
        public async Task<Response> GetAsync(string key)
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
        public T Get<T>(string key) where T: Skyhash, new()
        {
            var query = new Query();
            query.Push("get");
            query.Push(key);
            var response = RunSimpleQuery(query);
            return new T().From<T>(response);
        }

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        public async Task<T> GetAsync<T>(string key) where T: Skyhash, new()
        {
            var query = new Query();
            query.Push("get");
            query.Push(key);
            var response = await RunSimpleQueryAsync(query);
            return new T().From<T>(response);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public Response Set(string key, string value)
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
        public async Task<Response> SetAsync(string key, string value)
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
        public Response Set(string key, Skyhash value)
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
        public async Task<Response> SetAsync(string key, Skyhash value)
        {
            var query = new Query();
            query.Push("set");
            query.Push(key);
            query.Push(value.Into());
            return await RunSimpleQueryAsync(query);
        }
    }
}
