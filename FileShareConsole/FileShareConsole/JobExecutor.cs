using Protocol;
using System.Net;
using System.Net.Sockets;
using static Protocol.ProtocolEndpoint;

namespace FileTransfer {

    /**
     * Executes the operation of sending an opened file given an iterator to it
     */
    internal class JobExecutor : ExecutableThread {

        /**
         * Iterator which represents the acces point to the file to send
         */
        private StorageModule.FileIterator iterator;

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
        public JobExecutor(Job job, StorageModule.FileIterator iterator) {
            this.iterator = iterator;
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
            //socket.Connect(remoteEP); // DEBUG

            // Create a client for the specific protocol
            // The client receives the socket whith the connection established
            ClientProtocolEndpoint client = new DummyClient(socket, iterator, job.Task);

            // Executes the transmission and obtais a result
            DummyTransferResult res = (DummyTransferResult)client.transfer();
            
            // Close the socket
            socket.Close();

            // Calls the delegate
            OnTrasmissionEnd();

        }
    }
}