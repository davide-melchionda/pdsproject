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
         * Constructor
         */
        public JobScheduler() {
        }

        /**
         * Given a job, executes its computation. The computation of a 
         * Job consist in the assignment of the job to a JobExecutor 
         * which has the purpose of execute the file tranfer according 
         * to a specific protocol.
         */
        public void scheduleJob(Job job) {

            // Pushes the job in the list of in-service jobs
            JobsList.Sending.push(job);

            // Schedules a thread to send the packet 
            JobExecutor sender = new JobExecutor(job);
            // When the transmissione ends 
            sender.OnTrasmissionEnd += () => {
                // Removes the job from the active jobs list 
                JobsList.Sending.remove(job.Id);
            };

            // Run the sender 
            sender.run();
            
        }

    }

}