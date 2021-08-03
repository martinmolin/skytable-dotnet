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

            // Set the Key 'BasicAsync' to "MyValue"
            var setResponse = await connection.SetAsync("BasicAsync", "MyValue");

            // Get the value "MyValue" of the Key 'BasicAsync'.
            var getResponse = await connection.GetAsync("BasicAsync");
        }
    }
}
