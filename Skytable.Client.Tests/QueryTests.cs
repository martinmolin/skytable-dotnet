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

        private bool SpansEqual(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }
    }
}
