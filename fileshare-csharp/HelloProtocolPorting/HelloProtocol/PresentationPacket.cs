using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloProtocol {

    class PresentationPacket : HelloPacket {

        /**
         * The information about the peer who is announcing.
         */
        private Peer peer;
        
        /**
         * The constructor sets the property Type to the Type.Presentation value.
         */
        public PresentationPacket(Peer peer) {
            Type = PacketType.Presentation;
            this.peer = peer;
        }

        /**
         * Properties
         */
        public Peer Peer {
            get {
                return peer;
            }
            set {
                peer = value;
            }
        }

    }

}
