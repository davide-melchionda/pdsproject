using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace FileTransfer
{
    public class ServerClass : ExecutableThread
    {
        private Thread server;
        private int port = 25000;
   
        ~ServerClass() { server.Join(); }

        protected override void execute()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(0);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();
                    ExecutableThread clientHandler = new ClientHandler(handler);
                    clientHandler.run();
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

      
      
    }
}


