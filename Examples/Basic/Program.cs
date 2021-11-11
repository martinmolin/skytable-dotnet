using System;
using Skytable.Client;

namespace Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            // Connect to Skytable
            var connection = new Connection("127.0.0.1", 2003);
            connection.Connect();

            // Set the Key 'Basic' to "MyValue"
            var setResult = connection.Set("Basic", "MyValue");

            // Get the value "MyValue" of the Key 'Basic'.
            var getResult = connection.Get("Basic");

            if (getResult.IsOk)
                Console.WriteLine(getResult.Item);
            else
                Console.WriteLine(getResult.Error);
        }
    }
}
