using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NetworkTransmission
{
    public class TransmissionPacket
    {
        /**
     * Defines the possible values that can be assumed by the 
     * field "Type" in a packet.
     * The Json convertion must produce a string.
     */
        [JsonConverter(typeof(StringEnumConverter))] // Enum to string when serialized 
        public enum PacketType
        {
            request,    // a Request packet is sent in order to express to a 
                                   // peer the willingness to send a file.

            response    // a Response packet is sent to accept or refuse a request.
        };

        public AuthenticationLayer challenge;
        private PacketType type;

        /**
         * The property coresponding to the type field.
         * type => Type is not editable by others than class childs.
         */
        public PacketType Type
        {
            get
            {
                return type;
            }
            protected set
            {
                type = value;
            }
        }


    }

}
    