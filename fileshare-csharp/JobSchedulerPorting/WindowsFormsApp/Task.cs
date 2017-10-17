using System;

/// <summary>
/// Summary description for Class1
/// </summary>
/// 
namespace FileTransfer
{

    public class Task
    {
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
    }
}