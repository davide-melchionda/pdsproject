using FileTransfer;
using HelloProtocol;
using NetworkTransmission;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static StorageModule;

namespace FileShareConsole
{
    class Program
    {
        static void Main(string[] args) {
            ServerClass server = new ServerClass(new TnSProtocol());
            server.run();
            ////new HelloThread().run();

            ////new JobScheduler().run();

            //JobZipStorageModule storage = new JobZipStorageModule();

            //string s = @"C:\Users\franc\Desktop\testo.txt";
            ////string to = @"C:\Users\franc\Desktop\tempzipped" + DateTime.Now.Millisecond+@".zip";

            ////ZipFile.CreateFromDirectory(s, to, 0, true);

            //FileTransfer.Task myTask = new FileTransfer.Task("me", "you", s);
            //Job j = new Job(myTask, s);
            //FileIterator i = storage.prepareJob(j);
            //Socket sender = new Socket(AddressFamily.InterNetwork,
            //                SocketType.Stream, ProtocolType.Tcp);
            //IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Settings.Instance.TCP_RECEIVING_PORT);
            //sender.Connect(remoteEP);
            //TnSClient client = new TnSClient(sender, new DummyProtocol(),i, myTask);
            //client.transfer();
            ////i.close();

            string s = @"C:\Users\franc\Desktop\temp";
            string t = @"C:\Users\franc\Desktop\zio.pdf";

            string sr = @"C:\Users\franc\Desktop\zio.jpg";
            //string to = @"C:\Users\vm-dm-win\Desktop\tmpzipped" + DateTime.Now.Millisecond + @".zip";

            //ServerClass server = new ServerClass(new DummyProtocol());
            //server.run();
            PeersList.Instance.put(new Peer("you", "you", "127.0.0.1"));
            PipeDaemon pipeListener = new PipeDaemon();
            pipeListener.run();
            JobScheduler scheduler = new JobScheduler();
            //scheduler.scheduleJob(new Job(new FileTransfer.Task("me", "you", sr), sr));
            Thread.Sleep(1000);
            //scheduler.scheduleJob(new Job(new FileTransfer.Task("me", "you", s), s));

            























        }
    }
}
