
using System;
using System.Diagnostics;

namespace HelloProtocol {
    internal class HelloThread : ExecutableThread {
        public HelloThread() {

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
                if (localPeer.Id == keepalive.PeerId) {
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
                }
                else {    // otherwise update the timestamp of the peer in the peers table
                    Logger.log(Logger.HELLO_DEBUG, "I already know the sender!\n"); // DEBUG
                    //peers.updateTimestamp(keepalive.PeerId, new DateTime());
                    peers.updateTimestamp(keepalive.PeerId, DateTime.Now);
                }
            } else if (packet.Type == HelloPacket.PacketType.Query) {  // If it's a query
                Logger.log(Logger.HELLO_DEBUG, "Packet received from " + senderip + ". Type: " + packet.Type + "\n"); // DEBUG
                Logger.log(Logger.HELLO_DEBUG, "I'll send a Presentation packet!\n"); // DEBUG
                // Send a presentation packet to the peer who is requiring information
                network.sendUnicast(new PresentationPacket(Settings.Instance.LocalPeer), senderip);
            } else if (packet.Type == HelloPacket.PacketType.Presentation) {  // If it's a presentation
                PresentationPacket presentation = (PresentationPacket)packet;
                Logger.log(Logger.HELLO_DEBUG, "Packet received from " + presentation.Peer.Id + ". Type: " + packet.Type + "\n"); // DEBUG
                // If the peer is unknown
                if (peers.get(presentation.Peer.Id) == null) {
                    Logger.log(Logger.HELLO_DEBUG, "I don't know the sender. I'll add him in the peers table!\n"); // DEBUG
                    // Put the peer in the peers table
                    peers.put(presentation.Peer);
                } else
                    Logger.log(Logger.HELLO_DEBUG, "I already know the sender. Why does he sent me this packet?\n"); // DEBUG
            }
            Logger.log(Logger.HELLO_DEBUG, "UPDATED PEERS LIST\n"); // DEBUG
            Logger.log(Logger.HELLO_DEBUG, "KEY\t\tNAME\t\tADDRESS\n"); // DEBUG
            foreach (Peer p in peers.Peers)
                Logger.log(Logger.HELLO_DEBUG, p.Id + "\t\t" + p.Name + "\t\t" + p.Ipaddress + "\n"); // DEBUG
        }

        protected override void execute() {
            // Obtaine a reference to the peers table
            PeersList peers = PeersList.Instance;

            // Instantiate a network module
            HelloNetworkModule network = HelloNetworkModule.Instance;

            // Run a thread dedicated to the sending of the
            // hello packets (one each 30 sec.)
            HelloSenderThread sender = new HelloSenderThread();
            sender.run();

            // Run a thread dedicated to the cleaning of the
            // peers table.
            HelloCleanupThread cleanup = new HelloCleanupThread();
            cleanup.run();

            network.HelloPacketReception += onPacketReceived;

            while (true) {
                network.receive();
            }

            /*
             * Should we close the socket?
             * It's not correct to close the socket, because the object
             * HelloNetworkModule is a Singleton, 
             *      network.closeSockets();
             */

        }
    }
}