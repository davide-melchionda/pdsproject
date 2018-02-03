using NetProtocol;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using static NetProtocol.ProtocolEndpoint;

namespace FileTransfer {

    /**
     * Executes the operation of sending an opened file given an iterator to it
     */
    internal class JobExecutor : ExecutableThread {

        /**
         * The job which represents the transmission operation
         */
        private SendingJob job;

        /**
         * Delegate: defines the format of a callback to call when the transmission 
         * ends
         */
        public delegate void TrasmissionEnddDel();
        /**
         * Event on which register the callback on transmission end
         */
        public event TrasmissionEnddDel OnTrasmissionEnd;

        /**
         * Delegate: format of the callback to call when error on connection occours
         */
        public delegate void OnConnectionError(Job j);
        /**
         * Event on which register the callback to manage the connection error
         */
        public event OnConnectionError ConnectionError;

        /// <summary>
        /// The socket on which the transfer will be executed
        /// </summary>
        Socket socket;

        public delegate void OnFileError(System.Exception e, String source);
        /**
         * Event on which register the callback to manage the connection error
         */
        public event OnFileError FileError;
        /**
         * Constructor
         */
        public JobExecutor(SendingJob job) {
            this.job = job;

            this.job.PropertyChanged += (object sender, PropertyChangedEventArgs args) => {
                if (args.PropertyName == "Status" && this.job.Status == Job.JobStatus.StoppedByLocal)
                    StopThread();
            };

        }

        /**
         * Execution of the thread
         */
        protected override void execute() {
            // Create a TCP/IP  socket.
            socket = new Socket(AddressFamily.InterNetwork,
                                         SocketType.Stream,
                                         ProtocolType.Tcp);
            // Specifies the receiver address in order to perform conncetion
            Peer receiver = HelloProtocol.PeersList.Instance.get(job.Task.Receiver);
            IPAddress receiverAddr = IPAddress.Parse(receiver.Ipaddress);
            IPEndPoint remoteEP = new IPEndPoint(receiverAddr, Settings.Instance.SERV_ACCEPTING_PORT);
            try {
                // Connect to the receiver socket
                socket.Connect(remoteEP);

                // Create a client for the specific protocol
                // The client receives the socket whith the connection established
                //ClientProtocolEndpoint client = new DummyClient(socket, new DummyProtocol(), iterator, job.Task);
                ClientProtocolEndpoint client = new NetworkTransmission.TnSClient(socket, new TnSProtocol(), job);

                // Executes the transmission and obtais a result
                TransferResult res = client.transfer();

                // Calls the delegate
                OnTrasmissionEnd();

            } catch (SocketException e) {
                // Remove the job from the list
                JobsList.Sending.remove(job.Id);
                // Trigger the event of conncetion error
                if (job.Status != Job.JobStatus.StoppedByLocal)
                    job.Status = Job.JobStatus.ConnectionError;
                //<<<<<<< job-status-and-notifications
                ConnectionError?.Invoke(job);
                //            } finally {
                //=======
                //                ConnectionError?.Invoke();
            } catch (System.Exception e) {
            
                // Remove the job from the list
                JobsList.Sending.remove(job.Id);
                
                // Update the job status
                job.Status = Job.JobStatus.ConnectionError;

                // Trigger the event of conncetion error
                String sourceName = job.Task.Info[0].Name;
                if (job.Task.Info.Count > 1)
                    for (int i = 1; i > job.Task.Info.Count; i++)
                        sourceName += ", " + job.Task.Info[i].Name;
                FileError?.Invoke(e, sourceName);

            } finally {
                // Close the socket
                socket.Close();
            }

        }

        protected override void PrepareStop() {
            socket.Close();
        }
    }
}