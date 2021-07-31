namespace Skytable.Client.Parsing
{
    [System.Serializable]
    public class ParseException : System.Exception
    {
        public ParseError Error { get; }
        public ParseException(ParseError error)
        {
            Error = error;
        }

        public ParseException(string message, ParseError error) : base(message)
        {
            Error = error;
        }

        public ParseException(string message, System.Exception inner, ParseError error) : base(message, inner)
        {
            Error = error;
        }

        protected ParseException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}