using System;
using System.Net.Sockets;
using NetworkTransmission;
using static NetProtocol.ProtocolEndpoint;
using NetProtocol;
using FileShareConsole;
using static StorageModule;

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
        private byte[] chunk = new byte[8192];
  

        public TnSServer(Socket handler, Protocol protocol) : base(handler, protocol)
        {
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

                JobZipStorageModule module = new JobZipStorageModule();
                FileIterator iterator = module.CreateJob(request.Task);  // Sarà l'iteratore ad aggiornare la progress_bar

                long receivedBytes = 0;
                while (iterator.hasNext())
                {

                    receivedBytes = socket.Receive(chunk);
                    iterator.write(chunk, receivedBytes);   //invio il chunk e l'effettiva quantità di dato, iterator gestirà internamente l'offset

                }

                base.socket.Close();

            }

            catch (Exception e)
            {
                Console.WriteLine("exception raised: " + e.ToString());
            }
            return new TnSTransferResult(true);

        }

    }

}

