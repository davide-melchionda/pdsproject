using FileTransfer;
using HelloProtocol;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileShare {

    /**
     * The DataContext class contains the structures used by the NotificationWindow to show informations
     * about the lists of Peers, sending Jobs and receiving Jobs. This class makes possible to execute the
     * mapping from the low level lists (modified by the core modules of the application) and the high level
     * lists (ObservableCollection(s) directly used by the GUI).
     */
    class FileShareDataContext {

        public ObservableCollection<Peer> peers { get; set; }
        public ObservableCollection<Job> sendingJobs { get; set; }
        public ObservableCollection<Job> receivingJobs { get; set; }

        public FileShareDataContext() {
            
            PeersList.Instance.PeerInserted += (Peer inserted) => {
                App.Current.Dispatcher.Invoke((Action)delegate {
                    peers.Add(inserted);
                });
            };

            PeersList.Instance.PeerRemoved += (Peer removed) => {
                foreach (Peer p in peers)
                    if (p.Id == removed.Id) {
                        App.Current.Dispatcher.Invoke((Action)delegate {
                            peers.Remove(p);
                        });
                        break;
                    }
            };

            JobsList.Sending.JobAdded += (Job job) => {
                App.Current.Dispatcher.Invoke((Action)delegate {
                    sendingJobs.Add(job);
                });
            };

            JobsList.Sending.JobRemoved += (Job removed) => {
                foreach (Job j in sendingJobs)
                    if (j.Id == removed.Id) {
                        App.Current.Dispatcher.Invoke((Action)delegate {
                            sendingJobs.Remove(j);
                        });
                        break;
                    }
            };

            JobsList.Receiving.JobAdded += (Job job) => {
                App.Current.Dispatcher.Invoke((Action)delegate {
                    receivingJobs.Add(job);
                });
            };

            JobsList.Receiving.JobRemoved += (Job removed) => {
                foreach (Job j in receivingJobs)
                    if (j.Id == removed.Id) {
                        App.Current.Dispatcher.Invoke((Action)delegate {
                            receivingJobs.Remove(j);
                        });
                        break;
                    }
            };

            peers = new ObservableCollection<Peer>(PeersList.Instance.Peers);
            //peers.Add(Settings.Instance.LocalPeer);

            receivingJobs = new ObservableCollection<Job>(JobsList.Receiving.Jobs);

            sendingJobs = new ObservableCollection<Job>(JobsList.Sending.Jobs);

            //new Thread(() => {
            //    System.Threading.Thread.Sleep(5000);
            //    Peer local = (Settings.Instance.LocalPeer);
            //    Job j = new Job(new FileTransfer.Task("me", "you", @"C:\Users\vm-dm-win\Desktop\Desert.jpg"), @"C:\Users\vm-dm-win\Desktop\Desert.jpg");
            //    Job j2 = new Job(new FileTransfer.Task("me2", "you2", @"C:\Users\vm-dm-win\Desktop\Desert.jpg"), @"C:\Users\vm-dm-win\Desktop\Desert.jpg");
            //    PeersList.Instance.put(local);
            //    JobsList.Sending.push(j);
            //    JobsList.Sending.push(j2);
            //    System.Threading.Thread.Sleep(5000);
            //    PeersList.Instance.del(local.Id);
            //    JobsList.Sending.remove(j.Id);
            //    System.Threading.Thread.Sleep(5000);
            //    //for (int i = 0; i < 50; i++) {
            //    //    App.Current.Dispatcher.Invoke((Action)delegate {
            //    //        sendingJobs[0].SentByte += 10;
            //    //    });
            //    //    System.Threading.Thread.Sleep(1000);
            //    //}
            //    JobsList.Sending.get(j2.Id).SentByte = 20;

            //}).Start();

        }

    }
}
