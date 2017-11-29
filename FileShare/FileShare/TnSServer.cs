using System;
using System.Net.Sockets;
using NetworkTransmission;
using static NetProtocol.ProtocolEndpoint;
using NetProtocol;
using FileShareConsole;
using static StorageModule;
using System.Net;
using System.Threading;
using static FileShareConsole.JobZipStorageModule;
using System.Windows.Forms;

namespace FileTransfer
{
    /**
     * The specific server that performs all the steps that compose our transmission protocol.
     * 
     */

    public class TnSServer : ServerProtocolEndpoint
    {

        public delegate bool OnRequest(Task task);
        public event OnRequest OnRequestReceived;

        public delegate void OnJobInitialized(Job j);
        public event OnJobInitialized JobInitialized;

        private TransferNetworkModule network;
        private byte[] chunk = new byte[8192];
        private static ManualResetEvent mre = new ManualResetEvent(false);

        public TnSServer(Socket handler, Protocol protocol) : base(handler, protocol)
        {
            this.protocol = protocol;
            network = new TransferNetworkModule();
        }

        public override TransferResult transfer() {

            RequestPacket request = null;
            FileIterator iterator = null;

            try {

                TransmissionPacket received = network.receivePacket(socket); //receive a network packet from the client
                if (received.Type.ToString() != "request") {
                    //TODO This must raise an exception cause we don't expect a client to send responsens 
                } else {

                    request = (RequestPacket)received;

                    // Try to acquire a semaphore to pass the high threshold
                    // If more than a certain number of servers are active, blocks.
                    protocol.enterServer((socket.RemoteEndPoint as IPEndPoint).Address.ToString());

                    // If settings allow us to receive anything or if the user confirms that he wants to accept
                    if (Settings.Instance.AutoAcceptFiles || OnRequestReceived(request.Task)) {
                        // Send a positive response
                        network.SendPacket(socket, network.generateResponsetStream(true));
                        string receivePath;
                        if (Settings.Instance.AlwaysUseDefault) {
                            receivePath = Settings.Instance.DefaultRecvPath;
                        } else {
                            FolderBrowserDialog dialog = new FolderBrowserDialog();
                            dialog.SelectedPath = Settings.Instance.DefaultRecvPath; ;
                            dialog.ShowDialog();
                            receivePath = dialog.SelectedPath;
                        }
                        /* Create a Job for the incoming task. */
                        JobZipStorageModule module = new JobZipStorageModule();
                        iterator = module.createJob(request.Task, receivePath);
                        // Execute operations when job has been created
                        JobInitialized?.Invoke(((JobFileIterator)iterator).Job);

                        // Start file trasfer
                        int receivedBytes = 0;
                        while (iterator.hasNext()) {

                            // socket.Poll(-1, SelectMode.SelectRead) checks the readable status of the socket. It's blocking, and (due to the infinite
                            // timeout setted as first parameter) unlocks only when data are available or throws an exception when error occours.
                            if ((socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0) || !(iterator as JobFileIterator).Job.Active) {
                                throw new SocketException();
                            } else {
                                receivedBytes = socket.Receive(chunk);
                                iterator.write(chunk, receivedBytes);
                            }
                        }
                        
                    } else { // ... if the user doesn't accepted to receive the file
                             // Send a negative response
                        network.SendPacket(socket, network.generateResponsetStream(false));
                    }
                }
            } finally {

                // Close the iterator so to release resources
                if (iterator != null)
                    iterator.close();
                
                // I don't know if I really have acquired a slot (maybe an exception occourde befor I could do it)
                // The realease operation will throw an exception if the I have not acquired a slot
                try {
                    // Release the slot
                    ((TnSProtocol)protocol).releaseServer((socket.RemoteEndPoint as IPEndPoint).Address.ToString());
                } catch (SemaphoreFullException e) {
                    // This means that SocketException occurred befor I could acquire the semaphore slot
                }

            }

            return new TnSTransferResult(true);

        }

    }

}

