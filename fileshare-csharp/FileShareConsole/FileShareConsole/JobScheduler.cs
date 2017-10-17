using System;
using System.Threading;
/// <summary>
//Il JobScheduler:
//1. Verifica se vi sono Job nella TO_SCHEDULE_FIFO_QUEUE.Se non ce ne sono si mette in wait() in attesa di un inserimento.
//2. Recupera un Job.
//3. Va dal MemoryManagementModule e gli chiede di inizializzare l'operazione di invio chiamando getFileIterator(). Se riceve null o cattura un'eccezione....
//4. Inserisce il Job nella JOBS_LIST.
//5. Avvio un nuovo thread e passagli il riferimento al FileIterator, il riferimento al Job nella JOBS_LIST e quanto serve per inviare.

/// </summary>
/// 
namespace FileTransfer
{

    public class JobScheduler : ExecutableThread
    {
        protected JobScheduler()
        {
        }

        protected override void execute()
        {
            //qui il funzionamento
        }

    
    }
}