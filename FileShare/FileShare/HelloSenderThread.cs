using System.Threading;

namespace HelloProtocol {

    internal class HelloSenderThread : ExecutableThread {
        public static ManualResetEvent visibilityChange = new ManualResetEvent(!Settings.Instance.IsInvisible);

        /// <summary>
        /// The AutoResetEvent that we will use to implement the "sleep". We use an event so to
        /// be able to wake up the thread in every moment.
        /// </summary>
        public static AutoResetEvent WaitingEvent = new AutoResetEvent(false);

        public HelloSenderThread() {
            Settings.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        // Qualora lo stato dovesse passare a invisibile blocca l'invio di keepAlive
        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "IsInvisible") {
                if (e.Equals(true)) {
                    visibilityChange.Reset();
                } else
                    visibilityChange.Set();

            }
        }

        protected override void execute() {
            HelloNetworkModule network = HelloNetworkModule.Instance;
            while (!Stop && visibilityChange.WaitOne()) {
                if (Stop)
                    break;  // quit

                // will store the result of the send() operation
                bool sendresult = false;

                //lock (Settings.Instance) {
                    if (!Settings.Instance.IsInvisible)
                        sendresult = network.send(new KeepalivePacket(Settings.Instance.LocalPeer.Id,
                                                     Settings.Instance.LocalPeer.Name,
                                                     Settings.Instance.LocalPeer.Ipaddress));
                //}

                if (sendresult) // if the send() failed we don't want to wait, we want to try again
                    WaitingEvent.WaitOne(Settings.Instance.HELLO_INTERVAL);
            }
        }

        protected override void PrepareStop() {
            Stop = true;   // prepares itself to stop
            // now that the Stop flag is to false
            visibilityChange.Set(); // unlock itself if needed 
            WaitingEvent.Set();
        }
    }
}

