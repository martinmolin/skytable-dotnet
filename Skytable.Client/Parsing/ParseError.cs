namespace Skytable.Client.Parsing
{
    public enum ParseError
    {
        NotEnough,
        UnexpectedByte,
        BadPacket,
        DataTypeParseError,
        UnknownDataType,
        Empty
    }
}