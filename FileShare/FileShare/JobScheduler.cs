using FileShareConsole;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static NetProtocol.ProtocolEndpoint;
using static StorageModule;

namespace FileTransfer {
    public class JobScheduler {

        /**
         * Delegate: on connection error during the transfer
         */
        public delegate void OnConnectionError(Job j);
        /**
         * Event on connection error
         */
        public event OnConnectionError ConnectionError;

        public delegate void OnFileError(Exception e, Job j);
        /**
         * Event on connection error
         */
        public event OnFileError FileError;
        /**
         * Constructor
         */
        public JobScheduler() {
        }

        /**
         * Given a job, executes its computation. The computation of a 
         * Job consist in the assignment of the job to a JobExecutor 
         * which has the purpose of execute the file tranfer according 
         * to a specific protocol.
         * Returns the ExecutableThread on which the job is beeing managed.
         */
        public ExecutableThread scheduleJob(SendingJob job) {

            // Pushes the job in the list of in-service jobs
            JobsList.Sending.push(job);

            // Schedules a thread to send the packet 
            JobExecutor sender = new JobExecutor(job);
            // When the transmissione ends 
            sender.OnTrasmissionEnd += () => {
                // Removes the job from the active jobs list 
                JobsList.Sending.remove(job.Id);
            };

            // When a connection error occours
            sender.ConnectionError += (Job j) => {
                ConnectionError?.Invoke(j);
            };


            // When a file error occours
            sender.FileError += (Exception e, Job j) => {
                FileError?.Invoke(e, j);
            };
            // Run the sender 
            sender.run();

            return sender;

        }

    }

}