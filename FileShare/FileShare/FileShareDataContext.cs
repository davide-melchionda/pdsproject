
using FileTransfer;
using HelloProtocol;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FileShare {

    /**
     * The DataContext class contains the structures used by the NotificationWindow to show informations
     * about the lists of Peers, sending Jobs and receiving Jobs. This class makes possible to execute the
     * mapping from the low level lists (modified by the core modules of the application) and the high level
     * lists (ObservableCollection(s) directly used by the GUI).
     */
    class FileShareDataContext {

        /**
         * The instance of the list of jobs for outgoing transfers
         */
        private static FileShareDataContext instance;
        /**
         * instance property.
         * Through the get method it's possble to get a reference to the
         * unique instance for outgoing transfers of this cass.
         */
        public static FileShareDataContext Instance {
            get {
                if (instance == null) {
                    instance = new FileShareDataContext();
                }
                return instance;
            }
        }

        public ObservableCollection<ListedPeer> peers { get; set; }
        public ObservableCollection<ListedJob> sendingJobs { get; set; }
        public ObservableCollection<ListedJob> receivingJobs { get; set; }
        public ListedPeer Me { get; set; }
        public Resources Resources = Settings.Instance.Resources;

        public bool NoPeers {
            get { return peers.Count == 0; }
        }
        public bool NothingToSend {
            get { return sendingJobs.Count == 0; }
        }
        public bool NothingToReceive {
            get { return receivingJobs.Count == 0; }
        }

        protected FileShareDataContext() {

            Me = new ListedPeer(Settings.Instance.LocalPeer);

            PeersList.Instance.PeerInserted += (Peer inserted) => {
                App.Current.Dispatcher.Invoke((Action)delegate {
                    peers.Add(new ListedPeer(inserted));
                });
            };

            PeersList.Instance.PeerRemoved += (Peer removed) => {
                foreach (ListedPeer p in peers)
                    if (p.Peer.Id == removed.Id) {
                        App.Current.Dispatcher.Invoke((Action)delegate {
                            peers.Remove(p);
                        });
                        break;
                    }
            };

            JobsList.Sending.JobAdded += (Job job) => {
                ListedJob listedItem = new ListedJob(job);
                App.Current.Dispatcher.Invoke((Action)delegate {
                    sendingJobs.Add(listedItem);
                });
                ConfigureState(listedItem); // Adapt view to the currest state of the job
                job.PropertyChanged += (object sender, PropertyChangedEventArgs args) => {
                    ConfigureState(listedItem);
                };
            };

            JobsList.Sending.JobRemoved += (Job removedJob) => {
                foreach (ListedJob listItem in sendingJobs)
                    if (listItem.Job.Id == removedJob.Id) {
                        ConfigureState(listItem);
                        System.Threading.Thread.Sleep(5000);
                        App.Current.Dispatcher.Invoke((Action)delegate {
                            sendingJobs.Remove(listItem);
                        });
                        break;
                    }
            };

            JobsList.Receiving.JobAdded += (Job job) => {
                ListedJob listedItem = new ListedJob(job);
                App.Current.Dispatcher.Invoke((Action)delegate {
                    receivingJobs.Add(listedItem);
                });
                ConfigureState(listedItem); // Adapt view to the currest state of the job
                job.PropertyChanged += (object sender, PropertyChangedEventArgs args) => {
                    ConfigureState(listedItem);
                };
            };

            JobsList.Receiving.JobRemoved += (Job removedJob) => {
                foreach (ListedJob listItem in receivingJobs)
                    if (listItem.Job.Id == removedJob.Id) {
                        ConfigureState(listItem);
                        System.Threading.Thread.Sleep(5000);
                        App.Current.Dispatcher.Invoke((Action)delegate {
                            receivingJobs.Remove(listItem);
                        });
                        break;
                    }
            };

            peers = new ObservableCollection<ListedPeer>();
            foreach (Peer p in PeersList.Instance.Peers)
                peers.Add(new ListedPeer(p));

            receivingJobs = new ObservableCollection<ListedJob>();
            foreach (Job j in JobsList.Receiving.Jobs)
                receivingJobs.Add(new ListedJob(j));

            sendingJobs = new ObservableCollection<ListedJob>();
            foreach (Job j in JobsList.Sending.Jobs)
                sendingJobs.Add(new ListedJob(j));

        }

        private void ConfigureState(ListedJob listItem) {
            // If no message was set, it was not me to configure this 
            // listItem to be removed. This means that I have to configure
            // all the fields to a generic "Error" or "Completed" sate.
            // I've no information to say more than this
            if (/*listItem.Message == null || listItem.Message == "In completamento"*/true) {
                //if (listItem.Job.Percentage != 100) {
                switch (listItem.Job.Status) {
                    case Job.JobStatus.Active:
                        listItem.Preparing = false;
                        listItem.Completing = false;
                        listItem.Completed = false;
                        listItem.Error = false;
                        listItem.Message = null;
                        break;
                    case Job.JobStatus.WaitingForRemoteAcceptance:
                        listItem.Preparing = true;
                        listItem.Completing = false;
                        listItem.Completed = false;
                        listItem.Message = listItem.Job.Task.ReceiverName + " sta decidendo se accettare il trasferimento";
                        break;
                    case Job.JobStatus.ConnectionError:
                        listItem.Preparing = false;
                        listItem.Completing = false;
                        listItem.Completed = true;
                        listItem.Error = true;
                        listItem.Message = "Trasferimento interrotto."; /*Impossibile contattare ";*/
                        //if (listItem.Job is SendingJob)
                        //    listItem.Message += "il destinatario";
                        //else
                        //    listItem.Message += "il mittente";
                        break;
                    case Job.JobStatus.NotAcceptedByRemote:
                        listItem.Preparing = false;
                        listItem.Completing = false;
                        listItem.Completed = true;
                        listItem.Error = false;
                        listItem.Message = listItem.Job.Task.ReceiverName + " ha rifiutato il trasferimento";
                        break;
                    case Job.JobStatus.StoppedByRemote:
                        listItem.Preparing = false;
                        listItem.Completing = false;
                        listItem.Completed = true;
                        listItem.Error = true;
                        listItem.Message = "Trasferimento interrotto dalla controparte";
                        break;
                    case Job.JobStatus.Completed:
                        listItem.Preparing = false;
                        listItem.Completing = false;
                        listItem.Completed = true;
                        listItem.Error = false;
                        listItem.Message = "Completato";
                        break;
                    case Job.JobStatus.Preparing:
                        listItem.Preparing = true;
                        listItem.Completing = false;
                        listItem.Completed = false;
                        listItem.Message = "In preparazione...";
                        break;
                    case Job.JobStatus.Completing:
                        listItem.Preparing = false;
                        listItem.Completing = true;
                        listItem.Completed = true;
                        listItem.Message = "In completamento...";
                        break;
                    default:
                        listItem.Message = "In cancellazione...";
                        break;
                }
                //} else {
                //    listItem.Completing = false;
                //    listItem.Completed = true;
                //    listItem.Error = false;
                //    listItem.Message = "Completato";
                //}
            }
            // Otherwise I've configured this listItem to be removed
            // somewhere else.
        }

        internal void DeactivateJob(ListedJob item) {
            item.Stopped = true;
            item.Job.Status = Job.JobStatus.StoppedByLocal; // It was me to stop this job
            item.Completed = true;
            item.Error = false;
            item.Message = "In cancellazione...";
        }

        public async void manageProgressBar(ProgressBar prog) {
            int count = 3;
            ListedJob item = prog.DataContext as ListedJob;
            App.Current.Dispatcher.Invoke((Action)delegate {
                prog.Value = item.Job.Percentage;
            });
            await System.Threading.Tasks.Task.Run(() => {
                while (true) {
                    // Responsive app: first thing to do is to show to the user something
                    // coherent with the status of the job.
                    App.Current.Dispatcher.Invoke((Action)delegate {
                        prog.Value = item.Job.Percentage;
                    });
                    if (count == 3) {
                        item.UpdateTimeLeft();
                        count = 0;
                    } else
                        count++;

                    // If we have done
                    if (item.Job.Percentage == 100 ||
                        (item.Job.Status != Job.JobStatus.Active &&
                        item.Job.Status != Job.JobStatus.WaitingForRemoteAcceptance &&
                        item.Job.Status != Job.JobStatus.Preparing))
                        break;

                    System.Threading.Thread.Sleep(300);
                }
            });
        }
    }
}
