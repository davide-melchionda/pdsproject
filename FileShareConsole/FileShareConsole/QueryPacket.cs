using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloProtocol {

    class QueryPacket : HelloPacket {

        /**
         * The constructor sets the property Type to the Type.Query value.
         */
        public QueryPacket() {
            Type = PacketType.Query;
        }

    }
}
