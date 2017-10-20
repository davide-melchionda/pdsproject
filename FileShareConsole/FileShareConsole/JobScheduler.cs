using FileShareConsole;
using System;
using System.Threading;
using static StorageModule;

namespace FileTransfer {
    public class JobScheduler {
        
        /**
         * Storage module which will be used to access to files
         */
        private StorageModule storage;

        /**
         * Constructor
         */
        public JobScheduler() {
            // Intantiate the storage managemet module he will use
            storage = new JobZipStorageModule();
        }

        /**
         * Given a job, executes its computation. The computation of a 
         * Job consist in the assignment of the job to a JobExecutor 
         * which has the purpose of execute the file tranfer according 
         * to a specific protocol.
         */
        public void scheduleJob(Job job) {

            // Requires memory management module to initialize the file on which
            // the job will work
            FileIterator iterator = ((JobZipStorageModule)storage).prepareJob(job);

            // Pushes the job in the list of in-service jobs
            JobsList.Instance.push(job);

            // Schedules a thread to send the packet
            JobExecutor sender = new JobExecutor(job, iterator);
            // When the transmissione ends
            sender.OnTrasmissionEnd += () => {
                iterator.close();   // Close the iterator
                                    // Removes the job from the active jobs list
                JobsList.Instance.remove(job.Id);
            };

            // Run the sender
            sender.run();
        }

    }
}