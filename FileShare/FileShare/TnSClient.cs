using System;
using FileTransfer;
using System.Net.Sockets;
using static NetProtocol.ProtocolEndpoint;
using NetProtocol;
using static StorageModule;
using FileShareConsole;

namespace NetworkTransmission
{
    /**
     * The specific client that performs all the steps that compose our transmission protocol. 
     */

    class TnSClient : ClientProtocolEndpoint
    {

        private FileIterator iterator;       // Iterator provides the abstraction towards the file location 
        private Job job;                   // Task that provides all the informations about transfer
        private TransferNetworkModule network;


        private byte[] transferBlock = new byte[8192];

        public TnSClient(Socket socket, Protocol protocol, Job job) : base(socket, protocol)
        {
            this.job = job;
            JobZipStorageModule module = new JobZipStorageModule();
            iterator = module.prepareJob(job);
            network = new TransferNetworkModule();
        }

        public override TransferResult transfer()
        {

            ResponsePacket response = null;
            TransmissionPacket tPacket = null;



            byte[] msg = network.generateRequestStream(job.Task);       // Encode the data string into a byte array.

            int bytesSent = network.SendPacket(socket, msg);        // Send the data through the socket.

            tPacket = (TransmissionPacket)network.receivePacket(this.socket);       // Receive the response from the remote.
            if (tPacket.Type.ToString() != "response")
                throw new SocketException();//TODO This must raise an exception cause we don't expect a server to send packets different from Responses at this point 

            response = (ResponsePacket)tPacket;

            Logger.log(Logger.TRANSFER_CLIENT_DEBUG, "Handshake competed. Starting transmission of the file " + job.Task.Info.Name + "\n");

            if (response.Procede)
            {
                try
                {
                    protocol.enterClient();

                    // Send timeout: if cannot execute tht send. More responsive
                    socket.SendTimeout = 5000;

                    int i = 0;
                    while (iterator.hasNext())
                    {
                        if (!job.Active)
                            throw new SocketException();
                        i = iterator.next(transferBlock);
                        socket.Send(transferBlock, 0, i, SocketFlags.None);
                    }

                    Logger.log(Logger.TRANSFER_CLIENT_DEBUG, "Transfer completed for the file " + job.Task.Info.Name + "\n");

                    Logger.log(Logger.TRANSFER_CLIENT_DEBUG, "Releasing lock\n");

                }

                finally
                {
                    /* Close the file iterator releasing resources and
                     * releases protocol resources */
                    iterator.close();
                    protocol.releaseClient();
                }
            }

            return new TnSTransferResult(response.Procede);

        }

    }


    public class TnSTransferResult : TransferResult
    {
        public bool result;
        public TnSTransferResult(bool result)
        {
            this.result = result;
        }
    }

}


