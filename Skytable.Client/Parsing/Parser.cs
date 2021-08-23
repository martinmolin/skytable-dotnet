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
        private const byte SKYHASH_HEADER       = 42;  // *
        private const byte SKYHASH_LINEFEED     = 10;  // \n
        private const byte SKYHASH_STRING       = 43;  // +
        private const byte SKYHASH_U64          = 58;  // :
        private const byte SKYHASH_ARRAY        = 38;  // &
        private const byte SKYHASH_RESPCODE     = 33;  // !
        private const byte SKYHASH_FLATARRAY    = 95;  // _
        private const byte SKYHASH_BINARYSTRING = 63;  // ?
        private const byte SKYHASH_TYPEDARRAY   = 64;  // @

        private int _cursor;
        private List<byte> _buffer;

        public Parser(List<byte> buffer)
        {
            _cursor = 0;
            _buffer = buffer;
        }

        public (ParseResult<Response>, int) Parse()
        {
            var numberOfQueries = ParseMetaframeGetDatagroupCount();
            if (numberOfQueries.IsError)
                return (ParseResult<Response>.Err(numberOfQueries.Error), _cursor);
            if (numberOfQueries.Item == 0)
                return (ParseResult<Response>.Err(ParseError.BadPacket), _cursor);
            
            if (numberOfQueries.Item == 1)
            {
                var result = ParseNextElement();
                if (result.IsError)
                    return (ParseResult<Response>.Err(result.Error), 0);
                
                // No need to check result since we pass in true here. Cannot be an Error.
                if (WillCursorGiveChar((char)SKYHASH_HEADER, true).Item)
                {
                    return (ParseResult<Response>.Ok(new Response(result.Item)), _cursor);
                }

                return (ParseResult<Response>.Err(ParseError.UnexpectedByte), _cursor);
            }

            throw new NotSupportedException("Pipelined queries are not supported yet.");
        }

        private ParseResult<Element> ParseNextElement()
        {
            if (_buffer.Count < _cursor)
                return ParseResult<Element>.Err(ParseError.NotEnough);

            var tsymbol = _buffer[_cursor++];
            switch (tsymbol)
            {
                case SKYHASH_STRING:
                    var str = ParseNextString();
                    if (str.IsError)
                        return ParseResult<Element>.Err(str.Error);
                    return ParseResult<Element>.Ok(new Element(str.Item));
                case SKYHASH_U64:
                    var u64 = ParseNextU64();
                    if (u64.IsError)
                        return ParseResult<Element>.Err(u64.Error);
                    return ParseResult<Element>.Ok(new Element(u64.Item));
                case SKYHASH_ARRAY:
                    var array = ParseNextArray(); //TODO: Recursive
                    if (array.IsError)
                        return ParseResult<Element>.Err(array.Error);
                    return ParseResult<Element>.Ok(new Element(array.Item));
                case SKYHASH_RESPCODE:
                    var respCode = ParseNextRespCode();
                    if (respCode.IsError)
                        return ParseResult<Element>.Err(respCode.Error);
                    return ParseResult<Element>.Ok(new Element(respCode.Item));
                case SKYHASH_FLATARRAY:
                    var flatArray = ParseNextFlatArray();
                    if (flatArray.IsError)
                        return ParseResult<Element>.Err(flatArray.Error);
                    return ParseResult<Element>.Ok(new Element(flatArray.Item));
                case SKYHASH_BINARYSTRING:
                    var binaryString = ParseNextBinaryString();
                    if (binaryString.IsError)
                        return ParseResult<Element>.Err(binaryString.Error);
                    return ParseResult<Element>.Ok(new Element(binaryString.Item));
                case SKYHASH_TYPEDARRAY:
                     // hmmm, a typed array; let's check the tsymbol
                    if (_buffer.Count < _cursor)
                        return ParseResult<Element>.Err(ParseError.NotEnough);

                    // got tsymbol, let's skip it too
                    var typed_tsymbol = _buffer[_cursor++];
                    switch(typed_tsymbol)
                    {
                        case SKYHASH_STRING:
                            var typedArrayStr = ParseNextTypedArrayStr();
                            if (typedArrayStr.IsError)
                                return ParseResult<Element>.Err(typedArrayStr.Error);
                            return ParseResult<Element>.Ok(new Element(typedArrayStr.Item));
                        case SKYHASH_BINARYSTRING:
                            var typedArrayBin = ParseNextTypedArrayBin();
                            if (typedArrayBin.IsError)
                                return ParseResult<Element>.Err(typedArrayBin.Error);
                            return ParseResult<Element>.Ok(new Element(typedArrayBin.Item));
                        default:
                            return ParseResult<Element>.Err(ParseError.UnknownDataType);
                    }
                default:
                    throw new NotImplementedException($"The tsymbol '{tsymbol}' is not yet implemented.");
            }
        }

        private ParseResult<List<Element>> ParseNextArray()
        {
            var (startedAt, stoppedAt) = ReadLine();
            var line = _buffer.GetRange(startedAt, stoppedAt - startedAt);
            if (line.Count == 0)
                return ParseResult<List<Element>>.Err(ParseError.NotEnough);
            
            var size = ParseSize(line);
            if (size.IsError)
                return ParseResult<List<Element>>.Err(size.Error);

            var elements = new List<Element>(size.Item);
            for (int i = 0; i < size.Item; i++)
            {
                var result = ParseNextElement();
                if (result.IsError)
                    return ParseResult<List<Element>>.Err(result.Error);

                elements.Add(result.Item);
            }
            return ParseResult<List<Element>>.Ok(elements);
        }

        private ParseResult<List<string>> ParseNextFlatArray()
        {
            var (startedAt, stoppedAt) = ReadLine();
            var line = _buffer.GetRange(startedAt, stoppedAt - startedAt);
            if (line.Count == 0)
                return ParseResult<List<string>>.Err(ParseError.NotEnough);
            
            var size = ParseSize(line);
            if (size.IsError)
                return ParseResult<List<string>>.Err(size.Error);

            var elements = new List<string>(size.Item);
            for (int i = 0; i < size.Item; i++)
            {
                if (_buffer.Count < _cursor)
                    return ParseResult<List<string>>.Err(ParseError.NotEnough);
                
                // TODO: +, ?, !, : should be supported here. Not just +.
                var tsymbol = _buffer[_cursor++];
                if (tsymbol != SKYHASH_STRING)
                    return ParseResult<List<string>>.Err(ParseError.UnknownDataType);

                var result = ParseNextString();
                if (result.IsError)
                    return ParseResult<List<string>>.Err(result.Error);

                elements.Add(result.Item);
            }

            return ParseResult<List<string>>.Ok(elements);
        }

        private ParseResult<List<string>> ParseNextTypedArrayStr()
        {
            var (startedAt, stoppedAt) = ReadLine();
            var line = _buffer.GetRange(startedAt, stoppedAt - startedAt);
            if (line.Count == 0)
                return ParseResult<List<string>>.Err(ParseError.NotEnough);

            // so we have a size chunk; let's get the size
            var size = ParseSize(line);
            if (size.IsError)
                return ParseResult<List<string>>.Err(size.Error);

            var elements = new List<string>(size.Item);
            for (int i = 0; i < size.Item; i++)
            {
                // no tsymbol, just elements and their sizes
                var result = ParseNextStringNullcheck();
                if (result.IsError)
                    return ParseResult<List<string>>.Err(result.Error);
                elements.Add(result.Item);
            }
            return ParseResult<List<string>>.Ok(elements);
        }

        private ParseResult<List<List<byte>>> ParseNextTypedArrayBin()
        {
            var (startedAt, stoppedAt) = ReadLine();
            var line = _buffer.GetRange(startedAt, stoppedAt - startedAt);
            if (line.Count == 0)
                return ParseResult<List<List<byte>>>.Err(ParseError.NotEnough);

            // so we have a size chunk; let's get the size
            var size = ParseSize(line);
            if (size.IsError)
                return ParseResult<List<List<byte>>>.Err(size.Error);

            var elements = new List<List<byte>>(size.Item);
            for (int i = 0; i < size.Item; i++)
            {
                // no tsymbol, just elements and their sizes
                var result = ParseNextBinaryStringNullcheck();
                if (result.IsError)
                    return ParseResult<List<List<byte>>>.Err(result.Error);
                elements.Add(result.Item);
            }
            return ParseResult<List<List<byte>>>.Ok(elements);
        }

        private ParseResult<ulong> ParseNextU64()
        {
            var ourU64Chunk = GetNextElement();
            if (ourU64Chunk.IsError)
                return ParseResult<ulong>.Err(ourU64Chunk.Error);

            var ourU64 = ParseU64(ourU64Chunk.Item);
            if (ourU64.IsError)
                return ourU64;
            
            var result = WillCursorGiveLineFeed();
            if (result.IsOk && result.Item)
            {
                _cursor++;
                return ParseResult<ulong>.Ok(ourU64.Item);
            }

            return ParseResult<ulong>.Err(ParseError.UnexpectedByte);
        }

        private ParseResult<List<byte>> ReadUntil(int until)
        {
            if (_buffer.Count < _cursor + until)
                return ParseResult<List<byte>>.Err(ParseError.NotEnough);

            var range = _buffer.GetRange(_cursor, until);
            _cursor += until;
            return ParseResult<List<byte>>.Ok(range);
        }

        private ParseResult<List<byte>> GetNextElement()
        {
            var (startedAt, stoppedAt) = ReadLine();
            var line = _buffer.GetRange(startedAt, stoppedAt - startedAt);
            var size = ParseSize(line);
            if (size.IsError)
                return ParseResult<List<byte>>.Err(size.Error);
            return ReadUntil(size.Item);
        }

        private ParseResult<bool> WillCursorGiveChar(char c, bool thisIfNothingAhead)
        {
            if (_buffer.Count <= _cursor)
            {
                if (thisIfNothingAhead)
                    return ParseResult<bool>.Ok(true);
                return ParseResult<bool>.Err(ParseError.NotEnough);
            }
            
            return ParseResult<bool>.Ok(_buffer[_cursor] == c);
        }

        private ParseResult<bool> WillCursorGiveLineFeed()
        {
            return WillCursorGiveChar((char)SKYHASH_LINEFEED, false);
        }

        private ParseResult<string> ParseNextString()
        {
            var ourStringChunk = GetNextElement();
            if (ourStringChunk.IsError)
                return ParseResult<string>.Err(ourStringChunk.Error);

            var ourString = Encoding.UTF8.GetString(ourStringChunk.Item.ToArray());
            var result = WillCursorGiveLineFeed();
            if (result.IsOk && result.Item)
            {
                _cursor++;
                return ParseResult<string>.Ok(ourString);
            }

            return ParseResult<string>.Err(ParseError.UnexpectedByte);
        }

        private ParseResult<List<byte>> ParseNextBinaryString()
        {
            var ourStringChunk = GetNextElement();
            if (ourStringChunk.IsError)
                return ourStringChunk;

            var result = WillCursorGiveLineFeed();
            if (result.IsOk && result.Item)
            {
                // there is a lf after the end of the binary string; great!
                // let's skip that now
                _cursor++;
                // let's return our string
                return ParseResult<List<byte>>.Ok(ourStringChunk.Item);
            }

            return ParseResult<List<byte>>.Err(ParseError.UnexpectedByte);
        }

        /// Parse the next null checked element
        public ParseResult<List<byte>> ParseNextChunkNullcheck()
        {
            var (startedAt, stoppedAt) = ReadLine();
            var sizeLine = _buffer.GetRange(startedAt, stoppedAt - startedAt);
            if (sizeLine.Count == 0)
                return ParseResult<List<byte>>.Err(ParseError.NotEnough);

            var result = ParseSize(sizeLine);
            if (result.IsError)
                return ParseResult<List<byte>>.Err(result.Error);
            return ReadUntil(result.Item);
        }

        public ParseResult<List<byte>> ParseNextBinaryStringNullcheck()
        {
            var ourChunk = ParseNextChunkNullcheck();
            if (ourChunk.IsError)
                return ourChunk;
            
            var result = WillCursorGiveLineFeed();
            if (result.IsOk && result.Item)
            {
                _cursor++;
                return ParseResult<List<byte>>.Ok(ourChunk.Item);
            }
            return ParseResult<List<byte>>.Err(ParseError.UnexpectedByte);
        }

        public ParseResult<string> ParseNextStringNullcheck()
        {
            var ourChunk = ParseNextBinaryStringNullcheck();
            if (ourChunk.IsError)
                return ParseResult<string>.Err(ourChunk.Error);

            return ParseResult<string>.Ok(Encoding.UTF8.GetString(ourChunk.Item.ToArray()));
        }

        private ParseResult<RespCode> ParseNextRespCode()
        {
            var ourRespcodeChunk = GetNextElement();
            if (ourRespcodeChunk.IsError)
                return ParseResult<RespCode>.Err(ourRespcodeChunk.Error);

            var ourRespCode = Encoding.UTF8.GetString(ourRespcodeChunk.Item.ToArray());
            var result = WillCursorGiveLineFeed();
            if (result.IsOk && result.Item)
            {
                _cursor++;
                return ParseResult<RespCode>.Ok(Enum.Parse<RespCode>(ourRespCode));
            }

            return ParseResult<RespCode>.Err(ParseError.UnexpectedByte);
        }

        /// This will return the number of datagroups present in this query packet
        ///
        /// This **will forward the cursor itself**
        private ParseResult<int> ParseMetaframeGetDatagroupCount()
        {
            // The smallest query we can have is: `*1\n` or 3 chars
            if (_buffer.Count < 3)
                return ParseResult<int>.Err(ParseError.NotEnough);
            
            // Now we want to read `*<n>\n`
            var (startedAt, stoppedAt) = ReadLine();
            var ourChunk = _buffer.GetRange(startedAt, stoppedAt - startedAt);

            if (ourChunk[0] == SKYHASH_HEADER)
                return ParseSize(ourChunk.GetRange(1, ourChunk.Count - 1));

            return ParseResult<int>.Err(ParseError.UnexpectedByte);
        }

        private ParseResult<int> ParseSize(List<byte> bytes)
        {
            if (bytes.Count == 0)
                return ParseResult<int>.Err(ParseError.NotEnough);
            
            int itemSize = 0;
            foreach (byte digit in bytes)
            {
                var c = (char)digit;
                if (!char.IsDigit(c))
                    return ParseResult<int>.Err(ParseError.DataTypeParseError);
                
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
                    return ParseResult<int>.Err(ParseError.DataTypeParseError);
                }
            }

            return ParseResult<int>.Ok(itemSize);
        }

        private ParseResult<ulong> ParseU64(List<byte> bytes)
        {
            if (bytes.Count == 0)
                return ParseResult<ulong>.Err(ParseError.NotEnough);

            ulong itemU64 = 0;
            foreach (byte digit in bytes)
            {
                var c = (char)digit;
                if (!char.IsDigit(c))
                    return ParseResult<ulong>.Err(ParseError.DataTypeParseError);
                
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
                    return ParseResult<ulong>.Err(ParseError.DataTypeParseError);
                }
            }

            return ParseResult<ulong>.Ok(itemU64);
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