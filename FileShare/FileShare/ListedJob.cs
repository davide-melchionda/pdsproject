using FileTransfer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileShare {

    /// <summary>
    /// This class represents a job as it's listed in the list of jobs. An istance of this
    /// class simply wraps a job and is linked to it.
    /// </summary>
    class ListedJob : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property representing the job wrapped by an instance of this class
        /// </summary>
        public Job Job {
            get;
        }

        /// <summary>
        /// Constructor: a ListedJob can only be created starting from an existing job.
        /// No possibility to customize the task or the filepath.
        /// </summary>
        /// <param name="j"></param>
        public ListedJob(Job j) {
            Job = j;
        }

        /// <summary>
        /// A property which has value equal to 'true' if the job is completed,
        /// 'false' otherwise.
        /// </summary>
        private bool completed = false;
        public bool Completed {
            get {
                return completed;
            }
            set {
                completed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Completed"));
            }
        }

        /// <summary>
        /// A property which has value equal to 'true' if the job is not completed 
        /// yet, 'false' otherwise.
        /// </summary>
        public bool Partial {
            get; set;
        }

        /// <summary>
        /// A boolean which specify if an error occurred
        /// </summary>
        private bool error;
        /// <summary>
        /// The property associated to the error filed
        /// </summary>
        public bool Error {
            get {
                return error;
            }
            set {
                error = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Error"));
            }
        }

        /// <summary>
        /// A message which describes the current status of the job
        /// </summary>
        private string message;
        /// <summary>
        /// The property corresponding to the message field
        /// </summary>
        public string Message {
            get {
                return message;
            }
            set {
                message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Message"));
            }
        }
    }
}
