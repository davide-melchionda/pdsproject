using System;

/// <summary>
/// Summary description for Class1
/// </summary>
/// 
namespace FileTransfer
{

    public class Task
    {
      
        public FileInfo informations;       //oggetto information contiene le info sul file originario
        public string sender { get; set; }
        public string receiver { get; set; }
        public long size { get; set; }            //campi riempiti dal modulo di memoria e utilizzati nell'ambito della trasmissione
}
}