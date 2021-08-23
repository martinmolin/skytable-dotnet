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

namespace Skytable.Client.Parsing
{
    [System.Serializable]
    public class ParseException : System.Exception
    {
        public ParseError Error { get; }

        public ParseException(ParseError error)
        {
            Error = error;
        }

        public ParseException(string message, ParseError error) : base(message)
        {
            Error = error;
        }

        public ParseException(string message, System.Exception inner, ParseError error) : base(message, inner)
        {
            Error = error;
        }

        protected ParseException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}