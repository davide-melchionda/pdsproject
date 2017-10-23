using FileTransfer;
using HelloProtocol;
using NetProtocol;
using NetworkTransmission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileShareConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Protocol proto = new Protocol();

            new ServerClass(proto).run();
            ////new HelloThread().run();
            //System.Threading.Thread.Sleep(1000);



        }
    }

}
