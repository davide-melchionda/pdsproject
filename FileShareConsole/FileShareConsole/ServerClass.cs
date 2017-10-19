using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace FileTransfer
{
    public class ServerClass : ExecutableThread
    {
       
        protected override void execute()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Settings.Instance.TCP_RECEIVING_PORT);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(0);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();
                    ExecutableThread clientHandler = new TnSServer(handler);
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


