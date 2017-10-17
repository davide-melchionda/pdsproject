using System;
using System.Collections.Concurrent;
using System.Threading;

namespace FileTransfer
{
    public class ToScheduleFifoQueue
    {
        //classe che implementa il paradigma singleton
        private static ToScheduleFifoQueue instance;

        public delegate void onPushCallbackType(Job job);
        public delegate void onPopCallbackType(Job job);

        //la classe espone due eventi ai quali è possibile registrare delle callback
        public event onPushCallbackType pushHappened;
        public event onPopCallbackType popHappened;

        private JobScheduler jobScheduler;
        public static BlockingCollection<Job> jobsFifoQueue;

        public static ToScheduleFifoQueue Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ToScheduleFifoQueue();
                }
                return instance;
            }
        }



        public Job pop() //Se non riesce a poppae nulla restituisce void e mette a dormire il consumer
        {
            Job temp = jobsFifoQueue.Take();
            new Thread(() => { popHappened(temp); }).Start();
            return temp;
        }

        public void push(Job toPush) //inserisce un nuovo job in coda, se si rende conto che la coda era precedentemente vuota risveglia il consumer
        {
            jobsFifoQueue.Add(toPush);
            new Thread(() => { pushHappened(toPush); }).Start();

        }
    }
}