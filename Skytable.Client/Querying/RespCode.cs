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
    /// <summary>Response codes returned by the server.</summary>
    public enum RespCode : byte
    {
        /// <summary>Returned when a query is successful but contains no response data.</summary>
        Okay = 0,
        /// <summary>Returned when an element is not found using the specified key.</summary>
        NotFound = 1,
        /// <summary>Returned when trying to SET a key that already exists. Use UPDATE instead.</summary>
        OverwriteError = 2,
        /// <summary>The action did not expect the arguments sent.</summary>
        ActionError = 3,
        /// <summary>The packet contains invalid data.</summary>
        PacketError = 4,
        /// <summary>An error occurred on the server side.</summary>
        ServerError = 5,
        /// <summary>Some other error occurred and the server returned a description of this error. See this <see href="https://docs.skytable.io/protocol/errors">document</see></summary>
        OtherError = 6,
        /// <summary>WrongType Error.</summary>
        WrongType = 7
    }
}