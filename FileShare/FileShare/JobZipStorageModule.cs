using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransfer;
using System.IO.Compression;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading;

namespace FileShareConsole {
    ///<summary>
    /// Implements a StorageModule which has the purpose to offer a way to zip files when needed and to 
    /// return a file iterator to the zipped file.
    /// The zipped file is created in the position where the target file is, but it's associated to a
    /// timestamp. The module takes trace of the active file iterators which were required and, when 
    /// they all have been closed, removes the zip file from file system.
    ///</summary>
    class JobZipStorageModule : StorageModule {

        public static int READ_BLOCK_SIZE = 1024;
        private static ConcurrentDictionary<string, int> openedFiles = new ConcurrentDictionary<string, int>();
        private static object dictionaryLock = new object();
        private string zipTempFolder = Settings.Instance.AppDataPath + "\\temp";

        private static ConcurrentDictionary<string, ManualResetEvent> events = new ConcurrentDictionary<string, ManualResetEvent>();

        public JobZipStorageModule() {
            System.IO.DirectoryInfo di = new DirectoryInfo(zipTempFolder);
            if (!di.Exists) {
                Directory.CreateDirectory(zipTempFolder);
            }
            //openedFiles = new ConcurrentDictionary<string, int>();
        }

        ///<summary>
        /// Prepares a Job performing the operations needed to execute the transfer.
        /// The file relative to the Job is zipped (but only if necessary) and a
        /// JobFileIterator is created and returned in order to allow the caller to
        /// access to the temporary file to trasnfer.
        ///</summary>
        public FileIterator prepareJob(SendingJob j) {

            //DateTime lastModify = j.Task.RequestTimestamp;  // The timestamp

            string zipName = null;

            JobFileIterator jobIterator = null;

            try {
                // Check zipped file existence
                //if (!File.Exists(zipName)) {  // NO MORE NEEDED
                ManualResetEvent zippedEvent;
                if (j.FilePaths.Count == 1 /*&& j.Task.Info[0].Type == FileTransfer.FileInfo.FType.DIRECTORY*/) {
                    FileAttributes attr = 0;
                    attr = File.GetAttributes(j.FilePaths.Last());

                    bool toZip = true;
                    //DateTime lastModify;
                    string uniquetoken;

                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
                        var directory = new DirectoryInfo(j.FilePaths[0]);
                        //lastModify = RealLastModify(directory);
                        uniquetoken = UniqueTokenFromDirectory(directory);
                    } else {
                        DateTime lastModify = new System.IO.FileInfo(j.FilePaths[0]).LastWriteTimeUtc;
                        uniquetoken = "_" + lastModify.Year + lastModify.Month + lastModify.Day + lastModify.Hour + lastModify.Minute + lastModify.Second + lastModify.Millisecond;
                    }

                    zipName = Path.Combine(zipTempFolder, j.Task.Info[0].Name + uniquetoken);
                    lock (dictionaryLock) {
                        if (File.Exists(zipName)) {
                            toZip = false;
                            events.TryGetValue(zipName, out zippedEvent);
                        } else {
                            zippedEvent = new ManualResetEvent(false);
                            events.TryAdd(zipName, zippedEvent);
                        }

                    }
                    if (toZip) {
                        if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
                            // zip directory (including base directory)
                            ZipFile.CreateFromDirectory(j.FilePaths.Last(), zipName, CompressionLevel.NoCompression, false);
                        } else {  // otherwise
                                  // Note: if it's a file, ZipArchive has to work on a new file in order to zip
                            ZipArchive newFile = ZipFile.Open(zipName, ZipArchiveMode.Create);
                            //string p = Path.GetDirectoryName(filePath);
                            newFile.CreateEntryFromFile(j.FilePaths.Last(), Path.GetFileName(j.FilePaths.Last()), CompressionLevel.NoCompression);
                            newFile.Dispose();
                        }
                        zippedEvent.Set();
                        lock (dictionaryLock) {
                            events.TryRemove(zipName, out zippedEvent);
                            zippedEvent = null;
                        }
                    } else {
                        //TODO
                        //WAIT UNITL FILE IS READY;
                        if (zippedEvent != null)
                            zippedEvent.WaitOne();
                    }
                } else {
                    // Constructs the zip file name
                    Directory.CreateDirectory(Path.Combine(zipTempFolder, j.Id));
                    zipName = Path.Combine(zipTempFolder, j.Id, j.Id + ".tmp");// + @"\" + Path.GetFileNameWithoutExtension(filePath) + date + ".zip";

                    ZipArchive newFile = ZipFile.Open(zipName, ZipArchiveMode.Create);

                    foreach (string filePath in j.FilePaths) {
                        /* Retrieves the file attributes to check if the file is a directory or not
                            because the way to zip is different in the two cases. */
                        FileAttributes attr = 0;
                        attr = File.GetAttributes(filePath);

                        if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
                            string tmpArchivePath = Path.Combine(Path.GetDirectoryName(zipName), Path.GetFileName(filePath) + ".zip");
                            ZipFile.CreateFromDirectory(filePath, tmpArchivePath, CompressionLevel.NoCompression, false);
                            newFile.CreateEntryFromFile(tmpArchivePath, Path.GetFileName(filePath), CompressionLevel.NoCompression);
                            File.Delete(tmpArchivePath);
                        } else {
                            newFile.CreateEntryFromFile(filePath, Path.GetFileName(filePath), CompressionLevel.NoCompression);
                        }
                    }
                    //}

                    newFile.Dispose();
                }
                // Intializes the remaining info in the task (SentName and Size)
                j.Task.SentName = Path.GetFileName(zipName);    // name of the zipped file
                j.Task.Size = new System.IO.FileInfo(zipName).Length;     // size of the zipped file

                // Returns an iterator to the file
                jobIterator = (JobFileIterator)getIterator(zipName);
                jobIterator.Job = j;
                
                //}
            } catch (IOException e) {
                if (zipName != null)
                    File.Delete(zipName);
                throw e;
            }

            return jobIterator;
            // File not found
            //return null;

        }

        /**
         * Given a Task as parameter, creates a related Job in the receiving jobs list,
         * prepares a file coherent with what specified by the Task and returns a FileIterator
         * for this file.
         */
        public FileIterator createJob(FileTransfer.Task task, string receivePath) {

            // Retrieves current date and computes a string to uniquely identiy the task
            DateTime now = DateTime.Now;
            string zipTempFolder = Settings.Instance.AppDataPath + "\\temp";

            //string path = zipTempFolder + "\\" + Path.GetFileNameWithoutExtension(task.Info.Name) + now.ToString("yyyyMMddhhmmss") + now.Millisecond + ".zip";
            Directory.CreateDirectory(Path.Combine(zipTempFolder, task.Id));
            String path = Path.Combine(zipTempFolder, task.Id, task.Id + ".tmp");
            ReceivingJob job = new ReceivingJob(task, receivePath);

            // Push the job in the receiving jobs list
            //JobsList.Receiving.push(job);

            DirectoryInfo di = Directory.CreateDirectory(receivePath);


            // Creates the new file. Note: the file is zipped
            File.Create(path).Close();

            // Creates a new iterator to the file
            JobFileIterator iterator = (JobFileIterator)getIterator(path);
            iterator.Job = job;

            // Defines the behavior when the iterator is going to be closed
            // registering a callback on the related event.
            iterator.BeforeIteratorClosed += () => {
                // Job not completed: nothing to extract
                if (job.SentByte != job.Task.Size)
                    return;
                if (job.Task.Info.Count == 1/* && job.Task.Info[0].Type == FileTransfer.FileInfo.FType.DIRECTORY*/) {
                    //ZipFile.ExtractToDirectory(path, GetUniqueFilePath(receivePath + "\\" + job.Task.Info.Last().Name));
                    if (job.Task.Info.Last().Type == FileTransfer.FileInfo.FType.DIRECTORY)
                        ZipFile.ExtractToDirectory(path, GetUniqueFilePath(receivePath + "\\" + job.Task.Info.Last().Name));
                    else
                        using (ZipArchive archive = ZipFile.OpenRead(path)) {
                            string tempPath;
                            foreach (ZipArchiveEntry entry in archive.Entries) {
                                tempPath = GetUniqueFilePath(Path.Combine(job.DestinationPath, entry.Name));
                                entry.ExtractToFile(tempPath);
                            }
                        }
                } else {
                    using (ZipArchive archive = ZipFile.OpenRead(path)) {
                        foreach (FileTransfer.FileInfo fileInfo in job.Task.Info) {
                            foreach (ZipArchiveEntry entry in archive.Entries) {
                                if (fileInfo.Name == entry.Name) {
                                    if (fileInfo.Type == FileTransfer.FileInfo.FType.DIRECTORY) {
                                        string uniqueFileName = GetUniqueFilePath(Path.Combine(Path.GetDirectoryName(path), entry.Name));
                                        entry.ExtractToFile(uniqueFileName + ".zip");
                                        ZipFile.ExtractToDirectory(uniqueFileName + ".zip", GetUniqueFilePath(Path.Combine(receivePath, fileInfo.Name)));
                                        File.Delete(uniqueFileName + ".zip");
                                    } else {
                                        entry.ExtractToFile(GetUniqueFilePath(Path.Combine(job.DestinationPath, entry.Name)));
                                    }
                                }
                            }
                        }
                    }
                }
                //JobsList.Receiving.remove(job.Id);
            };

            return iterator;
        }


        /**
         * Creates an iterator which allows the owner to navigate in the file specified by
         * the path given as parameter.
         * The ZIPStorageModule registers the Iterator so that he can take trace of the
         * numbero of active iterator pointing to that file.
         */
        public override FileIterator getIterator(string path) {

            long fileLength = new System.IO.FileInfo(path).Length;
            int count;
            lock (dictionaryLock) {
                if (openedFiles.TryGetValue(path, out count))
                    openedFiles.TryUpdate(path, count + 1, count);
                else
                    openedFiles.TryAdd(path, 1);
            }

            FileIterator iterator = new JobFileIterator(this, path);

            return iterator;
        }

        protected override void onIteratorClosed(FileIterator iterator) {

            JobFileIterator ftiterator = (JobFileIterator)iterator;

            lock (dictionaryLock) {
                long fileLength = new System.IO.FileInfo(ftiterator.filePath).Length;
                int count;

                if (openedFiles.TryGetValue(ftiterator.filePath, out count)) {
                    count -= 1;
                    if (count == 0) {
                        openedFiles.TryRemove(ftiterator.filePath, out count);
                        File.Delete(ftiterator.filePath);
                        //if (ftiterator.Job.Task.Info.Count != 1)
                        if (Path.GetDirectoryName(ftiterator.filePath) != zipTempFolder)
                            Directory.Delete(Path.GetDirectoryName(ftiterator.filePath));
                    } else
                        openedFiles.TryUpdate(ftiterator.filePath, count, count + 1);
                }
            }


        }

        /**
         * Manages the duplicates if needed. It follows a Windows style pattern. 
        
         */

        private string GetUniqueFilePath(string filepath) {

            int number = 0;

            if (Directory.Exists(filepath)) {
                string tempPath;
                do {
                    tempPath = Path.Combine(string.Format("{0} ({1})", filepath, ++number));
                } while (Directory.Exists(tempPath));
                return tempPath;
            }

            if (File.Exists(filepath)) {
                string folder = Path.GetDirectoryName(filepath);
                string filename = Path.GetFileNameWithoutExtension(filepath);
                string extension = Path.GetExtension(filepath);

                Match regex = Regex.Match(filepath, @"(.+) \((\d+)\)\.\w+");        // Aggiunge un controllo aggiuntivo per essere certi che abbiamo davanti un file
                                                                                    // e per catturare eventualmente il valore progressivo già presente tra parentesi

                if (regex.Success) {
                    filename = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }

                do {
                    filepath = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, ++number, extension));
                } while (File.Exists(filepath));
            }

            return filepath;

        }

        public class JobFileIterator : FileIterator {
            /**
             * The fileStream used by the iterator to iterate inside
             * the file.
             */
            private FileStream fileStream;

            /**
             * Represents the offset of the iterator in the file. The
             * iterator is like a pointer in the file, and points to the
             * byte offset-th byte in the file.
             */
            private int offset;

            /**
             * path of the file pointed by the iterator.
             */
            public string filePath;

            /**
             * The job linked to this iterator.
             */
            private Job job;
            /**
             * job property.
             */
            public Job Job {
                get {
                    return job;
                }
                set {
                    job = value;
                }
            }

            public int READ_BLOCK_SIZE {
                get {
                    return 1 * 1024 * 1024 /*DEFAULT 100MB*/;
                }
            }

            /**
             * If any callback is registered to this delegate, the iterator
             * class gives the assurance that this callback will be executed 
             * before the onIteratorClosed() method of the StorageModule.
             */
            public delegate void BeforeIteraorClosedDel();
            public event BeforeIteraorClosedDel BeforeIteratorClosed;

            public static Dictionary<string, FileStreamRegisteredCounter> fileDescriptors = new Dictionary<string, FileStreamRegisteredCounter>();
            public static object descriptorsLock = new object();

            /**
             * Constructor.
             */
            public JobFileIterator(StorageModule s, string filePath) : base(s) {
                lock (descriptorsLock) {
                    FileStreamRegisteredCounter r;
                    if (fileDescriptors.ContainsKey(filePath)) {
                        fileDescriptors.TryGetValue(filePath, out r);
                        fileStream = r.FileStream;
                        r.Count++;
                    } else {
                        //fileStream = File.Open(filePath, FileMode.OpenOrCreate);
                        fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, System.IO.FileShare.Read);
                        fileDescriptors.Add(filePath, new FileStreamRegisteredCounter(fileStream, 1));
                    }
                }
                this.filePath = filePath;
                offset = 0;
            }

            /**
             * Puts in the given buffer the next READ_BLOCK_SIZE byte and
             * returns the number of byte he read.
             * Executes all the updates he wants in the meanwhile, such like
             * the update of the completion percentage for the job he's 
             * associated to.
             */
            public override int next(byte[] buffer) {
                //long length = new System.IO.FileInfo(filePath).Length;
                long length = job.Task.Size;
                int read = 0;
                fileStream.Seek(offset, 0);
                if (offset + READ_BLOCK_SIZE <= length)
                    read = fileStream.Read(buffer, 0, READ_BLOCK_SIZE);
                else {
                    int toRead = (int)length - offset;
                    read = fileStream.Read(buffer, 0, toRead); // no-overflow
                }
                job.SentByte += read;
                offset += read;
                return read;
            }

            /**
             * Writes 'count' bytes (or less if the file is too small) from buffer 
             * buf in the file pointed by the Task in the Job and returns the number
             * of written bytes.
             * Increments the completion percentage in the job coherently with the
             * just executed writing operation.
             */
            public override int write(byte[] buf, int count) {

                // How much bytes were written
                int written = 0;

                /* Size of the file in which execute the write operation.
                   This size is coherent with that one defined by the Task
                   included in the job associated to this iterator.
                   NOTE: This means that this is a logical dimension, not 
                   the size of the file on the file system. */
                long length = job.Task.Size;

                fileStream.Seek(offset, 0);

                // If the number of bytes to write is lower than the
                // number of bytes to the end of the file.
                if (offset + count <= length)
                    // writes the required amount of bytes
                    written = count;
                else
                    // Writes only the number of bytes which allows
                    // to now overflow file limits.
                    written = (int)length - offset;

                // Writes bytes in the file
                fileStream.Write(buf, 0, written);

                // Update offset
                offset += written;
                job.SentByte += written;

                // Returns the number of bytes written.
                return written;
            }

            /**
             * Returns true if the iterator is not positioned to the end of the file,
             * false otherwise.
             */
            public override bool hasNext() {
                //return offset < new System.IO.FileInfo(filePath).Length;
                return offset < job.Task.Size;
            }

            public override void close() {
                int openedNum;
                lock (dictionaryLock) {
                    //openedFiles.TryGetValue(filePath, out openedNum);
                    FileStreamRegisteredCounter r;
                    fileDescriptors.TryGetValue(filePath, out r);
                    if (r.Count < 2) {
                        // Close the filestream
                        fileStream.Close();
                        fileDescriptors.Remove(filePath);
                    } else
                        r.Count--;
                }

                // If anyone is registered to the closing event
                // and requires to be executed befor the closing.
                if (BeforeIteratorClosed != null)
                    BeforeIteratorClosed();

                // Calls the closing method of the father
                base.close();
            }
        }

        public class FileStreamRegisteredCounter {

            public FileStream FileStream { get; set; }
            public int Count { get; set; }

            public FileStreamRegisteredCounter(FileStream stream, int count) {
                FileStream = stream;
                Count = count;
            }
        }

        /// <summary>
        /// Utility method: needed for the computation of the unique token to put in the name of the
        /// zipped file (see UniqueTokenFromDirectory).
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string CreateMD5(string input) {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create()) {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++) {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Utitity method: gets a unique token to use in the name of the zipped file to uniquely
        /// identify it.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static string UniqueTokenFromDirectory(DirectoryInfo directory) {
            string token = "";

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create()) {

                //TODO - Everything
                //if (directory.GetFiles().Length > 0) {
                //    var file = (from f in directory.GetFiles()
                //                orderby f.LastWriteTime descending
                //                select f);
                //    foreach (System.IO.FileInfo f in file)
                //        token = CreateMD5(token + f.LastWriteTime.ToLongDateString());
                //}
                //curLastModify = file.LastWriteTimeUtc;

                foreach (System.IO.FileInfo f in directory.GetFiles())
                    token = CreateMD5(token + f.Name + f.LastWriteTime.ToUniversalTime().Subtract(
                                                        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                                                        ).TotalMilliseconds);

                foreach (DirectoryInfo d in directory.GetDirectories())
                    token = CreateMD5(token + UniqueTokenFromDirectory(d));
            }

            return token;
        }

    }

}