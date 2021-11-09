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

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Skytable.Client.Querying
{
    /// <summary>This type represents a single simple query as defined by the Skyhash protocol.</summary>
    public class Query : IQueryWriter
    {
        private ushort _sizeCount;
        private List<byte> _holdingBuffer;

        /// <summary>Returns the argument count of the query.</summary>
        public ushort ArgumentCount => _sizeCount;
        
        /// <summary>Creates an empty query with a no arguments.</summary>
        public Query()
        {
            _sizeCount = 0;
            _holdingBuffer = new List<byte>();
        }

        /// <summary>Pushes an argument into the query.</summary>
        public void Push(string argument)
        {
            var unicode_argument = System.Text.Encoding.UTF8.GetBytes(argument);
            var header = $"{unicode_argument.Length}\n";
            var unicode_header = System.Text.Encoding.UTF8.GetBytes(header);
            _holdingBuffer.AddRange(unicode_header);
            _holdingBuffer.AddRange(unicode_argument);
            _holdingBuffer.Add(10);
            _sizeCount++;
        }

        /// <summary>Writes the query to the specified stream.</summary>
        public void WriteTo(Stream stream)
        {
            // TODO: Write everything at once?
            var header = System.Text.Encoding.UTF8.GetBytes("*1\n");
            stream.Write(header, 0, header.Length);
            var numberOfItemsInDatagroup = System.Text.Encoding.UTF8.GetBytes($"~{_sizeCount}\n");
            stream.Write(numberOfItemsInDatagroup, 0, numberOfItemsInDatagroup.Length);
            stream.Write(_holdingBuffer.ToArray(), 0, _holdingBuffer.Count);
        }

        /// <summary>Writes the query to the specified list.</summary>
        public void WriteTo(List<byte> list)
        {
            var numberOfItemsInDatagroup = System.Text.Encoding.UTF8.GetBytes($"~{_sizeCount}\n");
            list.AddRange(numberOfItemsInDatagroup);
            list.AddRange(_holdingBuffer);
        }

        /// <summary>Writes the query to the specified stream asynchronously.</summary>
        public async Task WriteToAsync(Stream stream)
        {
            // TODO: Write everything at once?
            var header = System.Text.Encoding.UTF8.GetBytes("*1\n");
            await stream.WriteAsync(header, 0, header.Length);
            var numberOfItemsInDatagroup = System.Text.Encoding.UTF8.GetBytes($"~{_sizeCount}\n");
            await stream.WriteAsync(numberOfItemsInDatagroup, 0, numberOfItemsInDatagroup.Length);
            await stream.WriteAsync(_holdingBuffer.ToArray(), 0, _holdingBuffer.Count);
        }
    }
}
