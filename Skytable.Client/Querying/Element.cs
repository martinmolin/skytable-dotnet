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

namespace Skytable.Client.Querying
{
    /// <summary>Represents the data types supported by the Skyhash protocol.</summary>
    public class Element
    {
        /// <summary>Gets the object that this element represents. It is of type <see cref="ElementType"/>.</summary>
        public object Object { get; }
        
        /// <summary>Gets the <see cref="ElementType"/> of the Object that this element represents.</summary>
        public ElementType Type { get; }

        internal Element(string s)
        {
            Object = s;
            Type = ElementType.String;
        }

        internal Element(ulong n)
        {
            Object = n;
            Type = ElementType.UnsignedInt;
        }

        internal Element(ResponseCode r)
        {
            Object = r;
            Type = ElementType.RespCode;
        }

        internal Element(List<Element> elements)
        {
            Object = elements;
            Type = ElementType.Array;
        }

        internal Element(List<string> strings)
        {
            Object = strings;
            Type = ElementType.FlatArray;
        }

        internal Element(List<byte> binaryString)
        {
            Object = binaryString;
            Type = ElementType.BinaryString;
        }

        internal Element(List<List<byte>> binaryStrings)
        {
            Object = binaryStrings;
            Type = ElementType.FlatArray;
        }

        /// <summary>Returns a string with the format ElementType(Item).</summary>
        public override string ToString()
        {
            if (Object == null)
                return $"{Type}({base.ToString()})";
            switch (Type)
            {
                case ElementType.BinaryString:
                    return $"{Type}({string.Join(", ", Object as List<byte>)})";
                default:
                    return $"{Type}({Object})";
            }
            
        }
    }
}