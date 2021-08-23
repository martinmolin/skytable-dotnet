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

namespace Skytable.Client.Querying
{
    /// <summary>A server response containing an <see cref="Element"/>.</summary>
    public class Response
    {
        /// <summary>Gets the Element of this <see cref="Response"/>.</summary>
        public Element Element { get; }

        internal Response(Element element)
        {
            Element = element;
        }

        /// <summary>Returns a string with the format Response(Element=(ElementType(Item)).</summary>
        public override string ToString()
        {
            if (Element == null)
                return base.ToString();
            return $"Response(Element={Element.ToString()})";
        }
    }
}
