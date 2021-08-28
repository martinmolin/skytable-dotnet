using System.Threading;
using Skytable.Client;

namespace Pool
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a connection pool that targets the entity default:default and
            // allows temporary connections to be created in case the pool runs out of available connections.
            var pool = new ConnectionPool("127.0.0.1", 2003, "default", "default", true);

            // Initialize the pool with 10 connections.
            pool.Initialize(10);

            // Grab a connection from the pool.
            using (var connection = pool.Connection())
            {
                // Set the Key 'Pool' to "MyValue"
                var setResponse = connection.Set("Pool", "MyValue");
            }

            var threads = new Thread[4];

            // Spawning threads.
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(Run);
                threads[i].Start(pool);
                System.Console.WriteLine($"Spawned thread {i}");
            }

            // Waiting for threads to finish work.
            foreach (var thread in threads)
            {
                thread.Join();
                System.Console.WriteLine("Joined thread");
            }

            System.Console.WriteLine("Done");
        }

        static void Run(object p)
        {
            var pool = p as ConnectionPool;

            for (int i = 0; i < 4; i++)
            {
                // Grab a connection from the pool.
                using var connection = pool.Connection();

                // Get the value "MyValue" of the Key 'Pool'.
                var getResponse = connection.Get("Pool");
                System.Console.WriteLine($"{i} - {getResponse}");
            }
        }
    }
}
