using System;
using Skytable.Client;

namespace DDL
{
    class Program
    {
        static void Main(string[] args)
        {
            // Connect to Skytable
            var connection = new Connection("127.0.0.1", 2003);

            // Change the entity (keyspace:table) that we wish to query.
            var useResponse = connection.Use("MyKeyspace", "MyTable");

            if (useResponse.IsOk)
                Console.WriteLine(useResponse.Item);
            else
                Console.WriteLine(useResponse.Error);
        }
    }
}
