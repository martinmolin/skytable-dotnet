using System.Text.Json;
using Skytable.Client.Querying;

namespace Skytable.Client
{
    public abstract class Skyhash<T> where T: class
    {
        public virtual T From(Response response)
        {
            switch(response.Element.Type)
            {
                case ElementType.String:
                    return JsonSerializer.Deserialize<T>(response.Element.Item as string);
                default:
                    return default(T);
            }
        }

        public virtual string Into()
        {
            return JsonSerializer.Serialize<T>(this as T);
        }
    }
}