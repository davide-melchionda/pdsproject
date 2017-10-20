﻿using System;
using FileTransfer;
using System.Net.Sockets;
using System.Net;
using static NetProtocol.ProtocolEndpoint;
using NetProtocol;
using System.Text;

namespace NetworkTransmission
{
    /**
     * The specific client that performs all the steps that compose our transmission protocol. 
     */

    class TnSClient : ClientProtocolEndpoint
    {

        public FileIterator iterator;       // Iterator provides the abstraction towards the file location 
        public Task task;                   // Task that provides all the informations about transfer
        public Socket socket;               // The socket on wich we transmit---> The connection has been established by the caller
        public Protocol protocol;
        //private byte[] transferBlock= new byte[JobZipMemoryModuke.BLOCK_SIZE];
        private byte[] transferBlock = new byte[8192];

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
                //TODO This must raise an exception cause we don't expect a server to send packets different from Responses at this point 

            }

            ResponsePacket response = (ResponsePacket)tPacket;
            if (!response.Procede)
            {
                //TODO nothing to do, the server doesn't want my files
            }
            else
            {
                TransferNetworkModule.sendFile(this);
            }
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




