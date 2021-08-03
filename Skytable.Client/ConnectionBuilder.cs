using System.Security;

namespace Skytable.Client
{
    /// <summary>
    /// A builder to create a <see cref="Connection"/>.
    /// </summary>
    public class ConnectionBuilder
    {
        private string _host = "127.0.0.1";
        private ushort _port = 2003;
        private string _certPath = string.Empty;

        /// <summary>
        /// Create a builder to create a <see cref="Connection"/>.
        /// The connection will by default connect to 127.0.0.1:2003 without TLS.
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
        public ConnectionBuilder UseTls(string certPath)
        {
            _certPath = certPath;
            return this;
        }

        /// <summary>Creates the <see cref="Connection"/> and connects to the Skytable server.</summary>
        public Connection Build()
        {
            if (string.IsNullOrEmpty(_certPath))
                return new Connection(_host, _port);
            return new Connection(_host, _port, _certPath);
        }
    }
}