namespace Skytable.Client.Parsing
{
    public class ParseResult<T>
    {
        public T Item { get; }
        public ParseError Error { get; }
        public bool IsOk { get; }
        public bool IsError { get; }

        private ParseResult(T item)
        {
            Item = item;
            IsOk = true;
            IsError = false;
        }

        private ParseResult(ParseError error)
        {
            Error = error;
            IsOk = false;
            IsError = true;
        }

        public static ParseResult<T> Ok(T result)
        {
            return new ParseResult<T>(result);
        }

        public static ParseResult<T> Err(ParseError error)
        {
            return new ParseResult<T>(error);
        }
    }    
}
