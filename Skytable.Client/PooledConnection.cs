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
using System.Threading.Tasks;
using Skytable.Client.Querying;

namespace Skytable.Client
{
    /// <summary>A pooled database connection over Skyhash/TCP.</summary>
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

        /// <summary>Dispose of the pooled connection, returning it to the pool or closing it if it was a temporary connection.</summary>
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
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public SkyResult<Element> Get(string key)
        {
            return _connection.Get(key);
        }

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public async Task<SkyResult<Element>> GetAsync(string key)
        {
            return await _connection.GetAsync(key);
        }

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        public SkyResult<T> Get<T>(string key) where T: Skyhash, new()
        {
            return _connection.Get<T>(key);
        }

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        public async Task<SkyResult<T>> GetAsync<T>(string key) where T: Skyhash, new()
        {
            return await _connection.GetAsync<T>(key);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public SkyResult<Element> Set(string key, string value)
        {
            return _connection.Set(key, value);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public async Task<SkyResult<Element>> SetAsync(string key, string value)
        {
            return await _connection.SetAsync(key, value);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public SkyResult<Element> Set(string key, Skyhash value)
        {
            return _connection.Set(key, value);
        }

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public async Task<SkyResult<Element>> SetAsync(string key, Skyhash value)
        {
            return await _connection.SetAsync(key, value);
        }

        /// <summary>
        /// This function will create a DEL <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public SkyResult<Element> Delete(string key)
        {
            return _connection.Delete(key);
        }

        /// <summary>
        /// This function will create a DEL <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public async Task<SkyResult<Element>> DeleteAsync(string key)
        {
            return await _connection.DeleteAsync(key);
        }

        /// <summary>
        /// This function will create an USET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public SkyResult<Element> USet(string key, string value)
        {
            return _connection.USet(key, value);
        }

        /// <summary>
        /// This function will create an USET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public async Task<SkyResult<Element>> USetAsync(string key, string value)
        {
            return await _connection.USetAsync(key, value);
        }

        /// <summary>
        /// This function will create an USET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public SkyResult<Element> USet(string key, Skyhash value)
        {
            return _connection.USet(key, value);
        }

        /// <summary>
        /// This function will create an USET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public async Task<SkyResult<Element>> USetAsync(string key, Skyhash value)
        {
            return await _connection.USetAsync(key, value);
        }

        /// <summary>
        /// This function will create a POP <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public SkyResult<Element> Pop(string key)
        {
            return _connection.Pop(key);
        }

        /// <summary>
        /// This function will create a POP <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        public async Task<SkyResult<Element>> PopAsync(string key)
        {
            return await _connection.PopAsync(key);
        }

        /// <summary>
        /// This function will create a POP <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        public SkyResult<T> Pop<T>(string key) where T: Skyhash, new()
        {
            return _connection.Pop<T>(key);
        }

        /// <summary>
        /// This function will create a POP <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        public async Task<SkyResult<T>> PopAsync<T>(string key) where T: Skyhash, new()
        {
            return await _connection.PopAsync<T>(key);
        }
    }
}
