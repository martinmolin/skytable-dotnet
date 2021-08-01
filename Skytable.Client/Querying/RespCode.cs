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
    public enum RespCode : byte
    {
        Okay = 0,
        NotFound = 1,
        OverwriteError = 2,
        ActionError = 3,
        ServerError = 4,
        ErrorString = 5,
        OtherError = 6,
        WrongType = 7
    }
}