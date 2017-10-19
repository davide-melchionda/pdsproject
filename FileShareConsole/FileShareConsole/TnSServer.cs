using System;
using System.Net.Sockets;
using NetworkTransmission;
using static NetProtocol.ProtocolEndpoint;
using NetProtocol;

namespace FileTransfer
{
       /**
        * The specific server that performs all the steps that compose our transmission protocol.
        * 
        */
    public class TnSServer : ServerProtocolEndpoint
    {
       
        public Socket handler;
        public delegate void onRequest(FileInfo file);
        public event onRequest onRequestReceived;
        TransferNetworkModule tnm;
        Protocol protocol;

        public TnSServer(Socket handler,Protocol protocol) : base(handler)
        {
            this.handler = handler;
            tnm = TransferNetworkModule.Instance;
            this.protocol = protocol;
        }

        public override TransferResult transfer()
        {

            try
            {


                TransmissionPacket received = (TransmissionPacket)tnm.receivePacket(handler); //receive a network packet from the client
                if (received.Type.ToString() != "request")
                {
                    Console.WriteLine("il client ha richiesto un invio ma arriva roba a caso ");

                    //TODO This must raise an exception cause we don't expect a client to send responsens 
                }

                RequestPacket request = (RequestPacket)received;

                if (Settings.Instance.AutoAcceptFiles)
                {
                    Console.WriteLine("il client ha richiesto un invio\n");
                    int i = tnm.SendPacket(handler, tnm.generateResponsetStream(true));   // We don't need te user to accept the transmission
                    Console.WriteLine("io server vado via e ho spedito: " + i + "bytes");

                }

                else
                {
                    Console.WriteLine("il client ha richiesto un invio\n");

                    onRequestReceived(request.Task.informations);    // We need the user to know that there's a new transmission to accept or deny
                }


            }

            catch (Exception e)
            {
                Console.WriteLine("exception raised: " + e.ToString());
            }
            return new TnSTransferResult(true);

        }

    }

}

