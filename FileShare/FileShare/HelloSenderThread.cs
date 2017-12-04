using System.Threading;

namespace HelloProtocol {

    internal class HelloSenderThread : ExecutableThread {
        public static ManualResetEvent visibilityChange = new ManualResetEvent(!Settings.Instance.IsInvisible);
        
        public HelloSenderThread()
        {
            Settings.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        // Qualora lo stato dovesse passare a invisibile blocca l'invio di keepAlive
        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsInvisible")
            {
                if (e.Equals(true))
                {
                    visibilityChange.Reset();
                }
                else visibilityChange.Set();

            }
        }

        protected override void execute()
        {
            HelloNetworkModule network = HelloNetworkModule.Instance;
            while(visibilityChange.WaitOne())
            {
                lock (Settings.Instance)
                {
                    if(!Settings.Instance.IsInvisible)
                    network.send(new KeepalivePacket(Settings.Instance.LocalPeer.Id,
                                                     Settings.Instance.LocalPeer.Name,
                                                     Settings.Instance.LocalPeer.Ipaddress));
                }
                Thread.Sleep(Settings.Instance.HELLO_INTERVAL);
            }
        }
    }
}

     