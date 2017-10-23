using System.Net.Sockets;

namespace NetProtocol
{
    /**
        * Abstract class for generics clients and servers
        */

    public abstract class ProtocolEndpoint
    {

        private Socket socket;

        public ProtocolEndpoint(Socket socket)
        {
            this.socket = socket;
        }

        public abstract TransferResult transfer();

        public abstract class ServerProtocolEndpoint : ProtocolEndpoint
        {

            public ServerProtocolEndpoint(Socket socket) : base(socket) { }

            public abstract override TransferResult transfer();
        }

        public abstract class ClientProtocolEndpoint : ProtocolEndpoint
        {

            public ClientProtocolEndpoint(Socket socket) : base(socket) { }

            public abstract override TransferResult transfer();
        }

        
        public abstract class TransferResult
        {

        }
    }

}
