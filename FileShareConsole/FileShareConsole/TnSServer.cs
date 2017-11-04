using System;
using System.Net.Sockets;
using NetworkTransmission;
using static NetProtocol.ProtocolEndpoint;
using NetProtocol;
using FileShareConsole;
using static StorageModule;
using System.Net;

namespace FileTransfer
{
    /**
     * The specific server that performs all the steps that compose our transmission protocol.
     * 
     */

    public class TnSServer : ServerProtocolEndpoint
    {

        public delegate void onRequest(FileInfo file);
        public event onRequest onRequestReceived;
        private TransferNetworkModule network;
        private byte[] chunk = new byte[8192];
  

        public TnSServer(Socket handler, Protocol protocol) : base(handler, protocol)
        {
            this.protocol = protocol;
            network = new TransferNetworkModule();
        }

        public override TransferResult transfer()
        {
            try
            {

                //TransmissionPacket received = (TransmissionPacket)TransferNetworkModule.receivePacket(handler); //receive a network packet from the client
                TransmissionPacket received = network.receivePacket(socket); //receive a network packet from the client
                if (received.Type.ToString() != "request")
                {
                    //TODO This must raise an exception cause we don't expect a client to send responsens 
                }

                RequestPacket request = (RequestPacket)received;

                // Try to acquire a semaphore to pass the high threshold
                // If more than a certain number of servers are active, blocks.
                protocol.enterServer((socket.RemoteEndPoint as IPEndPoint).Address.ToString());

                if (!Settings.Instance.AutoAcceptFiles)
                    onRequestReceived(request.Task.Info);    // We need the user to know that there's a new transmission to accept or deny
                else
                    network.SendPacket(socket, network.generateResponsetStream(true));

                JobZipStorageModule module = new JobZipStorageModule();
                FileIterator iterator = module.createJob(request.Task);

                int receivedBytes = 0;
                while (iterator.hasNext()) {
                    receivedBytes = socket.Receive(chunk);
                    iterator.write(chunk, receivedBytes);
                }

                iterator.close();

            } catch (Exception e) {
                Console.WriteLine("exception raised: " + e.ToString());
            }

            // Release the slot
            ((TnSProtocol)protocol).releaseServer((socket.RemoteEndPoint as IPEndPoint).Address.ToString());

            return new TnSTransferResult(true);

        }

    }

}

