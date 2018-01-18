using FileShareConsole;
using FileTransfer;
using HelloProtocol;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Threading;
using System.Windows;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application {
        /// <summary>
        /// The instance of the HelloThread, that implements ExecutableThread
        /// </summary>
        HelloThread hellothread;

        /// <summary>
        /// The instance of the ServerClass, that implements ExecutableThread,
        /// which will be used to mange incoming request of transfers
        /// </summary>
        ServerClass receiver;

        /// <summary>
        /// This list will contain all the ExecutableThreads on which the outgoing
        /// Jobs will be scheduled.
        /// </summary>
        List<ExecutableThread> outgoingJobThreads = new List<ExecutableThread>();

        /// <summary>
        /// The PipeDaemon ExecutableThread tath allows the communication on the pipe
        /// </summary>
        PipeDaemon pipeListener;

        ///<summary>
        /// A form which will never be shown and which isresponsible of managing the
        /// logic behind the notify icon shwon in the system tray. */
        /// </summary> 
        BackgroundForm bf;

        /// <summary>
        /// A mutex to manage the case in which more than one instance of this application was started
        /// </summary>
        static Mutex mutex;// = new Mutex(true, "_FileShareApp_Mutex");

        //Deny the user the possibility to open two or more instances of the application
        void AppStartup(object sender, StartupEventArgs e) {

            // Only a single instance of this process must e executed
            // if the length is grather than 1 we must quit
            //if (Process.GetProcessesByName("FileShare").Length > 1) {
            //    Environment.Exit(0);
            //}
            bool created;   // will be true if the mutext will be created and not retrieved
            mutex = new Mutex(true, "_FileShareApp_Mutex", out created);
            if (!created) { // If the mutex was not created, we are not the first thread trying to execute the program
                //if (!mutex.WaitOne(TimeSpan.Zero, true)) {
                mutex.Close();    // we don't want the system mantins the mutex opened for us
                MessageBox.Show("File shar è già in esecuzione. Puoi accedervi dalla task bar.", "Avvio di File Share", MessageBoxButton.OK, MessageBoxImage.Information);
                Environment.Exit(0);
            }

            SettingsPersistence.readSettings();
            if (Settings.Instance.DontShowSetup == false) {
                ProfileSetupWindow pw = new ProfileSetupWindow();
                pw.ShowDialog();
            }

            // Start the thread responsible of the neighbor discovery process
            hellothread = new HelloThread();
            hellothread.OnProfilePicUpdate += Hellothread_OnProfilePicUpdate;
            hellothread.run();

            // Start the thread responsible of receiving request of transferring files
            receiver = new ServerClass();
            receiver.RequestReceived += (ToAccept request) => {

                // On the gui thread
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    //mostra la finestra e prende in uscita path e response
                    ReceiveWindow rw = new ReceiveWindow(request);
                    //rw.ShowDialog();
                    rw.Show();
                    //rw.Activate();
                });
                return request;
            };

            receiver.ConnectionError += (Job j) => {
                if (j == null || j.Status == Job.JobStatus.ConnectionError)
                    bf.NotifyError(BackgroundForm.ErrorNotificationType.Receiving);
            };
            receiver.PathError += () => {
                bf.NotifyError(BackgroundForm.ErrorNotificationType.Path);
            };

            receiver.run();

            /* Create, configura and start the thread responsible of managing the communication on a pipe 
             * with the application which inform this process of the path of the file the user wants to send. */
            pipeListener = new PipeDaemon();
            /* Register on the pipeListener a callback to execute when the user wants to send
             * a new file. */
            pipeListener.popHappened += (List<string> paths) => {

                // On the gui thread
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    /* Create a new SelectionWindow and register a callback to execute when
                     * the user has selected the list of receivers. */
                    SelectionWindow sw = new SelectionWindow(paths);

                    //sw.page.OnselectHappened += (List<Peer> selected, string filepath) => {
                    sw.Selected += (List<Peer> selected, List<string> filepaths) => {
                        // Close the window
                        sw.Close();
                        /* For each peer in the list schedule a new job on a job scheuler. */
                        JobScheduler scheduler = new JobScheduler();
                        // Register a callback to execute in case of connection errors

                        scheduler.ConnectionError += (Job j) => {
                            if (j.Status == Job.JobStatus.ConnectionError)
                                bf.NotifyError(BackgroundForm.ErrorNotificationType.Sending);
                        };
                        scheduler.FileError += () => {
                            bf.NotifyError(BackgroundForm.ErrorNotificationType.File);
                        };
                        for (int i = 0; i < selected.Count; i++) {
                            //tasks.Add(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
                            //                        Settings.Instance.LocalPeer.Name, PeersList.Instance.Peers.ElementAt(i).Id,
                            //                        PeersList.Instance.Peers.ElementAt(i).Name, filepath));
                            outgoingJobThreads.Add(scheduler.scheduleJob(new SendingJob(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
                                                  Settings.Instance.LocalPeer.Name, selected.ElementAt(i).Id,
                                                  selected.ElementAt(i).Name, filepaths), filepaths)));
                            //tasks.Add(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
                            //                        Settings.Instance.LocalPeer.Name, PeersList.Instance.Peers.ElementAt(i).Id,
                            //                        PeersList.Instance.Peers.ElementAt(i).Name, filepath));
                            //scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
                            //                        Settings.Instance.LocalPeer.Name, PeersList.Instance.Peers.ElementAt(i).Id,
                            //                        PeersList.Instance.Peers.ElementAt(i).Name, filepaths), filepaths));
                        }
                    };
                    sw.Show();
                });

            };
            // Run the thread which will listen on the pipe
            pipeListener.run();

            /* Start the background form which will manage the tray icon 
             * and the notification window */
            bf = new BackgroundForm();
            bf.BackgroundFormClosing += BeforeClosing;

            //for (int i = 0; i < 20; i++) {
            //    PeersList.Instance.put(new Peer(new Random().Next() + i + "", new Random().Next() + i + "", new Random().Next() + i + ""));
            //}

        }

        private void BeforeClosing() {
            // Stops the hello protocol thread (HelloThread)
            hellothread.StopThread();
            // Stop the receiver thread (ServerClass)
            receiver.StopThread();
            // Stop the pipe listener thread (PipeDaemon)
            pipeListener.StopThread();
            // Lets stop all the thread that are managing outgoing jobs
            foreach (ExecutableThread t in outgoingJobThreads)
                t.StopThread();

            // Waits for the end of all these threads
            receiver.Join();
            pipeListener.Join();
            hellothread.Join();
            foreach (ExecutableThread t in outgoingJobThreads)
                t.Join();

            SettingsPersistence.writeSettings();
            GarbageCleanup gc = new GarbageCleanup();
            gc.run();
            gc.Join();
        }

        private void Hellothread_OnProfilePicUpdate(string peerId, byte[] newPicture) {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                //updates in GUI a user profile picture

                PeersList.Instance.get(peerId).ByteIcon = newPicture;
            });
        }

        public ToAccept ShowConfirmWindow(ToAccept request) {

            ReceiveWindow rw = new ReceiveWindow(request);
            rw.Show();
            return request;
        }

        /// <summary>
        /// Manages the closing of the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void AppExit(object sender, EventArgs e) {

            //await System.Threading.Tasks.Task.Factory.StartNew(() => {

            //    // Stops the hello protocol thread (HelloThread)
            //    hellothread.StopThread();
            //    hellothread.Join();
            //    // Stop the receiver thread (ServerClass)
            //    receiver.StopThread();
            //    receiver.Join();

            //    // Lets stop all the thread that are managing outgoing jobs
            //    foreach (ExecutableThread t in outgoingJobThreads)
            //        t.StopThread();
            //    foreach (ExecutableThread t in outgoingJobThreads)
            //        t.Join();

            //    SettingsPersistence.writeSettings();
            //    GarbageCleanup gc = new GarbageCleanup();
            //    gc.run();
            //    gc.Join();
            //    //bf.Close();
            //    // however the mutex will be automatically released 
            //    // if this code will be not executed for any reason
            //    mutex.ReleaseMutex();

            //    // Process completed successfully
            //    //Environment.Exit(0);
            //});
            //task.ContinueWith(new Action<System.Threading.Tasks.Task>((antecedent) => {
            mutex.ReleaseMutex();

            //Environment.Exit(0);
                
            //}));
            
        }
    }
}
