using System;
using System.Threading.Tasks;
using Skytable.Client;

namespace BasicAsync
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Connect to Skytable
            var connection = new Connection("127.0.0.1", 2003);
            await connection.ConnectAsync();

            // Set the Key 'BasicAsync' to "MyValue"
            var setResult = await connection.SetAsync("BasicAsync", "MyValue");

            // Get the value "MyValue" of the Key 'BasicAsync'.
            var getResult = await connection.GetAsync("BasicAsync");
            
            if (getResult.IsOk)
                Console.WriteLine(getResult.Item);
            else
                Console.WriteLine(getResult.Error);
        }
    }
}
