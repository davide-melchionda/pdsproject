using System;
using FileTransfer;
using System.Net.Sockets;
using System.Net;
using static NetProtocol.ProtocolEndpoint;
using NetProtocol;

namespace NetworkTransmission
{
        /**
         * The specific client that performs all the steps that compose our transmission protocol.
         * 
         */

    class TnSClient : ClientProtocolEndpoint
    {

        public FileIterator iterator;       // Iterator provides the abstraction towards the file location 
        public Task task;                   // Task that provides all the informations about transfer
        public Socket socket;               // The socket on wich we transmit---> The connection has been established by the caller
        public Protocol protocol;


        public TnSClient(Socket socket, Protocol protocol, FileIterator fIterator, Task involvedTask) : base(socket)
        {
            this.socket = socket;
            this.iterator = fIterator;
            this.task = involvedTask;
            this.protocol = protocol;
        }


        public override TransferResult transfer()
        {
            TransmissionPacket tPacket = null;

            byte[] bytes = new byte[8192];      // Data buffer for incoming data.

            Console.WriteLine("Socket connected to {0}",
                    this.socket.RemoteEndPoint.ToString());

            byte[] msg = TransferNetworkModule.generateRequestStream(task);       // Encode the data string into a byte array.

            int bytesSent = TransferNetworkModule.SendPacket(socket, msg);        // Send the data through the socket.
            Console.WriteLine("data sended to server");



            tPacket = (TransmissionPacket)TransferNetworkModule.receivePacket(this.socket);       // Receive the response from the remote.
            if (tPacket.Type.ToString() != "response")

            {
                //TODO This must raise an exception cause we don't expect a server to send requests 

            }

            ResponsePacket response = (ResponsePacket)tPacket;
            if (response.Procede)
            {
                this.protocol.enter();   //TODO Ho avuto risposta affermativa, procedo provando ad acquisire il semaforo (BLOCKING)
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

