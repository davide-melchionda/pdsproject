using Newtonsoft.Json;
using System;
using System.IO;


namespace FileTransfer
{

    public class Task
    {

        /**
         * The id of a task is a set of unique informations
         */
        public string Id
        {
            get { return Info.Name + sender + receiver + requestTimestamp.ToString("yyyyMMddhhmmss") + requestTimestamp.Millisecond; }
            

        }

        /**
         * The id of the sender of the file interested by this task
         */
        private string sender;
        /**
         * sender property
         */
        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        /**
         * The id of the receiver of the file interested by this task
         */
        private string receiver;
        /**
         * receiver property
         */
        public string Receiver
        {
            get { return receiver; }
            set { receiver = value; }
        }

        /**
      * The name of the sender of the file interested by this task
      */
        private string senderName;
        /**
         * senderName property
         */
        public string SenderName
        {
            get { return senderName; }
            set { senderName = value; }
        }

        /**
         * The name of the receiver of the file interested by this task
         */
        private string receiverName;
        /**
         * receiverName property
         */
        public string ReceiverName
        {
            get { return receiverName; }
            set { receiverName = value; }
        }

        /**
         * Date of creation of the task
         */
        private DateTime requestTimestamp;
        /**
         * date property
         */
        public DateTime RequestTimestamp
        {
            get { return requestTimestamp; }
            set { requestTimestamp = value; }

        }

        /**
         * Set of information about the file
         */
        private FileInfo info;
        /**
         * info property
         */
        public FileInfo Info
        {
            get { return info; }
            set { info = value; }

        }

        /**
         * The name of the file which will be sent. This name could be different
         * from the name of the file present in the FileInfo object, due to the 
         * pre-elaboration process that could be executed befor sending the file.
         */
        private string sentName;
        /**
         * sentName property
         */
        public string SentName
        {
            get
            {
                return sentName;
            }
            set
            {
                sentName = value;
            }
        }

        /**
         * Dimension in byte of the file that will be sent (this size could
         * be different from the size associated to the file in the FileInfo
         * object: the reason is that some pre-elaboration could be done on 
         * the file in order to improve performances.
         */
        private long size;
        /**
         * size property
         */
        public long Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        /**
         * Constructor of the task. Takes only few arguments because the others are
         * retrieved from these ones or initialized calling other methods.
         */
        public Task(string sender, string senderName, string receiver, string receiverName,string filePath)
        {
            // Initialize sender and receiver
            this.sender = sender;
            this.receiver = receiver;
            this.senderName = senderName;
            this.receiverName = receiverName;

            // Initializes file informationa
            info = new FileInfo(filePath);

            // Initializes timestamp
            requestTimestamp = DateTime.Now;

        }

        /**
        * Constructor used by the Json library
        * 
        */
        [JsonConstructor]
        public Task() { }

    }

}