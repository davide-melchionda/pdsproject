using System;
using System.IO;

/// <summary>
/// Summary description for Class1
/// </summary>
/// 
namespace FileTransfer
{

    public class Task
    {
<<<<<<< Updated upstream
=======
<<<<<<< Updated upstream
       public class FileInfo
        {
            public enum Type { directory, file };
            public Type type;
            public string id;
            public string name;
            public string sender;
            public string receiver;
            public string requestTimestamp;
            public long size;
        }
        //oggetto information contiene le info sul file originario
        public FileInfo informations;
        //campi riempiti dal modulo di memoria e utilizzati nell'ambito della trasmissione

        public long size;
=======
<<<<<<< master
>>>>>>> Stashed changes
        /**
         * The id of a task is a set of unique informations
         */
        public string Id {
            get { return Info.Name + sender + receiver + requestTimestamp.ToString("yyyyMMddhhmmss") + requestTimestamp.Millisecond; }
        }

        /**
         * The id of the sender of the file interested by this task
         */
        private string sender;
        /**
         * sender property
         */
        public string Sender {
            get { return sender; }
        }

        /**
         * The id of the receiver of the file interested by this task
         */
        private string receiver;
        /**
         * receiver property
         */
        public string Receiver {
            get { return receiver; }
        }

        /**
         * Date of creation of the task
         */
        private DateTime requestTimestamp;
        /**
         * date property
         */
        public DateTime RequestTimestamp {
            get { return requestTimestamp; }
        }

        /**
         * Set of information about the file
         */
        private FileInfo info;
        /**
         * info property
         */
        public FileInfo Info {
            get { return info; }
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
        public string SentName {
            get {
                return sentName;
            }
            set {
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
        public long Size {
            get {
                return size;
            }
            set {
                size = value;
            }
        }

        /**
         * Constructor of the task. Takes only few arguments because the others are
         * retrieved from these ones or initialized calling other methods.
         */
        public Task(string sender, string receiver, string filePath) {
            // Initialize sender and receiver
            this.sender = sender;
            this.receiver = receiver;

            // Initializes file informationa
            info = new FileInfo(filePath);

            // Initializes timestamp
            requestTimestamp = DateTime.Now;

        }

        public class FileInfo {
            /**
             * Enum that has two values, each one coresponding to one of
             * the two possible kind of file: directory and simple file.
             */
            public enum FType {
                DIRECTORY,  // Directory files
                FILE        // Simple files
            };

            /**
             * Type of the file
             */
            private FType type;
            /**
             * type property
             */
            public FType Type;

            /**
             * Name of the file
             */
            private string name;
            /**
             * name property
             */
            public string Name {
                get { return name; }
            }

            /**
             * Size of the file to send
             */
            private long size;
            /**
             * size property
             */
            public long Size {
                get { return size; }
            }

            /**
             * Constructor. Receives the path of a file and initialize
             * allt he iformations about the file itself.
             */
            public FileInfo(string filePath) {
                // Retrieves the path of the file
                name = Path.GetFileName(filePath);

                // Check if the file is a directory
                FileAttributes attr = 0;
                attr = File.GetAttributes(filePath);    
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)  // if it is
                    type = FType.DIRECTORY;
                else    // otherwise
                    type = FType.FILE;

                // Sets the file size
                System.IO.FileInfo fi = new System.IO.FileInfo(filePath);
                size = fi.Length;
                
            }
        }
<<<<<<< Updated upstream
=======
=======
      
        public FileInfo informations;       //oggetto information contiene le info sul file originario

        public long size;                   //campi riempiti dal modulo di memoria e utilizzati nell'ambito della trasmissione
>>>>>>> Packets hiercarchy and client hineritance
>>>>>>> Stashed changes
>>>>>>> Stashed changes
    }
    
}