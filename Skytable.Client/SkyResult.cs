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
    public class SkyResult<T>
    {
        public T Item { get; }
        public ParseError Error { get; }
        public bool IsOk { get; }
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

        internal static SkyResult<T> Ok(T result)
        {
            return new SkyResult<T>(result);
        }

        internal static SkyResult<T> Err(ParseError error)
        {
            return new SkyResult<T>(error);
        }
    }    
}
