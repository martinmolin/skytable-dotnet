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

using System.IO;
using System.Threading.Tasks;

namespace Skytable.Client.Querying
{
    /// <summary>Interface for an object that can be written to a stream.</summary>
    public interface IQueryWriter
    {
        /// <summary>Writes the query to the specified stream.</summary>
        void WriteTo(Stream stream);

        /// <summary>Writes the query to the specified stream asynchronously.</summary>
        Task WriteToAsync(Stream stream);
    }
}
