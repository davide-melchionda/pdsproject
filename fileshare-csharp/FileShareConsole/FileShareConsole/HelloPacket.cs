using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HelloProtocol {
    
    internal abstract class HelloPacket {

        /**
         * Defines the possible values that can be assumed by the 
         * field "Type" in a packet.
         * The Json convertion must produce a string.
         */
         [JsonConverter(typeof(StringEnumConverter))] // Enum to string when serialized 
        public enum PacketType {
            Keepalive,      // A Keepalive packet is sent in order to let the
                            // other peers know the current peer is still alive.
                            // This packet is sent in multicast.
            Query,          // A query packet is sent when the peer receives a
                            // (keepalive) packet from an unknown peer.
                            // This packet ins sent in unicast.
            Presentation    // A presentation packet is sent when a peer receives
                            // a query packet from another peer. It contains all 
                            // the relevan informations about the sender peer.
                            // This packet is sent in unicast.
        };

        /**
         * Defines the type of the packet.
         */
        private PacketType type;
        
        /**
         * The property coresponding to the type field.
         * type => Type is not editable by others than class childs.
         */
         public PacketType Type {
            get {
                return type;
            }
            protected set {
                type = value;
            }
        }


    }

}