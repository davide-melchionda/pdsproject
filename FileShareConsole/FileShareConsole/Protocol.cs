﻿using System.Net.Sockets;

namespace NetProtocol {

    public abstract class Protocol {

        public abstract void enter();
        public abstract void release();

    }

    public abstract class ProtocolEndpoint {

        protected Socket socket;
        protected Protocol protocol;

        public ProtocolEndpoint(Socket socket, Protocol protocol) {
            this.socket = socket;
            this.protocol = protocol;
        }

        public abstract TransferResult transfer();

        internal void tryEnter() {
            protocol.enter();
        }

        public abstract class ServerProtocolEndpoint : ProtocolEndpoint {

            public ServerProtocolEndpoint(Socket socket, Protocol protocol) : base(socket, protocol) { }

            public abstract override TransferResult transfer();
        }

        public abstract class ClientProtocolEndpoint : ProtocolEndpoint {

            public ClientProtocolEndpoint(Socket socket, Protocol protocol) : base(socket, protocol) { }

            public abstract override TransferResult transfer();
        }

        public abstract class HandshakeResult {

        }

        public abstract class TransferResult {

        }
    }
    
}
