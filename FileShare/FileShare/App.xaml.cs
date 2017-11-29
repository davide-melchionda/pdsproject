
using FileShareConsole;
using FileTransfer;
using HelloProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Windows;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application {

        ///<summary>
        /// A form which will never be shown and which isresponsible of managing the
        /// logic behind the notify icon shwon in the system tray. */
        /// </summary> 
        BackgroundForm bf;
   
        void AppStartup(object sender, StartupEventArgs e) {

            SettingsPersistence.readSettings();
            //GarbageCleanup gc = new GarbageCleanup();
            //gc.run();

            //WindowsIdentity wi = WindowsIdentity.GetCurrent();
            //Settings.Instance.LocalPeer.Name = Environment.UserName;//wi.Name;

            //Settings.Instance.PicturePath = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Temp\" + Environment.UserName + @".bmp";

            // Start the thread responsible of the neighbor discovery process
            HelloThread hellothread=new HelloThread();
            hellothread.OnProfilePicUpdate += Hellothread_OnProfilePicUpdate;
            hellothread.run();

            // Start the thread responsible of receiving request of transferring files
            ServerClass receiver= new ServerClass();
            receiver.RequestReceived += ( ToAccept request) => {

                // On the gui thread
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    //mostra la finestra e prende in uscita path e response
                    ReceiveWindow rw = new ReceiveWindow(request);
                    rw.ShowDialog();

                });
                return request;
            };
                receiver.ConnectionError += () => {
                bf.NotifyError(BackgroundForm.ErrorNotificationType.Receiving);
            };
            receiver.run();

            /* Create, configura and start the thread responsible of managing the communication on a pipe 
             * with the application which inform this process of the path of the file the user wants to send. */
            PipeDaemon pipeListener = new PipeDaemon(); 
            /* Register on the pipeListener a callback to execute when the user wants to send
             * a new file. */
            pipeListener.popHappened += (string path) => {
                
                // On the gui thread
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    /* Create a new SelectionWindow and register a callback to execute when
                     * the user has selected the list of receivers. */
                    SelectionWindow sw = new SelectionWindow(path);
                    
                    //sw.page.OnselectHappened += (List<Peer> selected, string filepath) => {
                    sw.Selected += (List<Peer> selected, string filepath) => {
                        // Close the window
                        sw.Close();
                        /* For each peer in the list schedule a new job on a job scheuler. */
                        JobScheduler scheduler = new JobScheduler();
                        // Register a callback to execute in case of connection errors
                        scheduler.ConnectionError += () => {
                            bf.NotifyError(BackgroundForm.ErrorNotificationType.Sending);
                        };
                        for (int i = 0; i < selected.Count; i++)
                            scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
                                                        Settings.Instance.LocalPeer.Name, PeersList.Instance.Peers.ElementAt(i).Id,
                                                        PeersList.Instance.Peers.ElementAt(i).Name, filepath), filepath));
                    };                    
                    sw.Show();
                });
              
            };
            // Run the thread which will listen on the pipe
            pipeListener.run();

            /* Start the background form which will manage the tray icon 
             * and the notification window */
            bf = new BackgroundForm();
        }

        private void Hellothread_OnProfilePicUpdate(string peerId, byte[] newPicture)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                //updates in GUI a user profile picture

                PeersList.Instance.get(peerId).ByteIcon = newPicture;
            });
        }

        public ToAccept  ShowConfirmWindow(ToAccept request)
        {
            
            ReceiveWindow rw = new ReceiveWindow(request);
            rw.Show();

            return request;
        }

        /// <summary>
        /// Manages the closing of the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AppExit(object sender, EventArgs e) {
            bf.Close();SettingsPersistence.writeSettings();
            GarbageCleanup gc = new GarbageCleanup();
            gc.run();
            // Process completed successfully
            Environment.Exit(0);
        }
    }
}
