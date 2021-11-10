using System;
using System.Diagnostics;
using Skytable.Client;
using Skytable.Client.Querying;

namespace Skytable.Benchmark
{
    class Program
    {
        static Connection _connection;

        static void Main(string[] args)
        {
            _connection = new Connection("127.0.0.1", 2003);
            _connection.Connect();

            // If you want to clean the default table before running the benchmarks
            // you can use this query.
            
            // var query = new Query();
            // query.Push("flushdb");
            // _connection.RunSimpleQuery(query);

            var iterations = 10000;
            SetBenchmark(iterations);
            GetBenchmark(iterations);
        }

        static void SetBenchmark(int iterations)
        {
            Console.WriteLine($"Running {iterations} iterations of the SET query.");
            var benchmark = new Benchmark();
            benchmark.Run(Set, iterations);
            benchmark.Report();
        }

        static void GetBenchmark(int iterations)
        {
            Console.WriteLine($"Running {iterations} iterations of the GET query.");
            var benchmark = new Benchmark();
            benchmark.Run(Get, iterations);
            benchmark.Report();
        }

        static void Set(int i)
        {
            var result = _connection.Set(i.ToString(), "A");
            if (result.IsError)
                throw new Exception("Set failed.");
        }

        static void Get(int i)
        {
            var result = _connection.Get(i.ToString());
            if (result.IsError)
                throw new Exception("Get failed.");
        }
    }

    public class Benchmark
    {
        private Stopwatch _stopwatch;
        private int _iterations;

        public void Run(Action<int> action, int iterations)
        {
            _iterations = iterations;
            _stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                action(i);
            }
            _stopwatch.Stop();
        }

        public TimeSpan TotalElapsed => _stopwatch.Elapsed;

        public void Report()
        {
            Console.WriteLine($"Benchmark completed {_iterations} iterations in {TotalElapsed}. {_iterations / TotalElapsed.TotalSeconds} queries per second.");
        }
    }
}
