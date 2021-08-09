using System;
using System.Threading.Tasks;
using Skytable.Client.Querying;

namespace Skytable.Client
{
    public class PooledConnection : IConnection, IDisposable
    {
        private ConnectionPool _pool;
        private Connection _connection;
        private ConnectionType _connectionType;

        internal PooledConnection(ConnectionPool pool, Connection connection, ConnectionType connectionType)
        {
            _pool = pool;
            _connection = connection;
            _connectionType = connectionType;
        }

        public void Dispose()
        {
            if (_connectionType == ConnectionType.Pooled)
                _pool.Return(_connection);
            else
                _connection.Close();
        }

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public Response Get(string key)
        {
            return _connection.Get(key);
        }

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public async Task<Response> GetAsync(string key)
        {
            return await _connection.GetAsync(key);
        }

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        public T Get<T>(string key) where T: Skyhash, new()
        {
            return _connection.Get<T>(key);
        }

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        public async Task<T> GetAsync<T>(string key) where T: Skyhash, new()
        {
            return await _connection.GetAsync<T>(key);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public Response Set(string key, string value)
        {
            return _connection.Set(key, value);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public async Task<Response> SetAsync(string key, string value)
        {
            return await _connection.SetAsync(key, value);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public Response Set(string key, Skyhash value)
        {
            return _connection.Set(key, value);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Response"/>.
        /// </summary>
        public async Task<Response> SetAsync(string key, Skyhash value)
        {
            return await _connection.SetAsync(key, value);
        }
    }
}
