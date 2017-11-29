using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer {
    class ReceptionExecutor : ExecutableThread {

        private Socket socket;
        private Job job;
        
        public delegate ToAccept OnRequest(ToAccept request);
        public event OnRequest RequestReceived;

        /**
         * Delegate: format of the callback to call when error on connection occours
         */
        public delegate void OnConnectionError();
        /**
         * Event on which register the callback to manage the connection error
         */
        public event OnConnectionError ConnectionError;

        public ReceptionExecutor(Socket socket) {
            this.socket = socket;
        }

        protected override void execute() {
            TnSServer server = new TnSServer(socket, new TnSProtocol());
            server.OnRequestReceived += (ToAccept request) => {
                if (RequestReceived != null)
                    return RequestReceived(request);
                return request;
            };

            server.JobInitialized += (Job job) => {
                JobsList.Receiving.push(job);
                this.job = job;
            };

            try {

                server.transfer();

            } catch (SocketException e) {
                
                // Trigger the event of conncetion error
                ConnectionError?.Invoke();

            } finally {
                // Remove the job (if any) from the list
                if (job != null)
                    JobsList.Receiving.remove(job.Id);
                // Close the socket
                socket.Close();
            }
        }
    }
}
