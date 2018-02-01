using System;
using System.Diagnostics;
using System.Threading;
using static HelloProtocol.PeersList;

namespace HelloProtocol {

    internal class HelloCleanupThread : ExecutableThread {

        private AutoResetEvent sleepHandle = new AutoResetEvent(false);

        public HelloCleanupThread() {}

        protected override void execute() {

            while (!Stop) {
                // Sleep for a certain time interval or while not waken up
                sleepHandle.WaitOne(Settings.Instance.HELLO_CLEANUP_SLEEP_TIME);

                if (Stop)   
                    break;  // quit

                // The instant he woke up
                //DateTime wakeup = new DateTime();
                DateTime wakeup = DateTime.Now;

                // Retrieves the peers list
                PeersList peers = PeersList.Instance;

                // Checks if anyone of the peers entry has to be removed
                foreach (PeerEntry entry in peers.Rows) {
                    // Computes the number of ticks (100ns) the peer entry has not been 
                    // updated in the peers list.
                    TimeSpan notupdated = wakeup - entry.Timestamp;
                    TimeSpan t = new TimeSpan(TimeSpan.TicksPerMillisecond * Settings.Instance.MAXAGE_MILLIS);
                    // If this time is over a cetain threshold
                    if (notupdated.TotalMilliseconds > t.TotalMilliseconds) {
                        // Removes the entry from the peers list
                        peers.del(entry.Peer.Id);
                    }
                }

            }
        }

        protected override void PrepareStop() {
            Stop = true;
            sleepHandle.Set();
        }
    }

}