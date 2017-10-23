using System;
using System.IO;

using System.Collections.Concurrent;
using System.IO.Compression;

/// <summary>
//SendMemoryManagementModule - Subclass del modulo di memoria
/// </summary>
/// 
namespace FileTransfer

{
    //public class SendMemoryModule : MemoryModule

    //{

    //    public static int READ_BLOCK_SIZE = 1024;
    //    private ConcurrentDictionary<string, int> openedFiles;
    //    static object dictionaryLock = new object();

    //    public SendMemoryModule()
    //    {
    //        this.openedFiles = new ConcurrentDictionary<string, int>();
    //    }

    //    public FileIterator getFileIterator(String name, string filePath, FileInfo.TypeEnum whatIs, DateTime lastModify)
    //    {
    //        string zipName = string.Concat(name, lastModify.ToString("yyyyMMdd"));
    //        Console.WriteLine("lo zip avrà nome: "+zipName+ "\n");

    //        if (!File.Exists(string.Concat(filePath, zipName)))
    //        { //il file non è already cached
    //            //detect whether its a directory or file
    //            if (whatIs == FileInfo.TypeEnum.directory)
    //            {
    //                Console.WriteLine(zipName+ " è una directory\n");

    //                //zippo cartella
    //                ZipFile.CreateFromDirectory(filePath+name, string.Concat(filePath, zipName), CompressionLevel.Optimal, true);
    //            }
    //            else
    //            {
    //                //zippo file
    //                Console.WriteLine(zipName + " è un file\n");

    //                ZipArchive newFile = ZipFile.Open(string.Concat(filePath, zipName), ZipArchiveMode.Create);
    //                newFile.CreateEntryFromFile(filePath, Path.GetDirectoryName(filePath), CompressionLevel.Fastest);
    //                newFile.Dispose();
    //            }
    //        }

    //        // the file is already cached
    //        Console.WriteLine(zipName + " esiste già\n");

    //        //long fileLength = new file.FileInfo(filePath + zipName).Length;
    //        int count;
    //        lock (dictionaryLock)
    //        {
    //            if (openedFiles.TryGetValue(filePath + zipName, out count))
    //            {
    //                openedFiles.TryAdd(filePath + zipName, count + 1);
    //            }
    //        }
    //        FileIterator i = new FileIterator(File.OpenRead(filePath + zipName));
    //        i.Closed += () =>
    //        {
    //            lock (dictionaryLock)
    //            {
    //                if (openedFiles.TryGetValue(filePath + zipName, out count))
    //                {
    //                    count -= 1;
    //                    if (count == 0)
    //                    {
    //                        openedFiles.TryRemove(filePath + zipName, out count);
    //                        File.Delete(filePath + zipName);
    //                    }
    //                    else
    //                        openedFiles.TryAdd(filePath + zipName, count - 1);
    //                }
    //            }
    //        };
    //        Console.WriteLine("puoi trovare lo zip in "+ filePath+"\n");

    //        return i;

    //    }
    //    //public static long DirSize(DirectoryInfo d)
    //    //{
    //    //    long size = 0;
    //    //    FileInfo[] fis = d.GetFiles();
    //    //    foreach (FileInfo fi in fis)
    //    //        size += fi.Length;

    //    //    DirectoryInfo[] dis = d.GetDirectories();
    //    //    foreach (DirectoryInfo di in dis)
    //    //        size += DirSize(di);
    //    //    return size;
    //    //}
    //}
    //public class FileIterator
    //{
    //    private FileStream fileStream;
    //    public int offset;
    //    public delegate void OnClose();
    //    public event OnClose Closed;

    //    //viene restituito ad un client e fornisce astrazione verso il file reale da inviare
    //    public FileIterator(FileStream fs)
    //    {
    //        this.fileStream = fs;
    //        this.offset = 0;
    //    }

    //    public int next(byte[] buffer)
    //    {
    //        fileStream.Read(buffer, offset, SendMemoryModule.READ_BLOCK_SIZE);
    //        return 0;
    //    }

    //    public bool hasNext()
    //    {
    //        return true;
    //    }
    //    public void close()
    //    {
    //        this.fileStream.Close();
    //        Closed();
    //    }
    //}

}