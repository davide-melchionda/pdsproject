﻿
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

        ///<summary>
        /// A form which will never be shown and which isresponsible of managing the
        /// logic behind the notify icon shwon in the system tray. */
        /// </summary> 
        BackgroundForm bf;
        
        void AppStartup(object sender, StartupEventArgs e) {

            // Start the thread responsible of the neighbor discovery process
            new HelloThread().run();

            // Start the thread responsible of receiving request of transferring files
           ServerClass receiver= new ServerClass(new TnSProtocol());
           receiver.RequestReceived += RequestMessageBox.Show;
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
                        for (int i = 0; i < selected.Count; i++)
                            scheduler.scheduleJob(new Job(new FileTransfer.Task(Settings.Instance.LocalPeer.Id,
                                                                                    PeersList.Instance.Peers.ElementAt(i).Id,
                                                                                    filepath), filepath));
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

        /// <summary>
        /// Manages the closing of the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AppExit(object sender, EventArgs e) {
            bf.Close();
            // Process completed successfully
            Environment.Exit(0);
        }
    }
}
