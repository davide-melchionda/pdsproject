
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace HelloProtocol {
    internal class HelloThread : ExecutableThread {
        public delegate void ProfilePicUpdated(string peerId, byte[] newPicture);

        public event ProfilePicUpdated OnProfilePicUpdate;

        public delegate void OnNetworkCannotConnect();
        public event OnNetworkCannotConnect NetworkCannotConnect;

        /// <summary>
        /// Max number of attempts to send a packet if network fails the sent
        /// </summary>
        private const int MAX_NETFAIL_ATTEMPTS = 3;

        /// <summary>
        /// The HelloNetworkModule instance to access to the network functionalities
        /// </summary>
        private HelloNetworkModule network;

        /// <summary>
        ///  Constructor.
        ///  Retrieves the HelloNetworkModuleInstance
        /// </summary>
        public HelloThread() {
            network = HelloNetworkModule.Instance;
        }

        private void onPacketReceived(HelloPacket packet, String senderip) {
            // Only if we have the network we can process the packet
            if (network != null) {

                PeersList peers = PeersList.Instance;

                // Checks the type of the received packet
                if (packet.Type == HelloPacket.PacketType.Keepalive) {  // If it's a keepalive
                                                                        // Cast the received packet to a keepalive
                    KeepalivePacket keepalive = (KeepalivePacket)packet;
                    // Retrieves the local peer from the Settings
                    Peer localPeer = Settings.Instance.LocalPeer;

                    // If the packet come from local host ...
                    if (localPeer.Id == keepalive.PeerId || false) {
                        // ... and local ip address is not updated ...
                        if (localPeer.Ipaddress != senderip)
                            // ... update local ip address
                            Settings.Instance.updatePeerAddress(senderip);
                    } else if (peers.get(keepalive.PeerId) == null) {     // otherwise if the packet come from an unknown peer
                        int attempts = 0;   // attempts to send the packet
                                            // Try to send a query packet for MAX_NETFAIL_ATTEMPTS times
                        while (!network.sendUnicast(new QueryPacket(), senderip)) {
                            if (++attempts > MAX_NETFAIL_ATTEMPTS) {
                                NetworkCannotConnect?.Invoke(); // failed: calling the delegate
                                break;  // we failed to manage the packet; we will return after this break
                            }
                            System.Threading.Thread.Sleep(3000);
                        }
                    } else   // otherwise update the timestamp of the peer in the peers table
                        peers.updatePeer(keepalive.PeerId, DateTime.Now, keepalive.PeerName);
                } else if (packet.Type == HelloPacket.PacketType.Query) {  // If it's a query
                                                                           //lock (Settings.Instance) {
                    if (!Settings.Instance.IsInvisible) {
                        // Send a presentation packet to the peer who is requiring information
                        int attempts = 0;   // attempts to send the packet
                        // Try to send a presentation packet for MAX_NETFAIL_ATTEMTPS times
                        while (!network.sendUnicast(new PresentationPacket(Settings.Instance.LocalPeer), senderip)) {
                            if (++attempts > MAX_NETFAIL_ATTEMPTS) {
                                NetworkCannotConnect?.Invoke(); // failed: calling the delegate
                                break;  // we failed to manage the packet; we will return after this break
                            }
                            System.Threading.Thread.Sleep(3000);
                        }
                    }
                    //}
                } else if (packet.Type == HelloPacket.PacketType.Presentation) {  // If it's a presentation
                    Peer localPeer = Settings.Instance.LocalPeer;
                    PresentationPacket presentation = (PresentationPacket)packet;
                    //Don't accept presentation from myself
                    if (presentation.Peer.Id != localPeer.Id) {
                        // If the peer is unknown
                        if (peers.get(presentation.Peer.Id) == null)
                            // Put the peer in the peers table
                            peers.put(presentation.Peer);
                        OnProfilePicUpdate?.Invoke(presentation.Peer.Id, presentation.Peer.ByteIcon);
                    }
                } else if (packet.Type == HelloPacket.PacketType.GoodBye) {  // If it's a GoodBye
                    Peer localPeer = Settings.Instance.LocalPeer;
                    GoodByePacket goodBye = (GoodByePacket)packet;
                    //Don't accept goodBye from myself
                    if (goodBye.PeerId != localPeer.Id) {
                        // I know the sender: I must remove it from the PeersList
                        if (peers.get(goodBye.PeerId) != null)
                            peers.del(goodBye.PeerId);      //remove the peer from my peersList
                    }
                }
            } else {
                NetworkCannotConnect?.Invoke();
            }
        }

        protected override void execute() {

            // If we have no network, we can return
            if (network == null) {
                NetworkCannotConnect?.Invoke();
                return;
            }

            // Obtain a reference to the peers table
            PeersList peers = PeersList.Instance;

            // Run a thread dedicated to the sending of the
            // hello packets (one each 20 sec.)
            HelloSenderThread sender = new HelloSenderThread(network);
            RegisterChild(sender);  // a new child wa created
            sender.CannotSend += () => {
                //network.Reset();
                NetworkCannotConnect?.Invoke();
            };
            sender.run();

            // Run a thread dedicated to the cleaning of the
            // peers table.
            HelloCleanupThread cleanup = new HelloCleanupThread();
            RegisterChild(cleanup);  // a new child wa created
            cleanup.run();

            network.HelloPacketReception += onPacketReceived;
            Settings.Instance.PropertyChanged += (object s, System.ComponentModel.PropertyChangedEventArgs e) => {
                if (String.Compare(e.PropertyName, "CurrentUsername") == 0 || String.Compare(e.PropertyName, "PicturePath") == 0)
                    if (!Settings.Instance.IsInvisible) // Send a presentation packet only if not invisible
                        SendPresentationPacket(/*s, e*/);
            };
            Settings.Instance.PropertyChanged += Instance_visibilityChanged;

            while (!Stop && network.receive())
                ;

            /*
             * Should we close the socket?
             * It's not correct to close the socket, because the object
             * HelloNetworkModule is a Singleton, 
             *      network.closeSockets();
             */

        }

        private void SendPresentationPacket(/*object sender, System.ComponentModel.PropertyChangedEventArgs e*/) {
            //HelloNetworkModule network = HelloNetworkModule.Instance;

            int attempts = 0;   // attempts to send presentation

            // Tries to send the PresentationPacket for N times. If it fails, calls the relative callback
            while (!network.send(new PresentationPacket(Settings.Instance.LocalPeer))) {
                if (++attempts > MAX_NETFAIL_ATTEMPTS) {
                    NetworkCannotConnect?.Invoke(); // failed: calling the delegate
                    return;
                }
                System.Threading.Thread.Sleep(3000);
            }
        }

        private void Instance_visibilityChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (String.Compare(e.PropertyName, "IsInvisible") == 0) {
                if (Settings.Instance.IsInvisible)
                    sendGoodbye();
                else
                    SendPresentationPacket();
            }
        }

        private void sendGoodbye() {
            int attempts = 0;   // attempts to send goodbye

            // Tries to send the PresentationPacket for N times. If it fails, calls the relative callback
            while (!network.send(new GoodByePacket(Settings.Instance.LocalPeer.Id))) {
                if (++attempts > MAX_NETFAIL_ATTEMPTS) {
                    NetworkCannotConnect?.Invoke();     // failed: lets call delegate
                    return;
                }
                System.Threading.Thread.Sleep(3000);
            }
        }

        protected override void PrepareStop() {
            // immediately after have killed the childs we want
            ChildsGone += () => {
                if (network != null) {
                    sendGoodbye();  //... 1) to send a Goodbye packet
                    //network.closeSockets(); // ... 2) to make the network module release all resources
                    network.Reset();
                }
            };
        }
    }
}
