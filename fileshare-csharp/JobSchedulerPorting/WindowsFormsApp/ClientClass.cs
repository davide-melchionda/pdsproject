using System;
using System.Threading;
/// <summary>
//Il singolo Client si occupa di inviare i dati interfacciandosi al MemoryManagementModule(non accedendo direttamente al file). E non sa che il file Ã¨
//in realtÃ zippato.
//Il Client potrebbe ricevere dal JobScheduler un'istanza di un FileIterator, che Ã¨ una classe associata al MemoryManagementModule che consente di muoversi
//all'interno di uno specifico file.
//Il MemoryManagementModule perÃ² vuole tener traccia del numero di istanze di FileIterator che ha fornito (un FileIterator potrebbe essere ad esempio
//ottenibile solo tramite un metodo NON STATICO di MemoryManagementModule), cosÃ¬ da poter eliminare lo zip quando nessuno sta piÃ¹ inviando nulla.
//La info puÃ² arrivare al MemoryManagement tramite una chiamata di un meotdo sul FileIterator o lanciando un eventi.
//Il Client ha dunque la responsabilitÃ di comunicare degli eventi ad altri componenti.Gli eventi da comunicare potrebbero essere:
//	1. Ho terminato un invio correttamente.
//	2. Ho terminato un invio fallendo.

//Il JobScheduler ad esempio Ã¨ interassato alla terminazione di un invio, perchÃ© grazie a questa informazione puÃ² decidere se schedulare nuovi
//client o meno in conformitÃ con quanto definito dal protocollo di trasferimento (che fissa delle soglie per evitare la congestione della rete).
/// </summary>
/// 
namespace FileTransfer
{
    public class ClientClass : ExecutableThread
    {
        private Task fileToSend;
        public static Semaphore activeClientsPool = new Semaphore(15, 15);
        public ClientClass(Task toBePerformed)
        {
            this.fileToSend = toBePerformed;
        }
  

        protected override void execute()
        {
            
            //invio al server della request che incapsula il task

            /*il client ottiene risposta affermativa dal server --> */
            activeClientsPool.WaitOne();
            //il client fa altre cose
            activeClientsPool.Release();
        }

    }
}