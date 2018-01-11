using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransfer {

    class JobsList {

        /**
         * The instance of the list of jobs for outgoing transfers
         */
        private static JobsList sending;
        /**
         * The object on which acquire the lock in order to create the instance
         */
        private static object syncRootSending = new Object();
        /**
         * instance property.
         * Through the get method it's possble to get a reference to the
         * unique instance for outgoing transfers of this cass.
         */
        public static JobsList Sending {
            get {
                if (sending == null) {
                    lock (syncRootSending) {
                        if (sending == null)
                            sending = new JobsList();
                    }
                }
                return sending;
            }
        }

        /**
         * The instance of the list of jobs for incoming transfers
         */
        private static JobsList receiving;
        /**
         * The object on which acquire the lock in order to create the instance
         */
        private static object syncRootReceiving = new Object();
        /**
         * instance property.
         * Through the get method it's possble to get a reference to the
         * unique instance for incoming transfers of this cass.
         */
        public static JobsList Receiving {
            get {
                if (receiving== null) {
                    lock (syncRootReceiving) {
                        if (receiving == null)
                            receiving = new JobsList();
                    }
                }
                return receiving;
            }
        }

        /**
         * A List<Jobs> which represents all jobs contained in the JobsList
         */
        public List<Job> Jobs {
            get {
                List<Job> jobs = new List<Job>();
                foreach (string s in list.Keys) {
                    jobs.Add(get(s));
                }
                return jobs;
            }
        }

        public delegate void OnJobAdded(Job job);
        public delegate void OnJobRemoved(Job job);

        //la classe espone due eventi ai quali è possibile registrare delle callback
        public event OnJobAdded JobAdded;
        public event OnJobRemoved JobRemoved;
        
        private ConcurrentDictionary<string, Job> list;

        public JobsList() {
            list = new ConcurrentDictionary<string, Job>();
        }

        public Job get(string key) //Se non riesce a poppae nulla restituisce void e mette a dormire il consumer
        {
            Job j = null;
            try {
                list.TryGetValue(key, out j);
            } catch (ArgumentNullException e) {
                Logger.log(Logger.EXCEPTION_LOGGING, e.Message);
                return null;
            }
            return j;
        }

        public void remove(string key) //Se non riesce a poppae nulla restituisce void e mette a dormire il consumer
        {
            Job j = null;
            try {
                list.TryRemove(key, out j);
            } catch (ArgumentNullException e) {
                throw e;
            }
            JobRemoved?.Invoke(j);
        }

        public void push(Job job) //inserisce un nuovo job in coda, se si rende conto che la coda era precedentemente vuota risveglia il consumer
        {
            list.TryAdd(job.Id, job);
            if (JobAdded != null)
                new Thread(() => { JobAdded(job); }).Start();

        }
        
    }

}
