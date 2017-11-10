using FileShareConsole;
using FileTransfer;
using HelloProtocol;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public ObservableCollection<Peer> peers { get; set; }
        public ObservableCollection<Job> sendingJobs { get; set; }
        public ObservableCollection<Job> receivingJobs { get; set; }

        public MainWindow() {

            InitializeComponent();

            BackgroundForm bf = new BackgroundForm(this);

            new HelloThread().run();

            new ServerClass(new TnSProtocol()).run();
             
            JobScheduler scheduler = new JobScheduler();
            PipeDaemon pipeListener = new PipeDaemon();
            pipeListener.popHappened += (string path) => {
                
                scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
                                                                    PeersList.Instance.Peers.ElementAt(0).Id,
                                                                    path), path));
            };
            pipeListener.run();

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

            peers = new ObservableCollection<Peer>(PeersList.Instance.Peers);
            //peers.Add(Settings.Instance.LocalPeer);

            receivingJobs = new ObservableCollection<Job>(JobsList.Receiving.Jobs);

            sendingJobs = new ObservableCollection<Job>(JobsList.Sending.Jobs);

            DataContext = this;

            new Thread(() => {
                System.Threading.Thread.Sleep(5000);
                Peer local = (Settings.Instance.LocalPeer);
                Job j = new Job(new FileTransfer.Task("me", "you", @"C:\Users\vm-dm-win\Desktop\Desert.jpg"), @"C:\Users\vm-dm-win\Desktop\Desert.jpg");
                Job j2 = new Job(new FileTransfer.Task("me2", "you2", @"C:\Users\vm-dm-win\Desktop\Desert.jpg"), @"C:\Users\vm-dm-win\Desktop\Desert.jpg");
                PeersList.Instance.put(local);
                JobsList.Sending.push(j);
                JobsList.Sending.push(j2);
                System.Threading.Thread.Sleep(5000);
                PeersList.Instance.del(local.Id);
                JobsList.Sending.remove(j.Id);
                System.Threading.Thread.Sleep(5000);
                //for (int i = 0; i < 50; i++) {
                //    App.Current.Dispatcher.Invoke((Action)delegate {
                //        sendingJobs[0].SentByte += 10;
                //    });
                //    System.Threading.Thread.Sleep(1000);
                //}
                JobsList.Sending.get(j2.Id).SentByte = 20;

            }).Start();

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e) {

        }

        private void ListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e) {

        }
    }
}
