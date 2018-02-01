using System.Threading;

namespace HelloProtocol {

    internal class HelloSenderThread : ExecutableThread {
        public static ManualResetEvent visibilityChange = new ManualResetEvent(!Settings.Instance.IsInvisible);

        /// <summary>
        /// The AutoResetEvent that we will use to implement the "sleep". We use an event so to
        /// be able to wake up the thread in every moment.
        /// </summary>
        public static AutoResetEvent WaitingEvent = new AutoResetEvent(false);

        /// <summary>
        /// Delegate that defines the prototype for a callback to invoke
        /// when the send on the network is not available
        /// </summary>
        public delegate void OnCannotSend();
        /// <summary>
        /// The event corresponding to the OnCannotSend delegate. Third parts
        /// can register their callbacks on this event.
        /// </summary>
        public event OnCannotSend CannotSend;

        /// <summary>
        /// Max attempts to send a packet befor considering network down.
        /// </summary>
        private const int MAX_SEND_ATTEMPTS = 10;

        /// <summary>
        /// The HelloNetworkModule instance used to access to the network functionalities.
        /// </summary>
        private HelloNetworkModule network;

        public HelloSenderThread(HelloNetworkModule network) {
            Settings.Instance.PropertyChanged += Instance_PropertyChanged;
            this.network = network;
        }

        // Qualora lo stato dovesse passare a invisibile blocca l'invio di keepAlive
        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "IsInvisible") {
                if ((sender as Settings).IsInvisible/*e.Equals(true)*/) {
                    visibilityChange.Reset();
                } else
                    visibilityChange.Set();
            }
        }

        protected override void execute() {
            int attempts = 0;   // a counter to take trace of consecutive attempts to send

            // I will continue while it will not be telled me to stop and eventually I will
            // stuck on the visibiliyChange event if someone calls Reset() on it.
            while (!Stop && visibilityChange.WaitOne()) {
                if (Stop)
                    break;  // quit
                // will store the result of the send() operation

                bool sendresult = false;

                while (!sendresult) {
                     sendresult = network.send(new KeepalivePacket(Settings.Instance.LocalPeer.Id,
                                                 Settings.Instance.LocalPeer.Name,
                                                 Settings.Instance.LocalPeer.Ipaddress));

                    if (sendresult) { // if the send() failed we don't want to wait, we want to try again
                        WaitingEvent.WaitOne(Settings.Instance.HELLO_INTERVAL);
                        attempts = 0;   // reset attempts
                    } else if (++attempts == MAX_SEND_ATTEMPTS) {
                        CannotSend(); // TODO Do something
                        return;
                    }
                }
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

