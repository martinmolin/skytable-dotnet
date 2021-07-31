namespace Skytable.Client.Querying
{
    public class Response
    {
        public Element Element { get; }

        public Response(Element element)
        {
            Element = element;
        }

        public override string ToString()
        {
            if (Element == null)
                return base.ToString();
            return $"Response(Element={Element.ToString()})";
        }
    }
}