using System;
using System.Threading;

namespace FileTransfer
{
    public class ClientClass : ExecutableThread
    {
        private Task fileToSend;
        public static Semaphore activeClientsPool = new Semaphore(15, 15);
        public ClientClass(Task toBePerformed)
        {
            this.fileToSend = toBePerformed;
        }
  

        protected override void execute()
        {
            
            //invio al server della request che incapsula il task

            /*il client ottiene risposta affermativa dal server --> */
            activeClientsPool.WaitOne();
            //il client fa altre cose
            activeClientsPool.Release();
        }

    }
}