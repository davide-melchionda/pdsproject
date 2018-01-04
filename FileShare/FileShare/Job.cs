using System;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// Summary description for Class1
/// </summary>
/// 
namespace FileTransfer
{

    public abstract class Job : INotifyPropertyChanged {

        /// <summary>
        /// The Id of a job (coresponds to the id of the associated task)
        /// </summary>
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

        private long sentByte;

        public event PropertyChangedEventHandler PropertyChanged;

        public long SentByte {
            get {
                return sentByte;
            }
            set {
                sentByte = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Percentage"));
            }
        }

        public int Percentage {
            get {
                double tmp = (double)SentByte / (double)Task.Size;
                double val = (tmp * 100);
                if (val < 0)
                    ;
                return (int)val;
            }
        }

        //public bool Active {
        //    get; set;
        //}
        private JobStatus status;
        public JobStatus Status {
            get {
                return status;
            }
            set {
                status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }

        ///**
        // * Public constructor
        // */
        //public Job(Task task, List<string> filePaths) {
        //    timestamp = DateTime.Now;
        //    this.task = task;
        //    this.filePaths = filePaths;
        //    Status = Job.JobStatus.Active;
        //}
        
        ///**
        // * Public constructor
        // */
        //public Job(Task task, string filePath) {
        //    timestamp = DateTime.Now;
        //    this.task = task;
        //    DestinationPath = filePath;
        //    Status = Job.JobStatus.Active;
        //}

        ///<summary>
        ///Constructor for a generic Job
        ///</summary>
        public Job(Task task) {
            timestamp = DateTime.Now;
            this.task = task;
            Status = Job.JobStatus.Active;
        }

        /// <summary>
        /// Indicates the status of the current job.
        /// </summary>
        public enum JobStatus {
            Preparing,                     // the job is in preparing state (eg. zipping before sending)
            Completing,                     // the job is in completing state (eg. unzipping after receiving)
            Active,                         // we are executing the job
            Completed,                      // the job is completed
            StoppedByLocal,                 // the job was stopped before completion by the local peer
            StoppedByRemote,                // the job was stopped before completion by the remote peer
            ConnectionError,                // the job was stopped due to a network error
            NotAcceptedByRemote,            // the file(s) in this job was(were) refused by the remote receiving peer
            WaitingForRemoteAcceptance      // we are waiting the remote peer decision about accepting or refusing the file
        }
    }
}