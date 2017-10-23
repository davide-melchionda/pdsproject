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

        // CORR --> Non serve, il padre ne ha un'istanza
        //public Socket handler;

        public delegate void onRequest(FileInfo file);
        public event onRequest onRequestReceived;
        
        // CORR --> Non serve, il padre ne ha un'istanza
        //Protocol protocol;

        public TnSServer(Socket handler, Protocol protocol) : base(handler, protocol)
        {
            //this.handler = handler;
            this.protocol = protocol;
        }

        public override TransferResult transfer()
        {
            try
            {

                //TransmissionPacket received = (TransmissionPacket)TransferNetworkModule.receivePacket(handler); //receive a network packet from the client
                TransmissionPacket received = TransferNetworkModule.receivePacket(socket); //receive a network packet from the client
                if (received.Type.ToString() != "request")
                {
                    //TODO This must raise an exception cause we don't expect a client to send responsens 
                }

                RequestPacket request = (RequestPacket)received;

                if (!Settings.Instance.AutoAcceptFiles)
                {
                    onRequestReceived(request.Task.Info);    // We need the user to know that there's a new transmission to accept or deny
                }

                else
                {
                    int i = TransferNetworkModule.SendPacket(socket, TransferNetworkModule.generateResponsetStream(true));
                }

                //TransferNetworkModule.receiveFile(request.Task, this, transferBlock);
                long receivedBytes = 0;
                FileStream Fs = new FileStream(request.Task.Info.Name, FileMode.OpenOrCreate, FileAccess.Write);

                while (receivedBytes != request.Task.Size) {
                    receivedBytes += socket.Receive(transferBlock);

                    Fs.Write(transferBlock, 0, 0);
                }

                Fs.Close();

                // CORR --> Responsabilità del livello superiore?
                //socket.Close();

            }

            catch (Exception e)
            {
                Console.WriteLine("exception raised: " + e.ToString());
            }
            return new TnSTransferResult(true);

        }

    }

}

