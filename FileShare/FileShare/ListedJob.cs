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

        /// <summary>
        /// Implementation of INotifyPropertyChange: property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Represents almost the number of seconds occourred from when the transfer started.
        /// It was preferred to memorize seconds as int instead of a DateTime object due to the
        /// usage that will be one of this field: it will be useless to mantain all the information
        /// that a DateTime object mantains.
        /// </summary>
        private double transferStartTime;

        /// <summary>
        /// Represents the quantity of byte sent when measuring starts
        /// </summary>
        private int sentFromStartTime;

        /// <summary>
        /// To determine if show a label or not: returns 'true' if there is only
        /// one file to transfer.
        /// </summary>
        public bool SingleFile {
            get { return Job.Task.Info.Count == 1; }
        }

        /// <summary>
        /// TO show on the interface: number of files minus one
        /// </summary>
        public int FilesCountMinusOne {
            get { return Job.Task.Info.Count - 1; }
        }

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
            transferStartTime = 0.0;
            Stopped = false;
            //Preparing = j.SentByte == 0;
            //j.PropertyChanged += update;
            //Message = "In preparazione...";
        }

        //private void update(object sender, PropertyChangedEventArgs args) {
        //    if (args.PropertyName.Equals("Percentage")) {
        //        if (Job.SentByte != 0) {
        //            Preparing = false;
        //            Message = null;
        //        } else if (Job.SentByte == Job.Task.Size) {
        //            Completing = true;
        //            Message = "In completamento";
        //        }
        //        //Job.PropertyChanged -= updatePreparing;
        //    }
        //}

        /// <summary>
        /// If the job is in pre-sending phase (preparing the sent)
        /// </summary>
        private bool preparing;
        public bool Preparing {
            get {
                return preparing;
            }
            set {
                preparing = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Preparing"));
            }
        }

        private bool completing;
        public bool Completing {
            get {
                return completing;
            }
            set {
                completing = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Completing"));
            }
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

        /// <summary>
        /// The job execution was stoppad due to any reason.
        /// </summary>
        public bool Stopped {
            get; set;
        }

        /// <summary>
        /// An estimate of the time needed to complete the job.
        /// </summary>
        private int timeLeft;
        /// <summary>
        /// The public property associated to the field timeLeft
        /// </summary>
        private int TimeLeft {
            get {
                return timeLeft;
            }
            set {
                timeLeft = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TimeLeft"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RemainingTime"));
            }
        }

        public string RemainingTime {
            get {
                if (timeLeft > 3600)
                    return ((int)(timeLeft / 3600)) + "h " +
                            ((int)((timeLeft % 3600) / 60)) + "m " +
                            ((timeLeft % 3600) % 60) + "s";
                else if (timeLeft > 60)
                    return ((int)(timeLeft / 60)) + "m " + (timeLeft % 60) + "s";
                else if (timeLeft > 0)
                    return timeLeft + "s";
                else
                    return "";
            }
        }

        /// <summary>
        /// Updates the property TimeLeft in orded to make a more recent estimatio of
        /// the time remaining to complete the job.
        /// </summary>
        internal void UpdateTimeLeft() {
            // If not start time was setted up, we have to do it
            if (transferStartTime == 0.0) {
                if (Job.SentByte > 0) { // but only if trasnfer actually started
                    transferStartTime = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;   // Set start time to now
                    sentFromStartTime = Job.SentByte;
                }
                return;
            }

            // byteSent : (Now - transferStartTime) = totByteToSent : x
            // where x is the new TimeLeft value
            double startSeconds = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - transferStartTime;
            double remainingTime = (startSeconds / (Job.SentByte - sentFromStartTime)) * (Job.Task.Size - Job.SentByte);
            if (TimeLeft != (int)Math.Round(remainingTime))
                TimeLeft = (int)Math.Round(remainingTime);

        }
    }
}
