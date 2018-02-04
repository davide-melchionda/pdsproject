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
        public delegate void OnPathError(System.IO.IOException e, Job job);
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
                ConnectionError?.Invoke(job);

            } catch (System.IO.IOException e) {

                String sourceName = null;
                //if (job != null) {
                //    job.Status = Job.JobStatus.ConnectionError;

                //    // Trigger the event of Path error
                //    sourceName = job.Task.Info[0].Name;
                //    if (job.Task.Info.Count > 1)
                //        for (int i = 1; i > job.Task.Info.Count; i++)
                //            sourceName += ", " + job.Task.Info[i].Name;
                //}
                PathError?.Invoke(e, job);

            } catch (ObjectDisposedException e) {

                ConnectionError?.Invoke(job);

                // NOTE: 'ObjectDisposedException' and 'SocketException' make the same management as the generale 'Exception'.
                // However they have their own catch because they are very common exceptions in this point of the code, and 
                // in this way they are marked and clearely visible.
                // This separation is not neede, but it's really useful to remark this concept.
            } catch (Exception e) {

                ConnectionError?.Invoke(job);

            } finally {

                // Remove the job (if any) from the list
                if (job != null)
                    JobsList.Receiving.remove(job.Id);
                // Close the socket
                socket.Close();
            }
        }

        protected override void PrepareStop() {
            socket.Close(); // Close the socket. This will cause an error
        }
    }
}
