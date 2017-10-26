using System;

namespace FileTransfer
{

    public class Job {
    
        /**
         * The Id of a job (coresponds to the id of the associated task)
         */
        public string Id {
            get { return task.Id; }
        }

        /**
         * The task on associated to this job
         */
        private Task task;
        /**
         * task porperty
         */
         public Task Task {
            get {
                return task;
            }
        }

        /**
         * Local path of the file object of the job
         */
        private string filePath;
        /**
         * filePath property
         */
         public string FilePath {
            get {
                return filePath;
            }
        }

        /**
         * Timestamp which indicates the moment in which the job was created
         */
        private DateTime timestamp;
        /**
         * timestamp property
         */
         public DateTime Timestamp {
            get {
                return timestamp;
            }
        }

        private int sentByte;
        public int SentByte {
            get {
                return sentByte;
            }
            set {
                sentByte = value;
            }
        }

        public double getPercentage() {
            // TODO
            // Computes the percentage
            return 0.0;
        }

        /**
         * Public constructor
         */
        public Job(Task task, string filePath) {
            timestamp = DateTime.Now;
            this.task = task;
            this.filePath = filePath;
        }
    }
}