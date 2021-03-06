using System;
using Skytable.Client;

namespace SkyhashExample
{
    // Inherit from Skyhash to automatically implement From/Into JSON for your type.
    // From will be used when calling Connection.Set. Default implementation is JSON.
    // Into will be used when calling Connection.Get. Default implementation is JSON.
    // You may override From and Into in order to change the storage format of this type.
    class Person : Skyhash
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

            // Connect to Skytable
            var connection = new Connection("127.0.0.1", 2003);
            connection.Connect();

            // Set the Key 'Person' to a JSON string representing the 'setPerson' object.
            var setResult = connection.Set("Person", setPerson);

            // 'person' now contains a SkyResult<Person> with the name "John Doe" and age 30.
            var personResult = connection.Get<Person>("Person");

            if (personResult.IsOk)
                Console.WriteLine($"Name: {personResult.Item.Name}, Age: {personResult.Item.Age}");
            else
                Console.WriteLine(personResult.Error);
        }
    }
}
