using FileTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTransmission
{
    public class PacketClient : ExecutableThread
    {
        public delegate void  PacketReceivedDel(Packet p);
        public event PacketReceivedDel OnPacketReceived;

        public delegate void TrasmissionEnddDel(Packet p);
        public event TrasmissionEnddDel OnTrasmissionEnd;

        public PacketClient(FileIterator iterator) { }

        protected override void execute()
        {
            throw new NotImplementedException();
        }
    }

   
}

namespace NetworkTransmission
{
    public class PacketClient : ExecutableThread
    {
        public delegate void  PacketReceivedDel(Packet p);
        public event PacketReceivedDel OnPacketReceived;

        public delegate void TrasmissionEnddDel(Packet p);
        public event TrasmissionEnddDel OnTrasmissionEnd;

        public PacketClient(FileIterator iterator) { }

        protected override void execute()
        {
            throw new NotImplementedException();
        }
    }

   
}
=======
﻿using FileTransfer;
using System;


namespace NetworkTransmission
{
    public abstract class PacketClient : ExecutableThread
    {
        public delegate void  PacketReceivedDel(Packet p);
        public event PacketReceivedDel OnPacketReceived;

        public delegate void TransmissionEnddDel(Packet p);
        public event TransmissionEnddDel OnTransmissionEnd;

        }
    }

   

>>>>>>> Stashed changes

<<<<<<< Updated upstream
﻿using FileTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;