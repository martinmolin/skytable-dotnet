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
using System.Collections.Generic;
using System.Text;
using Skytable.Client.Querying;

namespace Skytable.Client.Parsing
{
    public class Parser
    {
        // SKYHASH protocol constants
        private const byte SKYHASH_HEADER    = 42;  // *
        private const byte SKYHASH_LINEFEED  = 10;  // \n
        private const byte SKYHASH_STRING    = 43;  // +
        private const byte SKYHASH_U64       = 58;  // :
        private const byte SKYHASH_ARRAY     = 38;  // &
        private const byte SKYHASH_RESPCODE  = 33;  // !
        private const byte SKYHASH_FLATARRAY = 95;  // _

        private int _cursor;
        private List<byte> _buffer;

        public Parser(List<byte> buffer)
        {
            _cursor = 0;
            _buffer = buffer;
        }

        public (Response, int) Parse()
        {
            var numberOfQueries = ParseMetaframeGetDatagroupCount();
            if (numberOfQueries == 0)
                throw new ParseException(ParseError.BadPacket);
            
            if (numberOfQueries == 1)
            {
                var element = ParseNextElement();
                
                if (WillCursorGiveChar((char)SKYHASH_HEADER, true))
                {
                    return (new Response(element), _cursor);
                }

                throw new ParseException(ParseError.UnexpectedByte);
            }

            throw new NotSupportedException("Pipelined queries are not supported yet.");
        }

        private Element ParseNextElement()
        {
            if (_buffer.Count < _cursor)
                throw new ParseException(ParseError.NotEnough);

            var tsymbol = _buffer[_cursor++];
            switch (tsymbol)
            {
                case SKYHASH_STRING:
                    var str = ParseNextString();
                    if (str == null)
                        throw new ParseException(ParseError.DataTypeParseError);
                    return new Element(str);
                case SKYHASH_U64:
                    var u64 = ParseNextU64();
                    return new Element(u64);
                case SKYHASH_ARRAY:
                    var array = ParseNextArray();
                    return new Element(array);
                case SKYHASH_RESPCODE:
                    var respCode = ParseNextRespCode();
                    return new Element(respCode);
                case SKYHASH_FLATARRAY:
                    var flatArray = ParseNextFlatArray();
                    return new Element(flatArray);
                default:
                    throw new NotImplementedException($"The tsymbol '{tsymbol}' is not yet implemented.");
            }
        }

        private List<Element> ParseNextArray()
        {
            var (startedAt, stoppedAt) = ReadLine();
            var line = _buffer.GetRange(startedAt, stoppedAt - startedAt);
            if (line.Count == 0)
                throw new ParseException(ParseError.NotEnough);
            
            var size = (int)ParseSize(line);
            var elements = new List<Element>((int)size);
            for (int i = 0; i < size; i++)
            {
                elements.Add(ParseNextElement());
            }
            return elements;
        }

        private List<string> ParseNextFlatArray()
        {
            var (startedAt, stoppedAt) = ReadLine();
            var line = _buffer.GetRange(startedAt, stoppedAt - startedAt);
            if (line.Count == 0)
                throw new ParseException(ParseError.NotEnough);
            
            var size = (int)ParseSize(line);
            var elements = new List<string>((int)size);
            for (int i = 0; i < size; i++)
            {
                if (_buffer.Count < _cursor)
                    throw new ParseException(ParseError.NotEnough);
                
                var tsymbol = _buffer[_cursor++];
                if (tsymbol != SKYHASH_STRING)
                    throw new ParseException(ParseError.UnknownDataType);

                elements.Add(ParseNextString());
            }
            return elements;
        }

        private ulong ParseNextU64()
        {
            var ourU64Chunk = GetNextElement();
            var ourU64 = ParseU64(ourU64Chunk);
            if (WillCursorGiveLineFeed())
            {
                _cursor++;
                return ourU64;
            }

            throw new ParseException(ParseError.UnexpectedByte);
        }

        private List<byte> ReadUntil(int until)
        {
            if (_buffer.Count < _cursor + until)
                throw new ParseException(ParseError.NotEnough);

            var range = _buffer.GetRange(_cursor, until);
            _cursor += until;
            return range;
        }

        private List<byte> GetNextElement()
        {
            var (startedAt, stoppedAt) = ReadLine();
            var line = _buffer.GetRange(startedAt, stoppedAt - startedAt);
            var size = ParseSize(line);
            return ReadUntil((int)size);
        }

        private bool WillCursorGiveChar(char c, bool thisIfNothingAhead)
        {
            if (_buffer.Count <= _cursor)
            {
                if (thisIfNothingAhead)
                    return true;
                throw new ParseException(ParseError.NotEnough);
            }
            
            return _buffer[_cursor] == c;
        }

        private bool WillCursorGiveLineFeed()
        {
            return WillCursorGiveChar((char)SKYHASH_LINEFEED, false);
        }

        private string ParseNextString()
        {
            var ourStringChunk = GetNextElement();
            var ourString = Encoding.UTF8.GetString(ourStringChunk.ToArray());
            if (WillCursorGiveLineFeed())
            {
                _cursor++;
                return ourString;
            }

            throw new ParseException(ParseError.UnexpectedByte);
        }

        private RespCode ParseNextRespCode()
        {
            var ourRespcodeChunk = GetNextElement();
            var ourRespCode = Encoding.UTF8.GetString(ourRespcodeChunk.ToArray());
            if (WillCursorGiveLineFeed())
            {
                _cursor++;
                return Enum.Parse<RespCode>(ourRespCode);
            }

            throw new ParseException(ParseError.UnexpectedByte);
        }

        /// This will return the number of datagroups present in this query packet
        ///
        /// This **will forward the cursor itself**
        private nuint ParseMetaframeGetDatagroupCount()
        {
            // The smallest query we can have is: `*1\n` or 3 chars
            if (_buffer.Count < 3) 
                throw new ParseException(ParseError.NotEnough);
            
            // Now we want to read `*<n>\n`
            var (startedAt, stoppedAt) = ReadLine();
            var ourChunk = _buffer.GetRange(startedAt, stoppedAt - startedAt);

            if (ourChunk[0] == SKYHASH_HEADER)
                return ParseSize(ourChunk.GetRange(1, ourChunk.Count - 1));

            throw new ParseException(ParseError.UnexpectedByte);
        }

        private nuint ParseSize(List<byte> bytes)
        {
            if (bytes.Count == 0)
                throw new ParseException(ParseError.NotEnough);
            
            nuint itemSize = 0;
            foreach (byte digit in bytes)
            {
                var c = (char)digit;
                if (!char.IsDigit(c))
                    throw new ParseException(ParseError.DataTypeParseError);
                
                // 48 is the ASCII code for 0, and 57 is the ascii code for 9
                // so if 0 is given, the subtraction should give 0; similarly
                // if 9 is given, the subtraction should give us 9!
                try
                {
                    checked
                    {
                        byte curDig = (byte)(digit - 48);
                        var product = itemSize * 10;
                        itemSize = product + curDig;
                    }
                }
                catch(OverflowException)
                {
                    throw new ParseException(ParseError.DataTypeParseError);
                }
            }

            return itemSize;
        }

        private ulong ParseU64(List<byte> bytes)
        {
            if (bytes.Count == 0)
                throw new ParseException(ParseError.NotEnough);

            ulong itemU64 = 0;
            foreach (byte digit in bytes)
            {
                var c = (char)digit;
                if (!char.IsDigit(c))
                    throw new ParseException(ParseError.DataTypeParseError);
                
                // 48 is the ASCII code for 0, and 57 is the ascii code for 9
                // so if 0 is given, the subtraction should give 0; similarly
                // if 9 is given, the subtraction should give us 9!
                try
                {
                    checked
                    {
                        byte curDig = (byte)(digit - 48);
                        var product = itemU64 * 10;
                        itemU64 = product + curDig;
                    }
                }
                catch(OverflowException)
                {
                    throw new ParseException(ParseError.DataTypeParseError);
                }
            }

            return itemU64;
        }
            

        private (int, int) ReadLine()
        {
            var startedAt = _cursor;
            var stoppedAt = _cursor;

            while (_cursor < _buffer.Count)
            {
                if (_buffer[_cursor] == SKYHASH_LINEFEED)
                {
                    _cursor++;
                    break;
                }

                _cursor++;
                stoppedAt += 1;
            }

            return (startedAt, stoppedAt);
        }
    }
}