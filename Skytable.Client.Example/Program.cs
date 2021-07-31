using System;
using Skytable.Client;

namespace Skytable.Client.Example
{
    class Person : Skyhash<Person>
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
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
        }
    }
}
