using FileShareConsole;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static NetProtocol.ProtocolEndpoint;
using static StorageModule;

namespace FileTransfer {
    public class JobScheduler {

        /**
         * Constructor
         */
        public JobScheduler() {
        }

        /**
         * Given a job, executes its computation. The computation of a 
         * Job consist in the assignment of the job to a JobExecutor 
         * which has the purpose of execute the file tranfer according 
         * to a specific protocol.
         */
        public void scheduleJob(Job job) {

            // Pushes the job in the list of in-service jobs
            JobsList.Sending.push(job);

            ThreadPool.QueueUserWorkItem(new WaitCallback(execute), job);

        }

        private  void execute(Object state)
        {
            Job job = (Job)state;
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
            ClientProtocolEndpoint client = new NetworkTransmission.TnSClient(socket, new TnSProtocol(), job);

            // Executes the transmission and obtais a result
            TransferResult res = client.transfer();

            // Close the socket
            socket.Close();
            JobsList.Sending.remove(job.Id);


        }
    }

}