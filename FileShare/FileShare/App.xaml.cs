
using FileShareConsole;
using FileTransfer;
using HelloProtocol;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application {

        BackgroundForm bf;
        NotificationWindow nw;
        void AppStartup(object sender, StartupEventArgs e) {

            new HelloThread().run();

           ServerClass receiver= new ServerClass(new TnSProtocol());
            receiver.RequestReceived += RequestMessageBox.show;
            receiver.run();

            PipeDaemon pipeListener = new PipeDaemon();

            pipeListener.popHappened += (string path) => {
                Application.Current.Dispatcher.Invoke((Action)delegate {

                    SelectionWindow sw = new SelectionWindow(path);
                    sw.page.OnselectHappened += schedule;
                    sw.Closed += Sw_Closed;
                    sw.Show();
                    


                });
              
            };
            pipeListener.run();

            bf = new BackgroundForm();
        }

        
        private void Sw_Closed(object sender, EventArgs e)
        {
            nw.Show();
            nw.Page.mainTabControl.SelectedIndex = 1;
                }

        public void AppExit(object sender, EventArgs e) {
            bf.Close();

            // Process completed successfully
            Environment.Exit(0);
        }
        private void schedule(List<Peer> selected, string path)
        {
            JobScheduler scheduler = new JobScheduler();

            
            for (int i = 0;  i < selected.Count; i++)
            {
                scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
                                                                        PeersList.Instance.Peers.ElementAt(i).Id,
                                                                        path), path));
            }
        }

    }
}
