using FileShareConsole;
using FileTransfer;
using HelloProtocol;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

            new ServerClass(new TnSProtocol()).run();

            JobScheduler scheduler = new JobScheduler();
            PipeDaemon pipeListener = new PipeDaemon();
            pipeListener.popHappened += (string path) => {

                scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
                                                                    PeersList.Instance.Peers.ElementAt(0).Id,
                                                                    path), path));
            };
            pipeListener.run();

            bf = new BackgroundForm();
        }

        public void AppExit(object sender, EventArgs e) {
            bf.Close();

            // Process completed successfully
            Environment.Exit(0);
        }

    }
}
