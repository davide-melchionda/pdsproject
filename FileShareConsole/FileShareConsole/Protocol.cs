using System.Net.Sockets;

namespace Protocol {

    public abstract class ProtocolEndpoint {

        private Socket socket;

        public ProtocolEndpoint(Socket socket) {
            this.socket = socket;
        }

        public abstract HandshakeResult handshake();
        public abstract TransferResult transfer();

        public abstract class ServerProtocolEndpoint : ProtocolEndpoint {

            public ServerProtocolEndpoint(Socket socket) : base(socket) { }

            public abstract override HandshakeResult handshake();
            public abstract override TransferResult transfer();
        }

        public abstract class ClientProtocolEndpoint : ProtocolEndpoint {

            public ClientProtocolEndpoint(Socket socket) : base(socket) { }

            public abstract override HandshakeResult handshake();
            public abstract override TransferResult transfer();
        }

        public abstract class HandshakeResult {

        }

        public abstract class TransferResult {

        }
    }
    
}
