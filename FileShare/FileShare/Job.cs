using System;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// Summary description for Class1
/// </summary>
/// 
namespace FileTransfer
{

    public class Job : INotifyPropertyChanged {

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

        /// <summary>
        /// The path of the destination directory where received files will be putted.
        /// </summary>
        public string DestinationPath { get; set; }

        /// <summary>
        /// The path of the base directory of files to zip
        /// </summary>
        private List<string> filePaths;
        /**
         * filePath property
         */
         public List<string> FilePaths {
            get {
                return filePaths;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public int SentByte {
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

        public bool Active {
            get; set;
        }

        /**
         * Public constructor
         */
        public Job(Task task, List<string> filePaths) {
            timestamp = DateTime.Now;
            this.task = task;
            this.filePaths = filePaths;
            Active = true;
        }
        
        /**
         * Public constructor
         */
        public Job(Task task, string filePath) {
            timestamp = DateTime.Now;
            this.task = task;
            DestinationPath = filePath;
            Active = true;
        }
    }
}