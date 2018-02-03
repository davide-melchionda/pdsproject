using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using NetProtocol;
using System.Net.NetworkInformation;

namespace FileTransfer {

    public class ServerClass : ExecutableThread {

        public delegate ToAccept OnRequest(ToAccept request);
        public event OnRequest RequestReceived;

        /**
         * Delegate: format of the callback to call when error on connection occours
         */
        public delegate void OnConnectionError(Job j);
        /**
         * Event on which register the callback to manage the connection error
         */
        public event OnConnectionError ConnectionError;

        /// <summary>
        /// The socket on which the server will receive connections
        /// </summary>
        Socket listener;

        /// <summary>
        /// Delegate: format of the callback to call when error on Path occours
        /// </summary>
        public delegate void OnPathError(System.IO.IOException e, String source);
        
        /**
         * Event on which register the callback to manage the Path error
         */
        public event OnPathError PathError;

        protected override void execute() {
            if (Settings.Instance.Network == null)
                return;
            NetworkInterface nic = Settings.Instance.Network.Nic;
            IPAddress addr = null;
            foreach (UnicastIPAddressInformation ip in nic.GetIPProperties().UnicastAddresses) {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    addr = ip.Address;
                }
            }
            //IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Settings.Instance.SERV_ACCEPTING_PORT);
            IPEndPoint localEndPoint = new IPEndPoint(addr, Settings.Instance.SERV_ACCEPTING_PORT);
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                listener.Bind(localEndPoint);
                listener.Listen(50);

                while (true) {
                    Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();

                    ReceptionExecutor receiver = new ReceptionExecutor(handler);
                    receiver.RequestReceived += (ToAccept request) => {
                        if (RequestReceived != null)
                            return RequestReceived(request);
                        return request;
                    };
                    receiver.ConnectionError += (Job j) => {
                        ConnectionError?.Invoke(j);
                    };
                    receiver.PathError += (System.IO.IOException e, String source) => {
                        PathError?.Invoke(e, source);
                    };
                    RegisterChild(receiver);
                    receiver.run();
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

        }

        protected override void PrepareStop() {
            listener.Close();   // Closes the socket. This whill cause an exception
        }
    }

}


