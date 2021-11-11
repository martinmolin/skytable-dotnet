using System;
using Skytable.Client;
using Skytable.Client.Querying;

namespace Skytable.Client.IntegrationTests
{
    public class SkytableFixture : IDisposable
    {
        private const string Keyspace = "Integration";
        private const string Table = "Tests";
        private readonly string Entity = $"{Keyspace}:{Table}";

        public SkytableFixture()
        {
            Db = new Connection("127.0.0.1", 2003);
            Db.Connect();

            DropTable();
            DropKeyspace();
            CreateKeyspace();
            CreateTable();
            Db.Use(Keyspace, Table);
            FlushTable();
        }

        private void DropTable()
        {
            var query = new Query();
            query.Push("DROP");
            query.Push("TABLE");
            query.Push(Entity);
            var result = Db.RunSimpleQuery(query);
        }

        private void DropKeyspace()
        {
            var query = new Query();
            query.Push("DROP");
            query.Push("KEYSPACE");
            query.Push(Keyspace);
            Db.RunSimpleQuery(query);
        }

        private void FlushTable()
        {
            var query = new Query();
            query.Push("FLUSHDB");
            Db.RunSimpleQuery(query);
        }

        private void CreateKeyspace()
        {
            var query = new Query();
            query.Push("CREATE");
            query.Push("KEYSPACE");
            query.Push(Keyspace);
            Db.RunSimpleQuery(query);
        }

        private void CreateTable()
        {
            var query = new Query();
            query.Push("CREATE");
            query.Push("TABLE");
            query.Push(Entity);
            query.Push("keymap(str,str)");
            Db.RunSimpleQuery(query);
        }

        public void Dispose()
        {
            // TODO: Cleanup.
        }

        public Connection Db { get; private set; }
    }
}