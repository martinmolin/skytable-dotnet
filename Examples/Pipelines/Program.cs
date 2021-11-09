using System;
using Skytable.Client;
using Skytable.Client.Querying;

namespace Pipelines
{
    class Program
    {
        static void Main(string[] args)
        {
            // Connect to Skytable
            var connection = new Connection("127.0.0.1", 2003);
            connection.Connect();

            // Create a query that will set the Key 'Pipeline' to "MyValue"
            var setQuery = new Query();
            setQuery.Push("SET");
            setQuery.Push("Pipeline");
            setQuery.Push("MyValue");

            // Create a query that will get the value "MyValue" of the Key 'Pipeline'
            var getQuery = new Query();
            getQuery.Push("GET");
            getQuery.Push("Pipeline");

            // Create a Pipeline that will execute both queries with one server call.
            var pipeline = new Pipeline()
                .Add(setQuery)
                .Add(getQuery);

            // Run the pipeline and get the response.
            var response = connection.RunPipeline(pipeline);

            if (response.IsOk)
                Console.WriteLine(response.Item);
            else
                Console.WriteLine(response.Error);
        }
    }
}
