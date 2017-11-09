using FileTransfer;
using HelloProtocol;
using System.Linq;
using System;

namespace FileShareConsole {
    class Program
    {
        static void Main(string[] args)
        {
            new HelloThread().run();

            new ServerClass(new TnSProtocol()).run();
           
            JobScheduler scheduler = new JobScheduler();
            PipeDaemon pipeListener = new PipeDaemon();
            pipeListener.popHappened+= (string path) => {
                scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
                                                                    PeersList.Instance.Peers.ElementAt(0).Id,
                                                                    path), path));
            };
            pipeListener.run();
            
        }
    }
}
