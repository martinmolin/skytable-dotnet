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
    /// <summary>This enum represents the data types supported by the Skyhash Protocol.</summary>
    public enum ElementType
    {
        /// <summary>A unicode string value; `<tsymbol>` is `+`.</summary>
        String,
        /// <summary>An unsigned integer value; `<tsymbol>` is `:`.</summary>
        UnsignedInt,
        /// <summary>Arrays can be nested! Their `<tsymbol>` is `&`.</summary>
        Array,
        /// <summary>A response code.</summary>
        RespCode,
        /// <summary>A non-recursive String array; `<tsymbol>` is `_`.</summary>
        FlatArray,
        /// <summary>A binary string value; `<tsymbol>` is `?`.</summary>
        BinaryString
    }
}