﻿using System;
using FileTransfer;
using System.Net.Sockets;
using static NetProtocol.ProtocolEndpoint;
using NetProtocol;
using static StorageModule;
using FileShareConsole;

namespace NetworkTransmission {
    /**
     * The specific client that performs all the steps that compose our transmission protocol. 
     */

    class TnSClient : ClientProtocolEndpoint {

        private FileIterator iterator;       // Iterator provides the abstraction towards the file location 
        private Job job;                   // Task that provides all the informations about transfer
        private TransferNetworkModule network;


        private byte[] transferBlock;

        public TnSClient(Socket socket, Protocol protocol, SendingJob job) : base(socket, protocol) {
            this.job = job;
            job.Status = Job.JobStatus.Preparing;
            try {
                JobZipStorageModule module = new JobZipStorageModule();
                iterator = module.prepareJob(job);
                network = new TransferNetworkModule();
                transferBlock = new byte[((JobZipStorageModule.JobFileIterator)iterator).READ_BLOCK_SIZE];
            } catch (Exception e) {
                if (iterator != null)
                    iterator.close();
                throw e;
            }
        }

        public override TransferResult transfer() {

            ResponsePacket response = null;
            TransmissionPacket tPacket = null;

            try {

                byte[] msg = network.generateRequestStream(job.Task);       // Encode the data string into a byte array.

                int bytesSent = network.SendPacket(socket, msg);        // Send the data through the socket.

                job.Status = Job.JobStatus.WaitingForRemoteAcceptance;
            
                tPacket = (TransmissionPacket)network.receivePacket(this.socket);       // Receive the response from the remote.
                if (tPacket.Type.ToString() != "response")
                    throw new SocketException();//TODO This must raise an exception cause we don't expect a server to send packets different from Responses at this point 

                response = (ResponsePacket)tPacket;

                job.Status = Job.JobStatus.Active;

                //Logger.log(Logger.TRANSFER_CLIENT_DEBUG, "Handshake competed. Starting transmission of the file " + job.Task.Info.Name + "\n");

                if (response.Procede) {
                    try {
                        protocol.enterClient();

                        int i = 0;
                        while (iterator.hasNext()) {
                            if (job.Status != Job.JobStatus.Active)
                                throw new SocketException();
                            i = iterator.next(transferBlock);

                            // If the server closes the socket, this instruction will throw a SocketException
                            socket.Send(transferBlock, 0, i, SocketFlags.None);
                        }

                        //Logger.log(Logger.TRANSFER_CLIENT_DEBUG, "Transfer completed for the file " + job.Task.Info.Name + "\n");

                        Logger.log(Logger.TRANSFER_CLIENT_DEBUG, "Releasing lock\n");

                        job.Status = Job.JobStatus.Completed;

                    } finally {
                        ///* Close the file iterator releasing resources and
                        // * releases protocol resources */
                        //iterator.close();

                        if (job.Status == Job.JobStatus.Active) // If job status is still active it was not me to raise this error
                            job.Status = Job.JobStatus.ConnectionError;

                        // I don't know if I really have acquired a slot (maybe an exception occourde befor I could do it)
                        // The realease operation will throw an exception if the I have not acquired a slot
                        try {
                            // Release the slot
                            protocol.releaseClient();
                        } catch (System.Threading.SemaphoreFullException e) {
                            // This means that SocketException occurred befor I could acquire the semaphore slot
                        }
                    }
                } else {
                    ///* Close the file iterator releasing resources and
                    // * releases protocol resources */
                    //iterator.close();
                    job.Status = Job.JobStatus.NotAcceptedByRemote;
                }

            } finally {
                /* Close the file iterator releasing resources and
                 * releases protocol resources */
                 iterator.close();
            }

            return new TnSTransferResult(response.Procede);
        }

    }


    public class TnSTransferResult : TransferResult {
        public bool result;
        public TnSTransferResult(bool result) {
            this.result = result;
        }
    }

}


