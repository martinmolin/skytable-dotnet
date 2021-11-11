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
using System.Text.Json;
using Skytable.Client.Querying;

namespace Skytable.Client
{
    /// <summary>
    /// A type inheriting from Skyhash can be used in queries.
    /// </summary>
    public abstract class Skyhash
    {
        /// <summary>
        /// This function will by default try to deserialize a <see cref="Element"/> into type T using the <see cref="JsonSerializer"/>.
        /// </summary>
        public virtual T From<T>(Element element)
        {
            switch(element.Type)
            {
                case ElementType.String:
                    return JsonSerializer.Deserialize<T>(element.Object as string);
                case ElementType.BinaryString:
                    var bytes = (element.Object as List<byte>).ToArray();
                    return JsonSerializer.Deserialize<T>(bytes);
                default:
                    return default(T);
            }
        }

        /// <summary>
        /// This function will by default try to serialize the object into JSON using the <see cref="JsonSerializer"/>.
        /// </summary>
        public virtual string Into()
        {
            return JsonSerializer.Serialize(this, this.GetType());
        }
    }
}