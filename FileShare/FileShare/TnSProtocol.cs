using System;
using NetProtocol;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace FileTransfer
{
    internal class TnSProtocol : Protocol
    {

        /**
         * The semaphore which manages fixed threshold in client scheduling.
         */
        private static Semaphore clientsPool = new Semaphore(20, 20);

        /**
         * The semaphore which manages the high level threshold in server scheduling.
         */
        private static Semaphore serversHighThreshold = new Semaphore(15, 15);

        /**
         * The number which represents the low level threshold in server scheuling.
         */
        private const int serversLowThreshold = 10;

        /**
         * The semaphore which manages the low level threshold in server scheduling.
         */
        //private static Semaphore serversLowThreshold = new Semaphore(5, 5);

        /**
         * This dictionary (which is thread safe) will contain pairs id - number_of_active_servers,and can be checked
         * to implement the logic of the two level threshold pooling system for server scheduling.
         * A server thread can start its work only if there are less than HIGH_LEVEL_THRESHOLD active server threads
         * adn one of this two condition is verified:
         * 1) There are less than LOW_LEVEL_THRESHOLD server threads active.
         * 2) There are more than LOW_LEVEL_THRESHOLD server threads active but no one of this has the same id of
         * the curret server thread.
         */
        private static ConcurrentDictionary<string, int> activeServersCount = new ConcurrentDictionary<string, int>();

        /**
         * The lock which must be acquired to execute complex operations on the activeServersCount dictionary.
         */
        private static object dictionaryLock = new object();

        /**
         * This event will represent the completion of a task by a thread.
         */
        AutoResetEvent threadFinishedEvent = new AutoResetEvent(false);

        public override void enterClient()
        {
            clientsPool.WaitOne();
        }

        public override void releaseClient()
        {
            clientsPool.Release();
        }

        /**
         * Called by a server thread which is trying to be scheduled to execute it's work.
         * The call blocks the thread while the two level threshold pooling system doesn't 
         * admit him in the set of working thread.
         */
        public override void enterServer(string connectionId) {
            
            // Wile admitted will not be true, this method will not return
            bool admitted = false;
            
            // Continue while you are not admitted to work. The body of this cycle
            // will be executed by a thread the first time he tries to access to the
            // pool and then each time an active thread ends its work.
            while (!admitted) {
                
                // Try to pass the high level threshold
                if (serversHighThreshold.WaitOne(0)) {

                    lock (dictionaryLock) {
                        int count = 0;
                        foreach (string key in activeServersCount.Keys) {
                            int val;
                            if (activeServersCount.TryGetValue(key, out val))
                                count += val;
                        }
                        // Try to pass the low level threshold or check if you are the only one
                        // server thread with this id in the pool
                        if (count < serversLowThreshold || !dictionaryExists(connectionId)) {

                            // Increment the number of references to this id in the dictionary
                            dictionaryValueIncrement(connectionId);
                            admitted = true;    // Admitted to work. Works like a 'break'
                        } else
                            serversHighThreshold.Release(); // Not working, so don't occupy a slot
                    }
                }
                if (!admitted) // If the work was not completeds
                    threadFinishedEvent.WaitOne();  // Wait for the end of any other active thread
            }
        }

        /**
         * Called by a server thread which has finished its work. Frees a slot in the thread
         * pool updating the activeServersCount and waking up the waiting threads.
         */
        public override void releaseServer(string connectionId) {
            serversHighThreshold.Release();
            dictionaryValueDecrement(connectionId);   // Decrements the count of active threads with the given id
            threadFinishedEvent.Set();  // Signals to waiting thread the event of finishing
        }

        /**
         * Check if an entry eists in the dictionary for the given id.
         */
         private bool dictionaryExists(string id) {
            return activeServersCount.ContainsKey(id);  // Threa safe method (by ConcurrentDictionary)
        }

        /**
         * Increment the value of the element with key 'id' in the dictionary.
         */
         private void dictionaryValueIncrement(string id) {
            activeServersCount.AddOrUpdate(id, 1, (key, value) => value + 1);
        }

        /**
         * Decrement the value of the element with key 'id' in the dictionary.
         */
        private void dictionaryValueDecrement(string id) {
            int count;
            lock (dictionaryLock) {
                if (activeServersCount.TryGetValue(id, out count)) {
                    count -= 1;
                    if (count == 0) {
                        int removed;
                        activeServersCount.TryRemove(id, out removed);
                    } else
                        activeServersCount.TryUpdate(id, count, count+1);
                }
            }
        }
    }
}
    