using FileTransfer;
using System;


namespace NetworkTransmission
{
    public abstract class PacketClient 
    {
        public delegate void  PacketReceivedDel(TransmissionPacket p);
        public event PacketReceivedDel OnPacketReceived;

        public delegate void TransmissionEnddDel(TransmissionPacket p);
        public event TransmissionEnddDel OnTransmissionEnd;

        }
    }

   

