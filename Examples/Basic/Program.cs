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

            // Set the Key 'Basic' to "MyValue"
            var setResponse = connection.Set("Basic", "MyValue");

            // Get the value "MyValue" of the Key 'Basic'.
            var getResponse = connection.Get("Basic");

            if (getResponse.IsOk)
                Console.WriteLine(getResponse.Item);
            else
                Console.WriteLine(getResponse.Error);
        }
    }
}
