using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloProtocol
{

    class GoodByePacket : HelloPacket
    {

        /**
         * The id of the announcing peer.s
         */
        private string peerId;


        /**
         * The constructor sets the property Type to the Type.GoodBye value.
         */
        public GoodByePacket(string peerId)
        {
            Type = PacketType.GoodBye;
            this.peerId = peerId;
          
        }

        /**
         * Properties
         */
        public string PeerId
        {
            get
            {
                return peerId;
            }
            set
            {
                peerId = value;
            }
        }

    }

}
