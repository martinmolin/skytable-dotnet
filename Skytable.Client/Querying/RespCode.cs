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