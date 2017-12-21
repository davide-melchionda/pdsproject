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
        public delegate void OnConnectionError(Job j);
        /**
         * Event on which register the callback to manage the connection error
         */
        public event OnConnectionError ConnectionError;

        /**
    * Delegate: format of the callback to call when error on Path occours
    */
        public delegate void OnPathError();
        /**
         * Event on which register the callback to manage the Path error
         */
        public event OnPathError PathError;
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
//<<<<<<< job-status-and-notifications
                ConnectionError?.Invoke(job);

//=======
//                ConnectionError?.Invoke();
            }
            catch (Exception e)
            {
                // Trigger the event of Path error
                PathError?.Invoke();
//>>>>>>> master
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
