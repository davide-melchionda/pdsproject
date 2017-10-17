using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTransmission
{
    class PacketServer : ExecutableThread
    {

        public delegate void PacketReceivedDel(Packet p);
        public event PacketReceivedDel OnPacketReceived;

        public delegate void TrasmissionEnddDel(Packet p);
        public event TrasmissionEnddDel OnTrasmissionEnd;

        public PacketServer() { }

        protected override void execute()
        {
            throw new NotImplementedException();
        }
    }

}


