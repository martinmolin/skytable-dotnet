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
    public class Connection
    {
        public string Host { get; }
        private const ushort BUF_CAP = 4096;
        private TcpClient _client;
        private Stream _stream;
        private List<byte> _buffer;
        
        public Connection(string host, ushort port)
            : this(host, port, false)
        {

        }

        public Connection(string host, ushort port, bool useTls)
        {
            Host = host;
            _client = new TcpClient(host, port);
            _buffer = new List<Byte>(BUF_CAP);

            if (useTls)
                AuthenticateSsl();
            else
                _stream = _client.GetStream();
        }

        private void AuthenticateSsl()
        {
            var sslStream = new SslStream(
                _client.GetStream(),
                false,
                new RemoteCertificateValidationCallback (ValidateServerCertificate),
                null
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

        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
           if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            
            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

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

        public Response Get(string key)
        {
            var query = new Query();
            query.Push("get");
            query.Push(key);
            return RunSimpleQuery(query);
        }

        public async Task<Response> GetAsync(string key)
        {
            var query = new Query();
            query.Push("get");
            query.Push(key);
            return await RunSimpleQueryAsync(query);
        }

        public T Get<T>(string key) where T: Skyhash, new()
        {
            var query = new Query();
            query.Push("get");
            query.Push(key);
            var response = RunSimpleQuery(query);
            return new T().From<T>(response);
        }

        public async Task<T> GetAsync<T>(string key) where T: Skyhash, new()
        {
            var query = new Query();
            query.Push("get");
            query.Push(key);
            var response = await RunSimpleQueryAsync(query);
            return new T().From<T>(response);
        }

        public Response Set(string key, string value)
        {
            var query = new Query();
            query.Push("set");
            query.Push(key);
            query.Push(value);
            return RunSimpleQuery(query);
        }

        public async Task<Response> SetAsync(string key, string value)
        {
            var query = new Query();
            query.Push("set");
            query.Push(key);
            query.Push(value);
            return await RunSimpleQueryAsync(query);
        }

        public Response Set(string key, Skyhash value)
        {
            var query = new Query();
            query.Push("set");
            query.Push(key);
            query.Push(value.Into());
            return RunSimpleQuery(query);
        }

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
