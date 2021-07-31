using System.Threading.Tasks;
using Skytable.Client.Querying;

namespace Skytable.Client.Example
{
    class Person : Skyhash
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // Sync Example
            var setPerson = new Person();
            setPerson.Name = "John Doe";
            setPerson.Age = 30;

            var connection = new Connection("127.0.0.1", 2003);
            var setResponse = connection.Set("P", setPerson);
            var getResponse = connection.Get("P");
            var getPerson = connection.Get<Person>("P");

            System.Console.WriteLine(setResponse);
            System.Console.WriteLine(getResponse);
            System.Console.WriteLine(getPerson.Name);
            
            // Async Example
            var setPersonAsync = new Person();
            setPersonAsync.Name = "Jane Doe";
            setPersonAsync.Age = 29;

            setResponse = await connection.SetAsync("PA", setPersonAsync);
            getResponse = await connection.GetAsync("PA");
            getPerson = await connection.GetAsync<Person>("PA");

            System.Console.WriteLine(setResponse);
            System.Console.WriteLine(getResponse);
            System.Console.WriteLine(getPerson.Name);

            // Custom MGET query Example
            var query = new Query();
            query.Push("MGET");
            query.Push("P");
            query.Push("PA");

            var result = connection.RunSimpleQuery(query);
            System.Console.WriteLine(result);
        }
    }
}
