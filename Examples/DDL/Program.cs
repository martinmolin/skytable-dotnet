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
            connection.Connect();

            // Change the entity (keyspace:table) that we wish to query.
            var useResult = connection.Use("MyKeyspace", "MyTable");

            if (useResult.IsOk)
                Console.WriteLine(useResult.Item);
            else
                Console.WriteLine(useResult.Error);
        }
    }
}
