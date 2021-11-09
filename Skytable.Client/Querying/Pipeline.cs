using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Skytable.Client.Querying
{
    /// <summary>
    /// A pipeline is a way of queing up multiple queries, sending them to the server at once instead of sending them individually,
    /// avoiding round-trip-times while also simplifying usage in several places. Responses are returned in the order they are sent.
    /// </summary>
    public class Pipeline : IQueryWriter
    {
        private List<byte> _chain;
        private int _queryCount;
        
        /// <summary>Creates an empty pipeline.</summary>
        public Pipeline()
        {
            _chain = new List<byte>();
            _queryCount = 0;
        }

        /// <summary>Append a query (builder pattern).</summary>
        public Pipeline Add(Query query)
        {
            _queryCount++;
            query.WriteTo(_chain);
            return this;
        }

        /// <summary>Append a query.</summary>
        public void Push(Query query)
        {
            _queryCount++;
            query.WriteTo(_chain);
        }

        /// <summary>Writes the pipeline to the specified stream.</summary>
        public void WriteTo(Stream stream)
        {
            var header = System.Text.Encoding.UTF8.GetBytes($"*{_queryCount}\n");
            stream.Write(header);
            stream.Write(_chain.ToArray(), 0, _chain.Count);
        }

        /// <summary>Writes the pipeline to the specified stream asynchronously.</summary>
        public async Task WriteToAsync(Stream stream)
        {
            var header = System.Text.Encoding.UTF8.GetBytes("*");
            var newline = System.Text.Encoding.UTF8.GetBytes("\n");
            await stream.WriteAsync(header);
            await stream.WriteAsync(BitConverter.GetBytes(_queryCount));
            await stream.WriteAsync(newline);
            await stream.WriteAsync(_chain.ToArray(), 0, _chain.Count);
        }

        /// <summary>Returns the number of queries in the pipeline.</summary>
        public int Count => _queryCount;
    }
}
