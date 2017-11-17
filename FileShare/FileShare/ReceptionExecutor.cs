﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer {
    class ReceptionExecutor : ExecutableThread {

        private Socket socket;
        private Job job;
        
        public delegate bool OnRequest(Task task);
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
            server.OnRequestReceived += (Task task) => {
                if (RequestReceived != null)
                    return RequestReceived(task);
                return true;
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

                if (job != null)
                    JobsList.Receiving.remove(job.Id);

            }
        }
    }
}
