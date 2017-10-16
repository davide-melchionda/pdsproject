using System.Threading;

namespace HelloProtocol {

    internal class HelloSenderThread : ExecutableThread {

        public HelloSenderThread() {

        }

        protected override void execute() {
            HelloNetworkModule network = HelloNetworkModule.Instance;
            while(true) {
                network.send(new KeepalivePacket(Settings.Instance.LocalPeer.Id,
                                                 Settings.Instance.LocalPeer.Name,
                                                 Settings.Instance.LocalPeer.Ipaddress));
                Thread.Sleep(Settings.Instance.HELLO_INTERVAL);
            }
        }
    }
}