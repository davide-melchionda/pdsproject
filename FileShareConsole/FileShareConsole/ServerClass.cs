using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using NetProtocol;

namespace FileTransfer
{
    public class ServerClass : ExecutableThread
    {
        private Protocol protocol;
       public ServerClass(Protocol protocol)
        {
            this.protocol = protocol;
        }
        protected override void execute()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Settings.Instance.SERV_ACCEPTING_PORT);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(50);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();
                    TnSServer server = new TnSServer(handler, protocol);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(makeTask), server);


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        private void makeTask(Object o)
        {
            TnSServer server = (TnSServer)o;
            server.transfer();
        }
    }
   
}


