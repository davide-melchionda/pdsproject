
namespace NetworkTransmission
{
    class ResponsePacket : TransmissionPacket
    {
        /**
         * The constructor sets the appropriate property Type 
         */
        public ResponsePacket(bool procede)
        {
            Type = PacketType.response;
            this.procede = procede;
        }

        /**
         * The actual will of the receiver to start or deny the transmission.
         */
        private bool procede;
        public bool Procede
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
