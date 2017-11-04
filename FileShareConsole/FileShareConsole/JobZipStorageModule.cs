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

namespace FileShareConsole
{
    /**
     * Implements a StorageModule which has the purpose to offer a way to zip files when needed and to 
     * return a file iterator to the zipped file.
     * The zipped file is created in the position where the target file is, but it's associated to a
     * timestamp. The module takes trace of the active file iterators which were required and, when 
     * they all have been closed, removes the zip file from file system.
     */
    class JobZipStorageModule : StorageModule
    {

        public static int READ_BLOCK_SIZE = 1024;
        private ConcurrentDictionary<string, int> openedFiles;
        private static object dictionaryLock = new object();

        public JobZipStorageModule()
        {
            openedFiles = new ConcurrentDictionary<string, int>();
        }

        /**
         * Prepares a Job performing the operations needed to execute the transfer.
         * The file relative to the Job is zipped (but only if necessary) and a 
         * JobFileIterator is created and returned in order to allow the caller to
         * access to the temporary file to trasnfer.
         */
        public FileIterator prepareJob(Job j)
        {
            string filePath = j.FilePath;   // Path of the file
            DateTime lastModify = j.Task.RequestTimestamp;  // The timestamp

            Logger.log(Logger.ZIP_DEBUG, "Called to zip " + filePath + "\n");

            // Constructs the zip file name
            string date = lastModify.ToString("yyyyMMddhhmmss") + lastModify.Millisecond;
            string zipName = Path.GetDirectoryName(filePath) + Path.GetFileNameWithoutExtension(filePath) + date + ".zip";

            // If the file was modified after the lastModify date, throws an exception
            if (File.GetLastWriteTime(filePath) > lastModify)
            {
                throw new FileVersioningException("The file was modified after the indicated time");
            }

            // Check zipped file existence
            if (!File.Exists(zipName))
            {

                // If the file was not zipped yet
                if (!openedFiles.ContainsKey(filePath))
                {

                    /* Retrieves the file attributes to check if the file is a directory or not
                        because the way to zip is different in the two cases. */
                    FileAttributes attr = 0;
                    attr = File.GetAttributes(filePath);

                    // If the file is a directory
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        // zip directory (including base directory)
                        ZipFile.CreateFromDirectory(filePath, zipName, CompressionLevel.NoCompression, false);
                    }
                    else
                    {  // otherwise
                        // Note: if it's a file, ZipArchive has to work on a new file in order to zip
                        ZipArchive newFile = ZipFile.Open(zipName, ZipArchiveMode.Create);
                        //string p = Path.GetDirectoryName(filePath);
                        newFile.CreateEntryFromFile(filePath, Path.GetFileName(filePath), CompressionLevel.NoCompression);
                        newFile.Dispose();
                    }
                }

                // Intializes the remaining info in the task (SentName and Size)
                j.Task.SentName = Path.GetFileName(zipName);    // name of the zipped file
                j.Task.Size = new System.IO.FileInfo(zipName).Length;     // size of the zipped file

                // Returns an iterator to the file
                JobFileIterator jobIterator = (JobFileIterator)getIterator(zipName);
                jobIterator.Job = j;
                return jobIterator;
            }

            // File not found
            return null;

        }

        /**
         * Given a Task as parameter, creates a related Job in the receiving jobs list,
         * prepares a file coherent with what specified by the Task and returns a FileIterator
         * for this file.
         */
        public FileIterator createJob(FileTransfer.Task task)
        {
            // Retrieves current date and computes a string to uniquely identiy the task
            DateTime now = DateTime.Now;
            string path = Settings.Instance.DefaultRecvPath + Path.GetFileNameWithoutExtension(task.Info.Name) + now.ToString("yyyyMMddhhmmss") + now.Millisecond + ".zip";
            Job job = new Job(task, path);

            // Push the job in the receiving jobs list
            JobsList.Receiving.push(job);

            // Creates the new file. Note: the file is zipped
            File.Create(path).Close();

            // Creates a new iterator to the file
            JobFileIterator iterator = (JobFileIterator)getIterator(path);
            iterator.Job = job;

            // Defines the behavior when the iterator is going to be closed
            // registering a callback on the related event.
            iterator.BeforeIteratorClosed += () => {
                if (job.Task.Info.Type == FileTransfer.FileInfo.FType.DIRECTORY) {
                    ZipFile.ExtractToDirectory(path, this.GetUniqueFilePath(Settings.Instance.DefaultRecvPath + job.Task.Info.Name));
                }  else using (ZipArchive archive = ZipFile.OpenRead(path)) {
                        string tempPath;
                        foreach (ZipArchiveEntry entry in archive.Entries) {
                            tempPath = this.GetUniqueFilePath(Settings.Instance.DefaultRecvPath + entry.Name);
                            entry.ExtractToFile(tempPath);
                        }
                    }
            };

            return iterator;
        }


        /**
         * Creates an iterator which allows the owner to navigate in the file specified by
         * the path given as parameter.
         * The ZIPStorageModule registers the Iterator so that he can take trace of the
         * numbero of active iterator pointing to that file.
         */
        public override FileIterator getIterator(string path)
        {

            long fileLength = new System.IO.FileInfo(path).Length;
            int count;
            lock (dictionaryLock)
            {
                if (openedFiles.TryGetValue(path, out count))
                    openedFiles.TryAdd(path, count + 1);
                else
                    openedFiles.TryAdd(path, 1);
            }

            FileIterator iterator = new JobFileIterator(this, path);

            return iterator;
        }

        protected override void onIteratorClosed(FileIterator iterator)
        {

            JobFileIterator ftiterator = (JobFileIterator)iterator;

            lock (dictionaryLock)
            {
                long fileLength = new System.IO.FileInfo(ftiterator.filePath).Length;
                int count;

                if (openedFiles.TryGetValue(ftiterator.filePath, out count))
                {
                    count -= 1;
                    if (count == 0)
                    {
                        openedFiles.TryRemove(ftiterator.filePath, out count);
                        File.Delete(ftiterator.filePath);
                    }
                    else
                        openedFiles.TryAdd(ftiterator.filePath, count);
                }
            }


        }

        /**
         * Manages the duplicates if needed. It follows a Windows style pattern. 
        
         */

        private string GetUniqueFilePath(string filepath)
        {
            
            if (Directory.Exists(filepath))
            {
                int number = 1;
                string tempPath;
                do
                {
                    number++;
                    tempPath = Path.Combine(string.Format("{0} ({1})", filepath, number));
                }
                while (Directory.Exists(tempPath));
                return tempPath;
            }

            if (File.Exists(filepath))
            {

                string folder = Path.GetDirectoryName(filepath);
                string filename = Path.GetFileNameWithoutExtension(filepath);
                string extension = Path.GetExtension(filepath);
                int number = 1;


                Match regex = Regex.Match(filepath, @"(.+) \((\d+)\)\.\w+");        // Aggiunge un controllo aggiuntivo per essere certi che abbiamo davanti un file
                                                                                    // e per catturare eventualmente il valore progressivo già presente tra parentesi

                if (regex.Success)
                {
                    filename = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }

                do
                {
                    number++;
                    filepath = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, number, extension));
                }
                while (File.Exists(filepath));
            }

            return filepath;

        }

        public class JobFileIterator : FileIterator
        {
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
            public Job Job
            {
                get
                {
                    return job;
                }
                set
                {
                    job = value;
                }
            }

            /**
             * If any callback is registered to this delegate, the iterator
             * class gives the assurance that this callback will be executed 
             * before the onIteratorClosed() method of the StorageModule.
             */
            public delegate void BeforeIteraorClosedDel();
            public event BeforeIteraorClosedDel BeforeIteratorClosed;

            /**
             * Constructor.
             */
            public JobFileIterator(StorageModule s, string filePath) : base(s)
            {
                fileStream = File.Open(filePath, FileMode.OpenOrCreate);
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
            public override int next(byte[] buffer)
            {
                //long length = new System.IO.FileInfo(filePath).Length;
                long length = job.Task.Size;
                int read = 0;
                if (offset + READ_BLOCK_SIZE <= length)
                    read = fileStream.Read(buffer, 0, READ_BLOCK_SIZE);
                else
                {
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
            public override int write(byte[] buf, int count)
            {

                // How much bytes were written
                int written = 0;

                /* Size of the file in which execute the write operation.
                   This size is coherent with that one defined by the Task
                   included in the job associated to this iterator.
                   NOTE: This means that this is a logical dimension, not 
                   the size of the file on the file system. */
                long length = job.Task.Size;

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

                // Returns the number of bytes written.
                return written;
            }

            /**
             * Returns true if the iterator is not positioned to the end of the file,
             * false otherwise.
             */
            public override bool hasNext()
            {
                //return offset < new System.IO.FileInfo(filePath).Length;
                return offset < job.Task.Size;
            }

            public override void close()
            {
                // Close the filestream
                fileStream.Close();

                // If anyone is registered to the closing event
                // and requires to be executed befor the closing.
                if (BeforeIteratorClosed != null)
                    BeforeIteratorClosed();

                // Calls the closing method of the father
                base.close();
            }
        }

    }
}
