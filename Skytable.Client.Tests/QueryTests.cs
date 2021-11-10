using System;
using System.IO;
using Xunit;
using Skytable.Client.Querying;
using System.Threading.Tasks;

namespace Skytable.Client.Tests
{
    public class QueryTests
    {
        [Fact]
        public void CreateGetQuery()
        {
            var query = new Query();
            query.Push("GET");
            query.Push("TestKey");

            using (var memoryStream = new MemoryStream())
            {
                query.WriteTo(memoryStream);
                var queryData = memoryStream.ToArray();
                var expectedQuery = "*1\n~2\n3\nGET\n7\nTestKey\n";
                var expectedQueryData = System.Text.Encoding.UTF8.GetBytes(expectedQuery);
                Assert.True(SpansEqual(expectedQueryData, queryData));
            }
        }

        [Fact]
        public async Task CreateGetQueryAsync()
        {
            var query = new Query();
            query.Push("GET");
            query.Push("TestKey");

            using (var memoryStream = new MemoryStream())
            {
                await query.WriteToAsync(memoryStream);
                var queryData = memoryStream.ToArray();
                var expectedQuery = "*1\n~2\n3\nGET\n7\nTestKey\n";
                var expectedQueryData = System.Text.Encoding.UTF8.GetBytes(expectedQuery);
                Assert.True(SpansEqual(expectedQueryData, queryData));
            }
        }

        [Fact]
        public void CreateSetQuery()
        {
            var query = new Query();
            query.Push("SET");
            query.Push("TestKey");
            query.Push("TestValue");

            using (var memoryStream = new MemoryStream())
            {
                query.WriteTo(memoryStream);
                var queryData = memoryStream.ToArray();
                
                var expectedQuery = "*1\n~3\n3\nSET\n7\nTestKey\n9\nTestValue\n";
                var expectedQueryData = System.Text.Encoding.UTF8.GetBytes(expectedQuery);
                Assert.True(SpansEqual(expectedQueryData, queryData));
            }
        }

        [Fact]
        public async Task CreateSetQueryAsync()
        {
            var query = new Query();
            query.Push("SET");
            query.Push("TestKey");
            query.Push("TestValue");

            using (var memoryStream = new MemoryStream())
            {
                await query.WriteToAsync(memoryStream);
                var queryData = memoryStream.ToArray();
                
                var expectedQuery = "*1\n~3\n3\nSET\n7\nTestKey\n9\nTestValue\n";
                var expectedQueryData = System.Text.Encoding.UTF8.GetBytes(expectedQuery);
                Assert.True(SpansEqual(expectedQueryData, queryData));
            }
        }

        [Fact]
        public void CreateDeleteQuery()
        {
            var query = new Query();
            query.Push("DEL");
            query.Push("TestKey");

            using (var memoryStream = new MemoryStream())
            {
                query.WriteTo(memoryStream);
                var queryData = memoryStream.ToArray();
                
                var expectedQuery = "*1\n~2\n3\nDEL\n7\nTestKey\n";
                var expectedQueryData = System.Text.Encoding.UTF8.GetBytes(expectedQuery);
                Assert.True(SpansEqual(expectedQueryData, queryData));
            }
        }

        [Fact]
        public async Task CreateDeleteQueryAsync()
        {
            var query = new Query();
            query.Push("DEL");
            query.Push("TestKey");

            using (var memoryStream = new MemoryStream())
            {
                await query.WriteToAsync(memoryStream);
                var queryData = memoryStream.ToArray();
                
                var expectedQuery = "*1\n~2\n3\nDEL\n7\nTestKey\n";
                var expectedQueryData = System.Text.Encoding.UTF8.GetBytes(expectedQuery);
                Assert.True(SpansEqual(expectedQueryData, queryData));
            }
        }

        [Fact]
        public void CreatePopQuery()
        {
            var query = new Query();
            query.Push("POP");
            query.Push("TestKey");

            using (var memoryStream = new MemoryStream())
            {
                query.WriteTo(memoryStream);
                var queryData = memoryStream.ToArray();
                var expectedQuery = "*1\n~2\n3\nPOP\n7\nTestKey\n";
                var expectedQueryData = System.Text.Encoding.UTF8.GetBytes(expectedQuery);
                Assert.True(SpansEqual(expectedQueryData, queryData));
            }
        }

        [Fact]
        public async Task CreatePopQueryAsync()
        {
            var query = new Query();
            query.Push("POP");
            query.Push("TestKey");

            using (var memoryStream = new MemoryStream())
            {
                await query.WriteToAsync(memoryStream);
                var queryData = memoryStream.ToArray();
                var expectedQuery = "*1\n~2\n3\nPOP\n7\nTestKey\n";
                var expectedQueryData = System.Text.Encoding.UTF8.GetBytes(expectedQuery);
                Assert.True(SpansEqual(expectedQueryData, queryData));
            }
        }

        [Fact]
        public void CreatePipeline()
        {
            var setQuery = new Query();
            setQuery.Push("SET");
            setQuery.Push("Basic");
            setQuery.Push("MyValue");

            var getQuery = new Query();
            getQuery.Push("GET");
            getQuery.Push("Basic");

            var pipeline = new Pipeline()
                .Add(setQuery)
                .Add(getQuery);

            using (var memoryStream = new MemoryStream())
            {
                pipeline.WriteTo(memoryStream);
                var queryData = memoryStream.ToArray();
                
                var expectedQuery = "*2\n~3\n3\nSET\n5\nBasic\n7\nMyValue\n~2\n3\nGET\n5\nBasic\n";
                var expectedQueryData = System.Text.Encoding.UTF8.GetBytes(expectedQuery);
                Assert.True(SpansEqual(expectedQueryData, queryData));
            }
        }

        private bool SpansEqual(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }
    }
}
