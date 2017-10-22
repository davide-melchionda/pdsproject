using FileShareConsole;
using System;
using System.Threading;
<<<<<<< Updated upstream
=======
<<<<<<< Updated upstream
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
=======
<<<<<<< master
>>>>>>> Stashed changes
using static StorageModule;

namespace FileTransfer {
    public class JobScheduler {
        
        /**
         * Storage module which will be used to access to files
         */
        private StorageModule storage;

        /**
         * Constructor
         */
        public JobScheduler() {
            // Intantiate the storage managemet module he will use
            storage = new JobZipStorageModule();
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
>>>>>>> Stashed changes
        }
=======
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

        public Semaphore activeClientsPool = new Semaphore(15, 15); //semaforo che regola il threadpool di invio
>>>>>>> Packets hiercarchy and client hineritance

        /**
         * Given a job, executes its computation. The computation of a 
         * Job consist in the assignment of the job to a JobExecutor 
         * which has the purpose of execute the file tranfer according 
         * to a specific protocol.
         */
        public void scheduleJob(Job job) {

            // Requires memory management module to initialize the file on which
            // the job will work
            FileIterator iterator = ((JobZipStorageModule)storage).prepareJob(job);

            // Pushes the job in the list of in-service jobs
            JobsList.Instance.push(job);

            // Schedules a thread to send the packet
            JobExecutor sender = new JobExecutor(job, iterator);
            // When the transmissione ends
            sender.OnTrasmissionEnd += () => {
                iterator.close();   // Close the iterator
                                    // Removes the job from the active jobs list
                JobsList.Instance.remove(job.Id);
            };

            // Run the sender
            sender.run();
        }

    }
}