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

using System;

namespace Skytable.Client.Querying
{
    /// <summary>A Skytable query response code.</summary>
    public class ResponseCode
    {
        /// <summary>The actual response code.</summary>
        public RespCode Code { get; }

        /// <summary>A message from the server if the RespCode was <see cref="RespCode.OtherError"/>.</summary>
        public string Error { get; }

        private ResponseCode(RespCode code)
        {
            Code = code;
        }

        private ResponseCode(string error)
        {
            Code = RespCode.OtherError;
            Error = error;
        }

        internal static ResponseCode From(string code)
        {
            if (Enum.TryParse<RespCode>(code, false, out var result))
                return new ResponseCode(result);
            else
                return new ResponseCode(code);
        }

        public override string ToString()
        {
            if (Error == null)
                return Code.ToString();
            else
                return $"{Code}({Error})";
        }
    }   
}
