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
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Skytable.Client
{
    /// <summary>A database connection pool for Skyhash/TCP.</summary>
    public class ConnectionPool
    {        
        /// <summary>Gets the host that this connection pool is connected to.</summary>
        public string Host { get; }

        /// <summary>Gets the port that this connection pool is connected to.</summary>
        public ushort Port { get; }

        /// <summary>Gets the count of the connections handled by this pool.</summary>
        public ushort Count { get; private set; }
        
        /// <summary>Gets the entity that this connection pool is targeting.</summary>
        public string Entity { get; }

        /// <summary>Gets or sets whether the pool should be able to create temporary connections in the case where it runs out of pooled connections.</summary>
        public bool AllowTemporaryConnections { get; set; }
        
        private ConcurrentQueue<Connection> _connections;
        private volatile ushort _borrowedCount = 0;
        private bool _initialized = false;
        private string _keyspace;
        private string _table;

        /// <summary>Create a new connection pool to a Skytable instance hosted on the provided host and port with Tls disabled. Call Initialize to create the connections to the Skytable instance.</summary>
        /// <Param name="host">The host which is running Skytable.</Param>
        /// <Param name="port">The port which the host is running Skytable.</Param>
        /// <Param name="keyspace">The keyspace that the connections of this pool should target.</Param>
        /// <Param name="table">The table that the connections of this pool should target.</Param>
        /// <Param name="allowTemporaryConnection">Allow the pool to create temporary connections in the case where it runs out of pooled connections.</Param>
        public ConnectionPool(string host, ushort port, string keyspace, string table, bool allowTemporaryConnection)
        {
            Host = host;
            Port = port;
            _keyspace = keyspace;
            _table = table;
            Entity = string.Join(':', keyspace, table);
            AllowTemporaryConnections = allowTemporaryConnection;
            _connections = new ConcurrentQueue<Connection>();
        }

        /// <summary>Initialize the pool with the specified amount of connections.</summary>
        /// <Param name="count">The count of connections that the pool should have.</Param>
        public void Initialize(ushort count)
        {
            if (_initialized)
                throw new Exception("This pool has already been initialized"); // TODO: Exception type.

            // TODO: Inspect the Skytable database to make sure the target entity exists.
            
            Count = count;
            for (int i = 0; i < Count; i++) {
                _connections.Enqueue(CreateConnection());
            }

            _initialized = true;
        }

        /// <summary>Initialize the pool asynchronously with the specified amount of connections.</summary>
        /// <Param name="count">The count of connections that the pool should have.</Param>
        public async Task InitializeAsync(ushort count)
        {
            if (_initialized)
                throw new Exception("This pool has already been initialized"); // TODO: Exception type.
            
            // TODO: Inspect the Skytable database to make sure the target entity exists.
            
            Count = count;
            for (int i = 0; i < Count; i++) {
                var connection = new Connection(Host, Port);
                await connection.ConnectAsync();
                await connection.UseAsync(_keyspace, _table);
                _connections.Enqueue(connection);
            }

            _initialized = true;
        }

        /// <summary>
        /// Returns a connection from the pool if there is one available.
        /// If no connections are available and <see cref="ConnectionPool.AllowTemporaryConnections" /> is set to true, a new temporary connection will be created.
        /// Otherwise an <see cref="Exception" /> will be thrown.
        /// </summary>
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

            // TODO: Exception type
            throw new Exception("No connections available.");
        }

        internal void Return(Connection connection)
        {
            if (connection.IsConnected)
                _connections.Enqueue(connection);
            else
                _connections.Enqueue(CreateConnection());

            _borrowedCount--;
        }

        private Connection CreateConnection()
        {
            var connection = new Connection(Host, Port);
            connection.Connect();
            connection.Use(_keyspace, _table);
            return connection;
        }
    }
}
