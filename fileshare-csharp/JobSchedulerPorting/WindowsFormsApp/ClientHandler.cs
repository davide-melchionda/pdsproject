using System;
using System.Net.Sockets;
using System.Net;
/// <summary>
/// Summary description for Class1
/// </summary>
/// 
namespace FileTransfer
{
    public class ClientHandler : ExecutableThread
    {
        public Socket handler;
        public ClientHandler(Socket handler)
        {
            this.handler = handler;
        }

        protected override void execute()
        {
            //metodo che si occupa di attuare il protocollo di comunicazione nel trasferimento file, a lui il compito di chiudere il socket

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

        }
    }

}
