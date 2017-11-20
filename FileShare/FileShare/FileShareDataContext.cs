
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
                    //ListedJob listedJob = new ListedJob(job);
                    //listedJob.Job.PropertyChanged += new PercentageChangedEventHandler(listedJob).UpdatePercentage;
                    //sendingJobs.Add(listedJob);
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
                    //ListedJob listedJob = new ListedJob(job);
                    //listedJob.Job.PropertyChanged += new PercentageChangedEventHandler(listedJob).UpdatePercentage;
                    //receivingJobs.Add(listedJob);
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

        public async void manageProgressBar(ProgressBar prog) {
            ListedJob item = prog.DataContext as ListedJob;
            await System.Threading.Tasks.Task.Run(() => {
                while (true) {
                    App.Current.Dispatcher.Invoke((Action)delegate {
                        prog.Value = item.Job.Percentage;
                    });
                    if (item.Job.Percentage == 100)
                        break;
                    System.Threading.Thread.Sleep(300);
                }
            });
        }

        ///// <summary>
        ///// A PropertyChangedEventHandler to manage the change of percentage in a Job. This operation must
        ///// be carefully handled, in order to avoid heavy work charge on the ui thread.
        ///// Anyway, the ui thrad can choose if it wants to listen on the changes on the property Percentage
        ///// of the job (this canges appen quite often) or on the property Parcentage of listedJob (this 
        ///// changes are quite rare, because they are guaranteed to appen only after a minum time - e.g. 300 ms -
        ///// from the previous one and only if a real changement in Percentage value appened).
        ///// Listening on changes on Percentage property of ListedJob is less heavy for a ui thread.
        ///// </summary>
        //private class PercentageChangedEventHandler {
        //    /// <summary>
        //    /// The listed job on which this PercentageChangedEventHandler will work
        //    /// </summary>
        //    public ListedJob ListedJob { get; set; }

        //    /// <summary>
        //    /// PropertyChangedEventHandler retrieved to manage the change of percentage
        //    /// on the job associated to ListedJob.
        //    /// </summary>
        //    public PropertyChangedEventHandler UpdatePercentage {
        //        get {
        //            return updatePercentage;
        //        }
        //    }

        //    /// <summary>
        //    /// Constructor
        //    /// </summary>
        //    /// <param name="listedJob"></param>
        //    public PercentageChangedEventHandler(ListedJob listedJob) {
        //        this.ListedJob = listedJob;
        //    }

        //    /// <summary>
        //    /// The first time 
        //    /// </summary>
        //    /// <param name="sender"></param>
        //    /// <param name="args"></param>
        //    public async void updatePercentage(object sender, PropertyChangedEventArgs args) {
        //        if (args.PropertyName == "Percentage") {

        //            ListedJob.Job.PropertyChanged -= updatePercentage;

        //            ListedJob.Percentage = ListedJob.Job.Percentage;    // retrieves Percentage value

        //            await System.Threading.Tasks.Task.Run(() => {
        //                while (ListedJob.Percentage != 100) {
        //                    //if (ListedJob.Job.Percentage != ListedJob.Percentage)
        //                        ListedJob.Percentage = ListedJob.Job.Percentage;
        //                    System.Threading.Thread.Sleep(1000);
        //                }
        //            });
        //        }
        //    }

        //}
    }
}
