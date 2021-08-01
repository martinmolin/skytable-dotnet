namespace Skytable.Client
{
    /// <summary>
    /// A builder to create a <see cref="Connection"/>.
    /// </summary>
    public class ConnectionBuilder
    {
        private string _host = "localhost";
        private ushort _port = 2003;
        private bool _useTls = false;

        /// <summary>
        /// Create a builder to create a <see cref="Connection"/>.
        /// The connection will by default connect to localhost:2003 without TLS.
        /// </summary>
        public ConnectionBuilder()
        {

        }

        /// <summary>Sets the Host that the connection should be made to.</summary>
        /// <param name="host">The host that the connection should be made to.</param>
        public ConnectionBuilder Host(string host)
        {
            _host = host;
            return this;
        }

        /// <summary>Sets the Port that the connection should be made to.</summary>
        /// <param name="port">The port that the connection should be made to.</param>
        public ConnectionBuilder Port(ushort port)
        {
            _port = port;
            return this;
        }

        /// <summary>Enables TLS on the connection.</summary>
        public ConnectionBuilder UseTls()
        {
            _useTls = true;
            return this;
        }

        /// <summary>Creates the <see cref="Connection"/> and connects to the Skytable server.</summary>
        public Connection Build()
        {
            var connection = new Connection(_host, _port, _useTls);
            return connection;
        }
    }
}