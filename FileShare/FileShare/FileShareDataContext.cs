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
        public ObservableCollection<ListedJob> sendingJobs { get; set; }
        public ObservableCollection<ListedJob> receivingJobs { get; set; }

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
                    sendingJobs.Add(new ListedJob(job));
                });
            };

            JobsList.Sending.JobRemoved += (Job removedJob) => {
                foreach (ListedJob listItem in sendingJobs)
                    if (listItem.Job.Id == removedJob.Id) {
                        if (removedJob.Percentage != 100) {
                            listItem.Completed = true;
                            listItem.Error = true;
                            listItem.Message = "Errore";
                        } else {
                            listItem.Completed = true;
                            listItem.Error = false;
                            listItem.Message = "Completato";
                        }
                        System.Threading.Thread.Sleep(5000);
                        App.Current.Dispatcher.Invoke((Action)delegate {
                            sendingJobs.Remove(listItem);
                        });
                        break;
                    }
            };

            JobsList.Receiving.JobAdded += (Job job) => {
                App.Current.Dispatcher.Invoke((Action)delegate {
                    receivingJobs.Add(new ListedJob(job));
                });
            };

            JobsList.Receiving.JobRemoved += (Job removedJob) => {
                foreach (ListedJob listItem in receivingJobs)
                    if (listItem.Job.Id == removedJob.Id) {
                        if (removedJob.Percentage != 100) {
                            listItem.Completed = true;
                            listItem.Error = true;
                            listItem.Message = "Errore";
                        } else {
                            listItem.Completed = true;
                            listItem.Error = false;
                            listItem.Message = "Completato";
                        }
                        System.Threading.Thread.Sleep(5000);
                        App.Current.Dispatcher.Invoke((Action)delegate {
                            receivingJobs.Remove(listItem);
                        });
                        break;
                    }
            };

            peers = new ObservableCollection<Peer>(PeersList.Instance.Peers);

            receivingJobs = new ObservableCollection<ListedJob>();
            foreach (Job j in JobsList.Receiving.Jobs)
                receivingJobs.Add(new ListedJob(j));

            sendingJobs = new ObservableCollection<ListedJob>();
            foreach (Job j in JobsList.Sending.Jobs)
                sendingJobs.Add(new ListedJob(j));

        }

    }
}
