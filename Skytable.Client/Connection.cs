using System;
using System.Net.Sockets;
using System.Collections.Generic;
using Skytable.Client.Querying;
using Skytable.Client.Parsing;

namespace Skytable.Client
{
    public class Connection
    {
        private const ushort BUF_CAP = 4096;
        private TcpClient _client;
        private List<byte> _buffer;

        public Connection(string host, ushort port)
        {
            _client = new TcpClient(host, port);
            _buffer = new List<Byte>(BUF_CAP);
        }

        public Response RunSimpleQuery(Query query)
        {
            query.WriteTo(_client.GetStream());

            while (true)
            {
                var buffer = new byte[1024];
                var read = _client.GetStream().Read(buffer, 0, 1024);
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

        public T Get<T>(string key) where T: Skyhash<T>, new()
        {
            var query = new Query();
            query.Push("get");
            query.Push(key);
            var response = RunSimpleQuery(query);
            return new T().From(response);
        }

        public Response Set(string key, string value)
        {
            var query = new Query();
            query.Push("set");
            query.Push(key);
            query.Push(value);
            return RunSimpleQuery(query);
        }

        public Response Set<T>(string key, Skyhash<T> value) where T: Skyhash<T>
        {
            var query = new Query();
            query.Push("set");
            query.Push(key);
            query.Push(value.Into());
            return RunSimpleQuery(query);
        }
    }
}
