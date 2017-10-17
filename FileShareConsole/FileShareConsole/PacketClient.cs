using FileTransfer;
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

   

