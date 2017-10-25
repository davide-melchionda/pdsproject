using System;
using NetProtocol;
using System.Threading;

namespace FileTransfer
{
    internal class TnSProtocol : Protocol
    {
        private static Semaphore clientsPool = new Semaphore(15, 15);
        public override void enter()
        {
            clientsPool.WaitOne();
        }
        public override void release()
        {
            clientsPool.Release();
        }
    }
}