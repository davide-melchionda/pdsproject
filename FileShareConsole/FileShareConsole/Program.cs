using FileTransfer;
using HelloProtocol;
using System.Linq;
using System.Threading;

namespace FileShareConsole {
    class Program
    {
        static void Main(string[] args)
        {
            ServerClass server = new ServerClass(new TnSProtocol());
            server.run();
            new HelloThread().run();

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

          
            //string to = @"C:\Users\vm-dm-win\Desktop\tmpzipped" + DateTime.Now.Millisecond + @".zip";

            //ServerClass server = new ServerClass(new DummyProtocol());
            //server.run();
            PipeDaemon pipeListener = new PipeDaemon();
            pipeListener.run();

            JobScheduler scheduler = new JobScheduler();
            
            //string s = @"C:\Users\vm-dm-win\Desktop\tmp";
            //string sz = @"C:\Users\vm-dm-win\Desktop\tmpz";

            //while (PeersList.Instance.Peers.Count <= 0)
            //    System.Threading.Thread.Sleep(5000);

            // 1 FILE
            //scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
            //                                                        PeersList.Instance.Peers.ElementAt(0).Id,
            //                                                        s + "1.txt"), s + "1.txt"));

            // 1 IMG
            //scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
            //                                                        PeersList.Instance.Peers.ElementAt(0).Id,
            //                                                        s + "img.jpg"), s + "img.jpg"));

            // 1 DIRECTORY
            //scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
            //                                                        PeersList.Instance.Peers.ElementAt(0).Id,
            //                                                        s), s));

            // 20 FILES
            //for (int i = 0; i < 20; i++) {
            //    scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
            //                                                        PeersList.Instance.Peers.ElementAt(0).Id,
            //                                                        s + @"/" + (i + 1) + ".txt"), s + @"/" + (i + 1) + ".txt"));
            //    //System.Threading.Thread.Sleep(500);
            //}

            // 1 HEAVY FILE
            //scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
            //                                                        PeersList.Instance.Peers.ElementAt(0).Id,
            //                                                        sz + @"/" + 1 + ".zip"), sz + @"/" + 1 + ".zip"));

            // 20 HEAVY FILES
            //for (int i = 0; i < 20; i++) {
            //    scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
            //                                                        PeersList.Instance.Peers.ElementAt(0).Id,
            //                                                        sz + @"/" + (i + 1) + ".zip"), sz + @"/" + (i + 1) + ".zip"));
            //    //System.Threading.Thread.Sleep(500);
            //}

        }
    }
}
