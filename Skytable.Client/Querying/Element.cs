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
    public class Element
    {
        public object Item { get; }
        public ElementType Type { get; }

        public Element(string s)
        {
            Item = s;
            Type = ElementType.String;
        }

        public Element(ulong n)
        {
            Item = n;
            Type = ElementType.UnsignedInt;
        }

        public Element(RespCode r)
        {
            Item = r;
            Type = ElementType.RespCode;
        }

        public Element(List<Element> elements)
        {
            Item = elements;
            Type = ElementType.Array;
        }

        public Element(List<string> strings)
        {
            Item = strings;
            Type = ElementType.FlatArray;
        }

        public override string ToString()
        {
            if (Item == null)
                return $"{Type}({base.ToString()})";
            return $"{Type}({Item})";
        }
    }
}