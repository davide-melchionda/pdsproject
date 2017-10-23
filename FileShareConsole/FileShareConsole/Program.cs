using FileTransfer;
using HelloProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StorageModule;

namespace FileShareConsole
{
    class Program
    {
        static void Main(string[] args) {

            //new HelloThread().run();

            //new JobScheduler().run();

            JobZipStorageModule storage = new JobZipStorageModule();
            
            string s = @"C:\Users\vm-dm-win\Desktop\tmp.txt";
            string to = @"C:\Users\vm-dm-win\Desktop\tmpzipped"+DateTime.Now.Millisecond+@".zip";

            //ZipFile.CreateFromDirectory(s, to);

            //ZipArchive newFile = ZipFile.Open(to, ZipArchiveMode.Create);
            //string p = Path.GetDirectoryName(s);
            //newFile.CreateEntryFromFile(s, Path.GetFileName(s), CompressionLevel.NoCompression);
            //newFile.Dispose();

            //FileIterator i = storage.prepareFile(s, DateTime.Now);
            //FileIterator i = storage.prepareFile(@"C:\Users\vm-dm-win\Desktop\tmp", new DateTime());
            //System.Threading.Thread.Sleep(20000);

            //new JobScheduler().run();

            
            
            //new ServerClass(proto).run();


            PeersList.Instance.put(new Peer("you", "you", "127.0.0.1"));

            JobScheduler scheduler = new JobScheduler();
            scheduler.scheduleJob(new Job(new FileTransfer.Task("me","you",s),s));

            //i.close();

        }
    }
}
