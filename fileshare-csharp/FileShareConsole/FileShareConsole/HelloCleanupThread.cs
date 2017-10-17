using System;
using System.Diagnostics;
using System.Threading;
using static HelloProtocol.PeersList;

namespace HelloProtocol {

    internal class HelloCleanupThread : ExecutableThread {

        public HelloCleanupThread() {

        }

        protected override void execute() {

            while (true) {
                // Sleep for a certain time interval
                Thread.Sleep(Settings.Instance.HELLO_CLEANUP_SLEEP_TIME);

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

                Logger.log(Logger.HELLO_DEBUG, "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\n"); // DEBUG
                Logger.log(Logger.HELLO_DEBUG, "CLEANUP THREAD WOKE UP\n"); // DEBUG
                Logger.log(Logger.HELLO_DEBUG, "KEY\t\tNAME\t\tADDRESS\n"); // DEBUG
                foreach (Peer p in peers.Peers)
                    Logger.log(Logger.HELLO_DEBUG, p.Id + "\t\t" + p.Name + "\t\t" + p.Ipaddress + "\n"); // DEBUG
                Logger.log(Logger.HELLO_DEBUG, "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\n"); // DEBUG

            }
        }

    }

}