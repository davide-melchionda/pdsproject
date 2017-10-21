using System;
using System.Net.Sockets;
using NetworkTransmission;
using static NetProtocol.ProtocolEndpoint;
using NetProtocol;
using System.IO;

namespace FileTransfer
{
    /**
     * The specific server that performs all the steps that compose our transmission protocol.
     * 
     */

    public class TnSServer : ServerProtocolEndpoint
    {

        private byte[] transferBlock = new byte[8192];
        public Socket handler;
        public delegate void onRequest(FileInfo file);
        public event onRequest onRequestReceived;
        Protocol protocol;

        public TnSServer(Socket handler, Protocol protocol) : base(handler)
        {
            this.handler = handler;
            this.protocol = protocol;
        }

        public override TransferResult transfer()
        {
            try
            {

                TransmissionPacket received = (TransmissionPacket)TransferNetworkModule.receivePacket(handler); //receive a network packet from the client
                if (received.Type.ToString() != "request")
                {
                    //TODO This must raise an exception cause we don't expect a client to send responsens 
                }

                RequestPacket request = (RequestPacket)received;

                if (!Settings.Instance.AutoAcceptFiles)
                {
                    onRequestReceived(request.Task.informations);    // We need the user to know that there's a new transmission to accept or deny
                }

                else
                {
                    int i = TransferNetworkModule.SendPacket(handler, TransferNetworkModule.generateResponsetStream(true));
                }

                TransferNetworkModule.receiveFile(request.Task, this, transferBlock);

                handler.Close();

            }

            catch (Exception e)
            {
                Console.WriteLine("exception raised: " + e.ToString());
            }
            return new TnSTransferResult(true);

        }

    }

}

