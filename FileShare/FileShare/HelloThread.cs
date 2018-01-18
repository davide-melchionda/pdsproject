
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace HelloProtocol {
    internal class HelloThread : ExecutableThread {
        public delegate void ProfilePicUpdated(string peerId, byte[] newPicture);

        public event ProfilePicUpdated OnProfilePicUpdate;

        public HelloThread() {
            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

        }

        private void onPacketReceived(HelloPacket packet, String senderip) {

            Logger.log(Logger.HELLO_DEBUG, "=======================================================================\n"); // DEBUG

            PeersList peers = PeersList.Instance;
            HelloNetworkModule network = HelloNetworkModule.Instance;

            // Checks the type of the received packet
            if (packet.Type == HelloPacket.PacketType.Keepalive) {  // If it's a keepalive
                // Cast the received packet to a keepalive
                KeepalivePacket keepalive = (KeepalivePacket)packet;
                // Retrieves the local peer from the Settings
                Peer localPeer = Settings.Instance.LocalPeer;

                Logger.log(Logger.HELLO_DEBUG, "Packet received from " + keepalive.PeerId + ". Type: " + packet.Type + "\n"); // DEBUG

                // If the packet come from local host ...
                if (localPeer.Id == keepalive.PeerId || false) {
                    Logger.log(Logger.HELLO_DEBUG, "I'm the sender!\n"); // DEBUG
                    // ... and local ip address is not updated ...
                    if (localPeer.Ipaddress != senderip) {
                        Logger.log(Logger.HELLO_DEBUG, "My address is wrong! I'll update it\n"); // DEBUG
                        // ... update local ip address
                        Settings.Instance.updatePeerAddress(senderip);
                    }
                } // otherwise if the packet come from an unknown peer
                else if (peers.get(keepalive.PeerId) == null) {
                    Logger.log(Logger.HELLO_DEBUG, "I don't know the sender. I'll send a Query!\n"); // DEBUG
                    // Send a query packet
                    network.sendUnicast(new QueryPacket(), senderip);
                } else {    // otherwise update the timestamp of the peer in the peers table
                    Logger.log(Logger.HELLO_DEBUG, "I already know the sender!\n"); // DEBUG
                    //peers.updateTimestamp(keepalive.PeerId, new DateTime());
                    peers.updatePeer(keepalive.PeerId, DateTime.Now, keepalive.PeerName);
                }
            } else if (packet.Type == HelloPacket.PacketType.Query) {  // If it's a query
                lock (Settings.Instance) {
                    if (!Settings.Instance.IsInvisible) {
                        Logger.log(Logger.HELLO_DEBUG, "Packet received from " + senderip + ". Type: " + packet.Type + "\n"); // DEBUG
                        Logger.log(Logger.HELLO_DEBUG, "I'll send a Presentation packet!\n"); // DEBUG
                                                                                              // Send a presentation packet to the peer who is requiring information
                        network.sendUnicast(new PresentationPacket(Settings.Instance.LocalPeer), senderip);
                    }
                }
            } else if (packet.Type == HelloPacket.PacketType.Presentation) {  // If it's a presentation
                Peer localPeer = Settings.Instance.LocalPeer;
                PresentationPacket presentation = (PresentationPacket)packet;
                Logger.log(Logger.HELLO_DEBUG, "Packet received from " + presentation.Peer.Id + ". Type: " + packet.Type + "\n"); // DEBUG
                //Don't accept presentation from myself
                if (presentation.Peer.Id != localPeer.Id) {
                    // If the peer is unknown

                    if (peers.get(presentation.Peer.Id) == null) {
                        Logger.log(Logger.HELLO_DEBUG, "I don't know the sender. I'll add him in the peers table!\n"); // DEBUG
                                                                                                                       // Put the peer in the peers table
                        peers.put(presentation.Peer);
                    } else
                        Logger.log(Logger.HELLO_DEBUG, "I already know the sender. Why does he sent me this packet?\n"); // DEBUG

                    OnProfilePicUpdate?.Invoke(presentation.Peer.Id, presentation.Peer.ByteIcon);
                    //peers.get(presentation.Peer.Id).ByteIcon = presentation.Peer.ByteIcon;
                }
                Logger.log(Logger.HELLO_DEBUG, "UPDATED PEERS LIST\n"); // DEBUG
                Logger.log(Logger.HELLO_DEBUG, "KEY\t\tNAME\t\tADDRESS\n"); // DEBUG
                foreach (Peer p in peers.Peers)
                    Logger.log(Logger.HELLO_DEBUG, p.Id + "\t\t" + p.Name + "\t\t" + p.Ipaddress + "\n"); // DEBUG
            } else if (packet.Type == HelloPacket.PacketType.GoodBye) {  // If it's a GoodBye
                Peer localPeer = Settings.Instance.LocalPeer;
                GoodByePacket goodBye = (GoodByePacket)packet;
                Logger.log(Logger.HELLO_DEBUG, "Packet received from " + goodBye.PeerId + ". Type: " + packet.Type + "\n"); // DEBUG
                //Don't accept goodBye from myself
                if (goodBye.PeerId != localPeer.Id) {
                    // If the peer is unknown i didn't know the sender, no action required

                    if (peers.get(goodBye.PeerId) == null)

                        Logger.log(Logger.HELLO_DEBUG, "I don't know the sender. I'll do nothing!\n"); // DEBUG


                    else {
                        Logger.log(Logger.HELLO_DEBUG, "I know the sender. I need to remove him from my online peers!\n"); // DEBUG
                        peers.del(goodBye.PeerId);                                                                      //remove the peer from my peersList
                    }
                    Logger.log(Logger.HELLO_DEBUG, "UPDATED PEERS LIST\n"); // DEBUG
                    Logger.log(Logger.HELLO_DEBUG, "KEY\t\tNAME\t\tADDRESS\n"); // DEBUG
                    foreach (Peer p in peers.Peers)
                        Logger.log(Logger.HELLO_DEBUG, p.Id + "\t\t" + p.Name + "\t\t" + p.Ipaddress + "\n"); // DEBUG
                }
            }
        }

        protected override void execute() {
            // Obtaine a reference to the peers table
            PeersList peers = PeersList.Instance;

            // Instantiate a network module
            HelloNetworkModule network = HelloNetworkModule.Instance;

            // Run a thread dedicated to the sending of the
            // hello packets (one each 30 sec.)
            HelloSenderThread sender = new HelloSenderThread();
            RegisterChild(sender);  // a new child wa created
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
            HelloNetworkModule network = HelloNetworkModule.Instance;

            //if (String.Compare(e.PropertyName, "CurrentUsername") == 0 || String.Compare(e.PropertyName, "PicturePath") == 0)
            while (!network.send(new PresentationPacket(Settings.Instance.LocalPeer)))
                System.Threading.Thread.Sleep(3000);
        }

        private void Instance_visibilityChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            HelloNetworkModule network = HelloNetworkModule.Instance;

            if (String.Compare(e.PropertyName, "IsInvisible") == 0) {
                if (Settings.Instance.IsInvisible)
                    sendGoodbye();
                else
                    SendPresentationPacket();
            }
        }

        //public void OnProcessExit(object sender, EventArgs e) {
        //    sendGoodbye();
        //}

        private void sendGoodbye() {
            HelloNetworkModule network = HelloNetworkModule.Instance;
            while (!network.send(new GoodByePacket(Settings.Instance.LocalPeer.Id)))
                System.Threading.Thread.Sleep(3000);
        }

        protected override void PrepareStop() {
            // immediately after have killed the childs we want
            ChildsGone += () => {
                sendGoodbye();  //... 1) to send a Goodbye packet
                HelloNetworkModule.Instance.closeSockets(); // ... 2) to make the network module release all resources
            };
        }
    }
}
