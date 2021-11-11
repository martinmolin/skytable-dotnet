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

using Skytable.Client.Parsing;

namespace Skytable.Client
{
    /// <summary>The result of a Skytable Query.</summary>
    public class SkyResult<T>
    {
        /// <summary>The item of type T if the result is Ok.</summary>
        public T Item { get; }
        /// <summary>The error in case the result is an Error.</summary>
        public ParseError Error { get; }
        /// <summary>True if the result was ok. You can use the Item.</summary>
        public bool IsOk { get; }
        /// <summary>True if the query failed in any way. Check the Error to learn more.</summary>
        public bool IsError { get; }

        private SkyResult(T item)
        {
            Item = item;
            IsOk = true;
            IsError = false;
        }

        private SkyResult(ParseError error)
        {
            Error = error;
            IsOk = false;
            IsError = true;
        }

        internal static SkyResult<T> Ok(T item)
        {
            return new SkyResult<T>(item);
        }

        internal static SkyResult<T> Err(ParseError error)
        {
            return new SkyResult<T>(error);
        }
    }    
}
