using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using NetProtocol;

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

        /**
   * Delegate: format of the callback to call when error on Path occours
   */
        public delegate void OnPathError();
        /**
         * Event on which register the callback to manage the Path error
         */
        public event OnPathError PathError;

        protected override void execute() {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Settings.Instance.SERV_ACCEPTING_PORT);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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
                    receiver.PathError += () => {
                        PathError?.Invoke();
                    };
                    receiver.run();
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

        }
    }

}


