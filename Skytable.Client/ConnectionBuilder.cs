namespace Skytable.Client
{
    public class ConnectionBuilder
    {
        private string _host = "localhost";
        private ushort _port = 2003;
        private bool _useTls = false;

        public ConnectionBuilder()
        {

        }

        public ConnectionBuilder Host(string host)
        {
            _host = host;
            return this;
        }

        public ConnectionBuilder Port(ushort port)
        {
            _port = port;
            return this;
        }

        public ConnectionBuilder UseTls()
        {
            _useTls = true;
            return this;
        }

        public Connection Build()
        {
            var connection = new Connection(_host, _port, _useTls);
            return connection;
        }
    }
}