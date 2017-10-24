using NetProtocol;
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
        private Job job;

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
         * Constructor
         */
        public JobExecutor(Job job) {
            this.job = job;
        }

        /**
         * Execution of the thread
         */
        protected override void execute() {
            // Create a TCP/IP  socket.
            Socket socket = new Socket(AddressFamily.InterNetwork,
                                        SocketType.Stream, 
                                        ProtocolType.Tcp);
            // Specifies the receiver address in order to perform conncetion
            Peer receiver = HelloProtocol.PeersList.Instance.get(job.Task.Receiver);
            IPAddress receiverAddr = IPAddress.Parse(receiver.Ipaddress);
            IPEndPoint remoteEP = new IPEndPoint(receiverAddr, Settings.Instance.SERV_ACCEPTING_PORT);
            // Connect to the receiver socket
            socket.Connect(remoteEP);

            // Create a client for the specific protocol
            // The client receives the socket whith the connection established
            //ClientProtocolEndpoint client = new DummyClient(socket, new DummyProtocol(), iterator, job.Task);
            ClientProtocolEndpoint client = new NetworkTransmission.TnSClient(socket, new DummyProtocol(), job);

            // Executes the transmission and obtais a result
            TransferResult res = client.transfer();
            
            // Close the socket
            socket.Close();

            // Calls the delegate
            OnTrasmissionEnd();

        }
    }
}