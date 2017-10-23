using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransfer {

    class JobsList {

        //classe che implementa il paradigma singleton
        private static JobsList instance;
        /**
         * instance property.
         * Through the get method it's possble to get a reference to the
         * unique instance of this cass.
         */
        public static JobsList Instance {
            get {
                if (instance == null) {
                    instance = new JobsList();
                }
                return instance;
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

        public Job remove(string key) //Se non riesce a poppae nulla restituisce void e mette a dormire il consumer
        {
            Job j = null;
            try {
                list.TryRemove(key, out j);
            } catch (ArgumentNullException e) {
                return null;
            }
            return j;
        }

        public void push(Job job) //inserisce un nuovo job in coda, se si rende conto che la coda era precedentemente vuota risveglia il consumer
        {
            list.TryAdd(job.Id, job);
            if (JobAdded != null)
                new Thread(() => { JobAdded(job); }).Start();

        }
        
    }

}
