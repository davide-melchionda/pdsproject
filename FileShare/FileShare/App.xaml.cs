using FileShareConsole;
using FileTransfer;
using HelloProtocol;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        /// <summary>
        /// Store the number of times the connection was shutted down and the 
        /// application tried to reconnect. After a maximum number of attepts
        /// a message is shown to the user in order to ask him if he wants to
        /// try again or quit the application.
        /// </summary>
        private int reconnectionAttempts;
        /// <summary>
        /// Max number of attempts befor asking user if he wants to close the app
        /// </summary>
        public const int MAX_RECONNECTION_ATTEMPTS = 3;

        /// <summary>
        /// An object on which aquire a lock to be sure to execute a function 
        /// only once.
        /// </summary>
        private object single_function_execution = new object();
        /// <summary>
        /// A bool variable that indicates, if true, that we are already executing the
        /// reconnection procedure, so that is not needed that another thread tries 
        /// to start this procedure.
        /// This variable will be modified only inside a critical section managed 
        /// through the lock of the object 'single_function_execution'.
        /// </summary>
        //private bool managingReconnection = false;

        private List<Window> openedWindow = new List<Window>();

        //Deny the user the possibility to open two or more instances of the application
        void AppStartup(object sender, StartupEventArgs e) {

            // Only a single instance of this process must e executed
            bool created;   // will be true if the mutext will be created and not retrieved
            // FROM MSDN DOCS: If name is not null and initiallyOwned is true, the calling thread owns the named mutex only if createdNew is true after the call.
            mutex = new Mutex(true, "_FileShareApp_Mutex", out created);
            if (!created) { // If the mutex was not created, we are not the first thread trying to execute the program
                mutex.Close();    // we don't want the system mantins the mutex opened for us
                MessageBox.Show("File shar è già in esecuzione. Puoi accedervi dalla task bar.", "Avvio di File Share", MessageBoxButton.OK, MessageBoxImage.Information);
                Environment.Exit(0);
            }

            // Read settings from the settings file so to retrieve user preferences
            SettingsPersistence.readSettings();
            if (Settings.Instance.DontShowSetup == false) {
                ProfileSetupWindow pw = new ProfileSetupWindow();
                pw.ShowDialog();
            }

            // Starts the thread responsible of implementing the Neighboor Discovery protocol
            InitHelloProtocolThread();
            reconnectionAttempts = 0;

            // Starts the thread responsible of listening for new connections
            InitReceiverThread();

            // Starts the thread responsible of managing pipe connections
            InitPipeDaemonThread();


            /* Start the background form which will manage the tray icon 
             * and the notification window */
            bf = new BackgroundForm();
            bf.BackgroundFormClosing += BeforeClosing;


            // A FOOLISH (BUT USEFUL) WAY TO DEBUG A MULTI-USER SITUATION 
            //for (int i = 0; i < 20; i++) {
            //    PeersList.Instance.put(new Peer(new Random().Next() + i + "", new Random().Next() + i + "", new Random().Next() + i + ""));
            //}
        }

        /// <summary>
        /// Creates, configures and runs a new thread of type HelloThread, which will be responsible
        /// of managing the whole Neighbor discovery procol execution.
        /// </summary>
        private void InitHelloProtocolThread() {
            hellothread = new HelloThread();
            hellothread.NetworkCannotConnect += RestoreHelloProtocolOnNetworkProblems;
            hellothread.OnProfilePicUpdate += Hellothread_OnProfilePicUpdate;
            hellothread.run();
        }

        /// <summary>
        /// Creates, configures and runs a new thread of type ServerClass, which will be resposible
        /// of managing incoming request of transmission.
        /// </summary>
        private void InitReceiverThread() {
            // Start the thread responsible of receiving request of transferring files
            receiver = new ServerClass();
            receiver.RequestReceived += (ToAccept request) => {
                AutoResetEvent goOn = new AutoResetEvent(false);    // To wait for the closing of the window
                // On the gui thread
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    //mostra la finestra e prende in uscita path e response
                    ReceiveWindow rw = new ReceiveWindow(request);
                    openedWindow.Add(rw);
                    rw.Closed += (object target, EventArgs args) => {
                        goOn.Set();
                    };
                    rw.Show();
                    rw.Activate();
                });
                // wait until window is closed
                goOn.WaitOne();
                return request;
            };
            /* Some callback must be registered to the receiver thread to instruct it about 
             what to do in case of errors during the tranfer. */
            receiver.ConnectionError += (Job j) => {
                if (j == null || j.Status == Job.JobStatus.ConnectionError)
                    bf.NotifyError(BackgroundForm.ErrorNotificationType.Receiving, BackgroundForm.ErrorDirection.Receiving);
            };
            receiver.PathError += manageIOException;
            // Run the receiver thread
            receiver.run();
        }

        private void manageIOException(Exception e, Job j) {
            BackgroundForm.ErrorDirection sendingOrReceiving = BackgroundForm.ErrorDirection.Unknown;
            if (j != null) {
                if (j is SendingJob)
                    sendingOrReceiving = BackgroundForm.ErrorDirection.Sending;
                else
                    sendingOrReceiving = BackgroundForm.ErrorDirection.Receiving;
            }

            if (e is PathTooLongException)
                bf.NotifyError(BackgroundForm.ErrorNotificationType.PathTooLong, sendingOrReceiving);
            else if (e is DirectoryNotFoundException)
                bf.NotifyError(BackgroundForm.ErrorNotificationType.DirectoryNotFound, sendingOrReceiving);
            else if (e is UnauthorizedAccessException)
                bf.NotifyError(BackgroundForm.ErrorNotificationType.Path, sendingOrReceiving);
            else
                bf.NotifyError(BackgroundForm.ErrorNotificationType.File, sendingOrReceiving);
        }

        /// <summary>
        /// Creates, configures and runs a new thread of type PipeDaemon, which will be responsible of 
        /// listening on the pipe dedicated to the communication with the one-shot process that sends
        /// (when the user requests it) to start a file transfer.
        /// </summary>
        private void InitPipeDaemonThread() {
            /* Create, configure and start the thread responsible of managing the communication on a pipe 
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
                    openedWindow.Add(sw);
                    sw.Selected += (List<Peer> selected, List<string> filepaths) => {
                        // Close the window
                        sw.Close();
                        /* For each peer in the list schedule a new job on a job scheuler. */
                        JobScheduler scheduler = new JobScheduler();
                        // Register a callback to execute in case of connection errors

                        scheduler.ConnectionError += (Job j) => {
                            if (j.Status == Job.JobStatus.ConnectionError)
                                bf.NotifyError(BackgroundForm.ErrorNotificationType.Sending, BackgroundForm.ErrorDirection.Sending);
                        };
                        scheduler.FileError += manageIOException;
                        for (int i = 0; i < selected.Count; i++) {
                            ExecutableThread worker = scheduler.scheduleJob(new SendingJob(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
                                                  Settings.Instance.LocalPeer.Name, selected.ElementAt(i).Id,
                                                  selected.ElementAt(i).Name, filepaths), filepaths));
                            outgoingJobThreads.Add(worker);
                        }
                    };
                    sw.Show();
                    sw.Activate();
                });
            };

            // Run the thread which will listen on the pipe
            pipeListener.run();
        }

        /// <summary>
        /// Starts a new thread (a Task) and assign to it the task of waiting for the end of the current
        /// HelloThread thread and the restart it after the user has selected a new network (or the same,
        /// if he wants) to use in this session.
        /// Operates directly on the instance field 'hellothread'.
        /// </summary>
        private void RestoreHelloProtocolOnNetworkProblems() {
            //if (Monitor.TryEnter(single_function_execution)) {  // lock if no other is already working here
            System.Threading.Tasks.Task.Factory.StartNew(() => {

                if (Monitor.TryEnter(single_function_execution)) {  // lock if no other is already working here
                    bool quit = false;

                    if (++reconnectionAttempts > MAX_RECONNECTION_ATTEMPTS) {
                        App.Current.Dispatcher.Invoke(() => {
                            MessageBoxResult mbres = MessageBox.Show("Sembra ci sia qualche problema con la rete. Sicuro di voler provare ancora?",
                                            "Impossibile stabilire una connessione", MessageBoxButton.YesNo, MessageBoxImage.Error);
                            if (mbres == MessageBoxResult.No)
                                quit = true;
                            else
                                reconnectionAttempts = 0;
                        });
                    }

                    if (quit) {
                        BeforeClosing();
                        App.Current.Dispatcher.Invoke(() => {
                            App.Current.Shutdown();
                        });
                        return;
                    }

                    // An event to determine when the user has performed the selection of the network
                    AutoResetEvent goOn = new AutoResetEvent(false);

                    // Ask the threads hierarchy to stop
                    hellothread.StopThread();
                    receiver.StopThread();
                    // Wait for the end of the threads
                    hellothread.Join();
                    receiver.Join();

                    // In the UI thread: show the window to select the network
                    App.Current.Dispatcher.Invoke(() => {
                        NetworkWindow nw = new NetworkWindow();
                        // When the window is closing (with network selected) Set() on the event
                        nw.Closed += (object target, EventArgs args) => goOn.Set();
                        nw.Show();
                        nw.Activate();
                    });

                    // ...wait for the Set() on the event (in other words: wait for the network selection)
                    goOn.WaitOne();

                    Monitor.Exit(single_function_execution);    // unlock here

                    // Run the threads again
                    InitHelloProtocolThread();
                    InitReceiverThread();
                }
            });
            //}
        }

        /// <summary>
        /// Befor the application stops we must perform some tasks.
        /// 1. Inform each threa we have runned to stop itself and its childs.
        /// 2. Wait for the end of each one of these threads (through Join())
        /// 3. Write Settings in a file so to save user preferences
        /// 4. Run the GarbageCleanup thread so to delete any pending file 
        ///     (even if there should not be any unmanaged file).
        /// 5. Wait for the en of the GarbageCleanup thread
        /// Please note that this method must be called by the entity performing
        /// the request of stopping the applicaiton. This metho IS NOT automatically
        /// called by the AppExit() method.
        /// The way we do this in this application is associating this method to a 
        /// delegate in the BackroundForm: the closing of this form corresponds to
        /// the end of the application.
        /// </summary>
        private void BeforeClosing() {
            foreach (Window w in openedWindow)
                App.Current.Dispatcher.Invoke(() => {
                    try {
                        w.Close();
                    } catch (Exception e) {
                        ;   // NOTHING TO DO
                    }
                });

            // Stops the hello protocol thread (HelloThread)
            //if (hellothread.Alive)
            hellothread.StopThread();
            // Stop the receiver thread (ServerClass)
            //if (receiver.Alive)
            receiver.StopThread();
            // Stop the pipe listener thread (PipeDaemon)
            //if (pipeListener.Alive)
            pipeListener.StopThread();
            // Lets stop all the thread that are managing outgoing jobs
            foreach (ExecutableThread t in outgoingJobThreads)
                //if (t.Alive)
                t.StopThread();

            // Waits for the end of all these threads
            receiver.Join();
            pipeListener.Join();
            hellothread.Join();
            foreach (ExecutableThread t in outgoingJobThreads)
                t.Join();

            // Write settings to save user preferences
            SettingsPersistence.writeSettings();

            // Run garbage cleanup thread
            GarbageCleanup gc = new GarbageCleanup();
            gc.run();
            gc.Join();  // Wait for completion
        }



        /// <summary>
        /// A callbak to execute in the case of update of the profile picture of the user
        /// </summary>
        /// <param name="peerId"></param>
        /// <param name="newPicture"></param>
        private void Hellothread_OnProfilePicUpdate(string peerId, byte[] newPicture) {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                //updates in GUI a user profile picture

                PeersList.Instance.get(peerId).ByteIcon = newPicture;
            });
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public ToAccept ShowConfirmWindow(ToAccept request) {
        //    ReceiveWindow rw = new ReceiveWindow(request);
        //    rw.ShowActivated = true;
        //    rw.Show();
        //    return request;
        //}

        /// <summary>
        /// Manages the closing of the app. The main role of this method is
        /// to release the mutex aquired at sturtup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AppExit(object sender, EventArgs e) {
            mutex.ReleaseMutex();
            Environment.Exit(0);
        }
    }
}
