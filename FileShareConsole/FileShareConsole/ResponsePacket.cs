using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NetworkTransmission
{
    class ResponsePacket : TransmissionPacket
    {
        /**
         * The constructor sets the appropriate property Type 
         */
        public ResponsePacket()
        {
            Type = PacketType.request;
        }

        /**
         * The actual will of the receiver to start or deny the transmission.
         */
        public bool procede
        {
            get
            {
                return procede;
            }
            set
            {
                procede = value;
            }
        }
    }
}
