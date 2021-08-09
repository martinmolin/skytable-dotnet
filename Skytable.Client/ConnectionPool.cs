using System;
using System.Collections.Concurrent;

namespace Skytable.Client
{
    public class ConnectionPool
    {
        public string Host { get; }
        public ushort Port { get; }
        public ushort Count { get; private set; }
        public bool AllowTemporaryConnections { get; set; }
        
        private ConcurrentQueue<Connection> _connections;
        private volatile ushort _borrowedCount = 0;
        private bool _initialized = false;

        public ConnectionPool(string host, ushort port, bool allowTemporaryConnection)
        {
            Host = host;
            Port = port;
            AllowTemporaryConnections = allowTemporaryConnection;
            _connections = new ConcurrentQueue<Connection>();
        }

        public void Initialize(ushort count)
        {
            Count = count;
            for (int i = 0; i < Count; i++)
                _connections.Enqueue(new Connection(Host, Port));

            _initialized = true;
        }

        public PooledConnection Connection()
        {
            if (!_initialized)
                throw new Exception("Pool is not initialized. Call Pool.Initialize() before using it.");

            if (_connections.TryDequeue(out var connection))
            {
                _borrowedCount++;
                return new PooledConnection(this, connection, ConnectionType.Pooled);
            }

            if (AllowTemporaryConnections)
                return new PooledConnection(this, new Connection(Host, Port), ConnectionType.Temporary);

            throw new Exception("No connections available.");
        }

        internal void Return(Connection connection)
        {
            _connections.Enqueue(connection);
            _borrowedCount--;
        }
    }
}
