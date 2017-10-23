using System;
using FileTransfer;
using System.Net.Sockets;
using System.Net;
using static NetProtocol.ProtocolEndpoint;
using NetProtocol;
using System.Text;
using static StorageModule;

namespace NetworkTransmission
{
    /**
     * The specific client that performs all the steps that compose our transmission protocol. 
     */

    class TnSClient : ClientProtocolEndpoint {

        public FileIterator iterator;       // Iterator provides the abstraction towards the file location 
        public Task task;                   // Task that provides all the informations about transfer
        
        // CORR --> Non serve: il padre ha il padre ha la sua istanza di socket
        //public Socket socket;               // The socket on wich we transmit---> The connection has been established by the caller
            
        // CORR --> Non serve: il padre ha la sua istanza di protocol
        //public Protocol protocol; -->
        
        //private byte[] transferBlock= new byte[JobZipMemoryModuke.BLOCK_SIZE];
        private byte[] transferBlock = new byte[8192];

        public TnSClient(Socket socket, Protocol protocol, FileIterator fIterator, Task involvedTask) : base(socket, protocol)
        {
            //this.socket = socket;     // CORR --> VEDI SOPRA
            this.iterator = fIterator;
            this.task = involvedTask;
            //this.protocol = protocol; // CORR --> VEDI SOPRA
        }
        //test constructor
        public TnSClient(Socket socket, Protocol protocol, Task involvedTask) : base(socket, protocol)
        {
            //this.socket = socket;   // CORR --> VEDI SOPRA
            this.task = involvedTask;
            //this.protocol = protocol; // CORR --> VEDI SOPRA
        }



        public override TransferResult transfer()
        {
            TransmissionPacket tPacket = null;

            byte[] msg = TransferNetworkModule.generateRequestStream(task);       // Encode the data string into a byte array.

            int bytesSent = TransferNetworkModule.SendPacket(socket, msg);        // Send the data through the socket.


            tPacket = (TransmissionPacket)TransferNetworkModule.receivePacket(this.socket);       // Receive the response from the remote.
            if (tPacket.Type.ToString() != "response")

            {
                //TODO This must raise an exception cause we don't expect a server to send packets different from Responses at this point 

            }

            ResponsePacket response = (ResponsePacket)tPacket;
            if (response.Procede)
            {
                //TransferNetworkModule.sendFile(this, transferBlock);
                // CORR -> Non facciamo vedere il protocollo all'esterno, ma solo il client
                protocol.enter();// protocol.enter();   //TODO Ho avuto risposta affermativa, procedo provando ad acquisire il semaforo (BLOCKING)

                while (iterator.hasNext()) {
                    iterator.next(transferBlock);
                    socket.Send(transferBlock);

                }
                iterator.close();
            }

            socket.Close();
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


