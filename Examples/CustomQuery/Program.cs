using System;
using Skytable.Client;
using Skytable.Client.Querying;

namespace CustomQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            // Connect to Skytable
            var connection = new Connection("127.0.0.1", 2003);
            connection.Connect();

            // Set the Key 'KeyOne' to "MyValueOne"
            var setKeyOneResult = connection.Set("KeyOne", "MyValueOne");
            // Set the Key 'KeyTwo' to "MyValueTwo"
            var setKeyTwoResult = connection.Set("KeyTwo", "MyValueTwo");

            // Create an MGET Query. See https://docs.skytable.io/actions/mget
            var query = new Query();
            query.Push("MGET");
            query.Push("KeyOne");
            query.Push("KeyTwo");

            // Result contains the values of both 'KeyOne' and 'KeyTwo'.
            var mgetResult = connection.RunSimpleQuery(query);

            if (mgetResult.IsOk)
                Console.WriteLine(mgetResult.Item);
            else
                Console.WriteLine(mgetResult.Error);
        }
    }
}
