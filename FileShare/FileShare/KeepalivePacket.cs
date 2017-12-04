using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloProtocol {

    class KeepalivePacket : HelloPacket {

        /**
         * The id of the announcing peer.s
         */
        private string peerId;

        /**
         * The name of the announcing peer.
         */
        private string peerName;

        /**
         * The ip address of the announcing peer.
         */
        private string peerIp;

        /**
         * The constructor sets the property Type to the Type.Keepalive value.
         */
        public KeepalivePacket(string peerId, string peerName, string peerIp) {
            Type = PacketType.Keepalive;
            this.peerId = peerId;
            this.peerName = peerName;
            this.peerIp = peerIp;
        }

        /**
         * Properties
         */
        public string PeerId {
            get {
                return peerId;
            }
            set {
                peerId = value;
            }
        }
        public string PeerName {
            get {
                return peerName;
            }
            set {
                peerName = value;
            }
        }
        public string PeerIp {
            get {
                return peerIp;
            }
            set {
                peerIp = value;
            }
        }
    }

}
