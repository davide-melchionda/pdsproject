using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransfer;
using System.IO.Compression;
using System.Collections.Concurrent;

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
        static object dictionaryLock = new object();

        public JobZipStorageModule() {
            openedFiles = new ConcurrentDictionary<string, int>();
        }

        public FileIterator prepareJob(Job j) {
            string filePath = j.FilePath;   // Path of the file
            DateTime lastModify = j.Task.RequestTimestamp;  // The timestamp

            Logger.log(Logger.ZIP_DEBUG, "Called to zip " + filePath+ "\n");

            // Constructs the zip file name
            string date = lastModify.ToString("yyyyMMddhhmmss")+lastModify.Millisecond;
            string zipName = filePath + date + ".zip";

            // If the file was modified after the lastModify date, throws an exception
            if (File.GetLastWriteTime(filePath) > lastModify) {
                throw new FileVersioningException("The file was modified after the indicated time");
            }

            // Check zipped file existence
            if (!File.Exists(zipName)) {
                
                // If the file was not zipped yet
                if (!openedFiles.ContainsKey(filePath)) {

                    /* Retrieves the file attributes to check if the file is a directory or not
                        because the way to zip is different in the two cases. */
                    FileAttributes attr = 0;
                    attr = File.GetAttributes(filePath);

                    // If the file is a directory
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        // zip directory
                        ZipFile.CreateFromDirectory(filePath, zipName);
                    else {  // otherwise
                        // Note: if it's a file, ZipArchive has to work on a new file in order to zip
                        ZipArchive newFile = ZipFile.Open(zipName, ZipArchiveMode.Create);
                        string p = Path.GetDirectoryName(filePath);
                        newFile.CreateEntryFromFile(filePath, Path.GetFileName(filePath), CompressionLevel.NoCompression);
                        newFile.Dispose();
                    }
                }

                // Intializes the remaining info in the task (SentName and Size)
                j.Task.SentName = Path.GetFileName(zipName);    // name of the zipped file
                j.Task.Size = new System.IO.FileInfo(zipName).Length;     // size of the zipped file


                // Returns an iterator to the file
                JobFileIterator jobIterator = (JobFileIterator) getIterator(zipName);
                jobIterator.Job = j;
                return jobIterator;
            }

            // File not found
            return null;

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

        protected override void onIteratorClosed(FileIterator iterator) {

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
                        openedFiles.TryAdd(ftiterator.filePath, count - 1);
                }
            }
        }

        public class JobFileIterator : FileIterator
        {
            private FileStream fileStream;
            private int offset;
            public string filePath;
            private Job job;
            public Job Job {
                get {
                    return job;
                }
                set {
                    job = value;
                }
            }

            public JobFileIterator(StorageModule s, string filePath) : base(s) {
                fileStream = File.OpenRead(filePath);
                this.filePath = filePath;
                offset = 0;
            }

            public override int next(byte[] buffer) {
                long length = new System.IO.FileInfo(filePath).Length;
                int read = 0;
                if (offset + READ_BLOCK_SIZE <= length)
                    read = fileStream.Read(buffer, offset, READ_BLOCK_SIZE);
                else {
                    int toRead = (int)length - offset;
                    read = fileStream.Read(buffer, offset, toRead); // no-overflow
                }
                job.SentByte += read;
                offset += read;
                return read;
            }

            public override bool hasNext() {
                return offset < new System.IO.FileInfo(filePath).Length;
            }
            
            public override void close() {
                fileStream.Close();
                base.close();
            }
        }

    }
}
