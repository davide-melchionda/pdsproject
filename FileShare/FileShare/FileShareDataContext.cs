﻿
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
                App.Current.Dispatcher.Invoke((Action)delegate {
                    sendingJobs.Add(new ListedJob(job));
                });
            };

            JobsList.Sending.JobRemoved += (Job removedJob) => {
                foreach (ListedJob listItem in sendingJobs)
                    if (listItem.Job.Id == removedJob.Id) {
                        ConfigurePreremovingState(listItem);
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
                        ConfigurePreremovingState(listItem);
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

        private void ConfigurePreremovingState(ListedJob listItem) {
            // If no message was set, it was not me to configure this 
            // listItem to be removed. This means that I have to configure
            // all the fields to a generic "Error" or "Completed" sate.
            // I've no information to say more than this
            if (listItem.Message == null || listItem.Message == "In completamento") {
                if (listItem.Job.Percentage != 100) {
                    listItem.Completing = false;
                    listItem.Completed = true;
                    listItem.Error = true;
                    listItem.Message = "Trasferimento non completato.";
                } else {
                    listItem.Completing = false;
                    listItem.Completed = true;
                    listItem.Error = false;
                    listItem.Message = "Completato";
                }
            }
            // Otherwise I've configured this listItem to be removed
            // somewhere else.
        }

        internal void DeactivateJob(ListedJob item) {
            item.Stopped = true;
            item.Job.Active = false;
            item.Completed = true;
            item.Error = false;
            item.Message = "In cancellazione...";
        }

        public async void manageProgressBar(ProgressBar prog) {
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
                    item.UpdateTimeLeft();

                    // If we have done
                    if (item.Job.Percentage == 100 || !item.Job.Active)
                        break;

                    System.Threading.Thread.Sleep(300);
                }
            });
        }
    }
}
