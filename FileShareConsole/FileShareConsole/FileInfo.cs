using Newtonsoft.Json;
using System.IO;

namespace FileTransfer
{
    //public class FileInfo {
    //    public enum TypeEnum { directory, file };
    //    private TypeEnum typeInfo;
    //    public TypeEnum TypeInfo {
    //        get {
    //            return typeInfo;
    //        }
    //        protected set {
    //            typeInfo = value;
    //        }
    //    }
    //    public string id { get; set; }
    //    public string name { get; set; }

    //    public string requestTimestamp { get; set; }
    //    public long size { get; set; }


    //    public FileInfo(TypeEnum te) {
    //        this.typeInfo = te;
    //    }

    //}

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
        public FType Type
        {
            get { return type; }
            set { type = value; }
        }

    /**
     * Name of the file
     */
    private string name;
        /**
         * name property
         */
        public string Name {
            get { return name; }
            set { name = value; }

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
            set { size=value; }
        }
        /**
      *  A constructor at use of the Json Library
      */
        [JsonConstructor]
        public FileInfo() {}
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

            if (type != FType.DIRECTORY) {
                // Sets the file size
                System.IO.FileInfo fi = new System.IO.FileInfo(filePath);
                size = fi.Length;
            } else {
                size = getDirectoryTotSize(new System.IO.DirectoryInfo(filePath));
            }
        }

        private long getDirectoryTotSize(DirectoryInfo d) {
            long size = 0;
            System.IO.FileInfo[] fis = d.GetFiles();
            foreach (System.IO.FileInfo fi in fis)
                size += fi.Length;

            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
                size += getDirectoryTotSize(di);
            return size;
        }

    }

}
