namespace Skytable.Client.Querying
{
    public class Element
    {
        public object Item { get; }
        public ElementType Type { get; }

        public Element(string s)
        {
            Item = s;
            Type = ElementType.String;
        }

        public Element(ulong n)
        {
            Item = n;
            Type = ElementType.UnsignedInt;
        }

        public Element(RespCode r)
        {
            Item = r;
            Type = ElementType.RespCode;
        }

        public override string ToString()
        {
            if (Item == null)
                return $"{Type}({base.ToString()})";
            return $"{Type}({Item})";
        }
    }
}