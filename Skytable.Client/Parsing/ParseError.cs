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

    /// <summary>Errors that can be returned by the <see cref="Parser"/>.</summary>
    public enum ParseError
    {
        /// <summary>NotEnough occurs when there are not enough bytes left to complete a so far well structured packet.</summary>
        NotEnough,
        /// <summary>UnexpectedByte occurs when a packet is not constructed correctly. Unable to continue parsing.</summary>
        UnexpectedByte,
        /// <summary>
        /// The packet simply contains invalid data.
        /// This is rarely returned and only in the special cases where a bad client sends `0` as
        /// the query count.
        ///</summary>
        BadPacket,
        /// <summary>A data type was given but the parser failed to serialize it into this type.</summary>
        DataTypeParseError,
        /// <summary>
        /// A data type that the client doesn't know was passed into the query.
        ///
        /// This is a frequent problem that can arise between different server editions as more data types
        /// can be added with changing server versions.
        /// </summary>
        UnknownDataType,
        /// <summary>The query is empty.</summary>
        Empty
    }
}