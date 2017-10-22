<<<<<<< Updated upstream
=======
<<<<<<< Updated upstream
>>>>>>> Stashed changes
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTransmission
{
    public interface Packet
    {
    }
}
﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTransmission
{
    public interface Packet {}

    public class TransmissionPacket : Packet
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

        public AuthenticationLayer challange;
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
    
