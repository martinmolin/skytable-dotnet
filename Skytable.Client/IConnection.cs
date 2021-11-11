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

using System.Threading.Tasks;
using Skytable.Client.Querying;

namespace Skytable.Client
{
    /// <summary>A database connection interface for Skyhash.</summary>
    public interface IConnection
    {
        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        SkyResult<Element> Get(string key);

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        Task<SkyResult<Element>> GetAsync(string key);

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        SkyResult<T> Get<T>(string key) where T: Skyhash, new();

        /// <summary>
        /// This function will create a GET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        Task<SkyResult<T>> GetAsync<T>(string key) where T: Skyhash, new();

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        SkyResult<Element> Set(string key, string value);

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        Task<SkyResult<Element>> SetAsync(string key, string value);

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        SkyResult<Element> Set(string key, Skyhash value);

        /// <summary>
        /// This function will create a SET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        Task<SkyResult<Element>> SetAsync(string key, Skyhash value);

        /// <summary>
        /// This function will create a DEL <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        SkyResult<Element> Delete(string key);

        /// <summary>
        /// This function will create a DEL <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        Task<SkyResult<Element>> DeleteAsync(string key);

        /// <summary>
        /// This function will create an USET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        SkyResult<Element> USet(string key, string value);

        /// <summary>
        /// This function will create an USET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        Task<SkyResult<Element>> USetAsync(string key, string value);

        /// <summary>
        /// This function will create an USET <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        SkyResult<Element> USet(string key, Skyhash value);

        /// <summary>
        /// This function will create an USET <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        Task<SkyResult<Element>> USetAsync(string key, Skyhash value);

        /// <summary>
        /// This function will create a POP <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        SkyResult<Element> Pop(string key);

        /// <summary>
        /// This function will create a POP <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and return an appropriate variant of <see cref="Element"/>.
        /// </summary>
        Task<SkyResult<Element>> PopAsync(string key);

        /// <summary>
        /// This function will create a POP <see cref="Query"/> and write it to the stream and read the response from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        SkyResult<T> Pop<T>(string key) where T: Skyhash, new();

        /// <summary>
        /// This function will create a POP <see cref="Query"/> and write it to the stream and read the response asynchronously from the
        /// server. It will then determine if the returned response is complete, incomplete
        /// or invalid and try to return type T if successful.
        /// </summary>
        Task<SkyResult<T>> PopAsync<T>(string key) where T: Skyhash, new();
    }
}
