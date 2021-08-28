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
            var setKeyOneResponse = connection.Set("KeyOne", "MyValueOne");
            // Set the Key 'KeyTwo' to "MyValueTwo"
            var setKeyTwoResponse = connection.Set("KeyTwo", "MyValueTwo");

            // Create an MGET Query. See https://docs.skytable.io/actions/mget
            var query = new Query();
            query.Push("MGET");
            query.Push("KeyOne");
            query.Push("KeyTwo");

            // Response contains the values of both 'KeyOne' and 'KeyTwo'.
            var mgetResponse = connection.RunSimpleQuery(query);

            if (mgetResponse.IsOk)
                Console.WriteLine(mgetResponse.Item);
            else
                Console.WriteLine(mgetResponse.Error);
        }
    }
}
