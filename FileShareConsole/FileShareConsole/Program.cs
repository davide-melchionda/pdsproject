using FileTransfer;
using HelloProtocol;
using NetProtocol;
using NetworkTransmission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileShareConsole
{
    class Program
    {
        static void Main(string[] args) {
            new ServerClass().run();
            //new HelloThread().run();
            System.Threading.Thread.Sleep(1000);

            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            FileTransfer.Task take = new FileTransfer.Task();
            FileInfo f= new FileInfo(FileInfo.TypeEnum.directory);


            Protocol proto;
            take.informations = f;
            new TnSClient(iPAddress,proto,  take).run();
            
        }
    }
}
