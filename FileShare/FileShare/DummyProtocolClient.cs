using System;
using System.Net.Sockets;
using NetworkTransmission;
using NetProtocol;
using static StorageModule;
using System.Text;
using static NetProtocol.ProtocolEndpoint;
using FileShareConsole;
using System.IO;

namespace FileTransfer {

    public class DummyClient : ClientProtocolEndpoint {

        private Job job;

        private FileIterator sendIterator;
        private FileIterator recvIterator;

        public DummyClient(Socket socket, Protocol protocol, Job job) : base(socket, protocol) {
            this.job = job;
            JobZipStorageModule storage = new JobZipStorageModule();
            sendIterator = storage.prepareJob(job);
            recvIterator = storage.createJob(job.Task,Settings.Instance.DefaultRecvPath);
        }
        

        public override TransferResult transfer() {
            byte[] buf = new Byte[JobZipStorageModule.READ_BLOCK_SIZE+1];
            while (sendIterator.hasNext()) {
                int read = sendIterator.next(buf);
                //filestream f = file.create(@"c:\users\vm-dm-win\desktop\examplefile.txt");
                //f.write(buf, 0, read);
                //f.close();
                recvIterator.write(buf, read);  // Rewrites in another file
            }

            System.Threading.Thread.Sleep(10000);

            sendIterator.close();
            recvIterator.close();

            return new DummyTransferResult();
        }
    }

    public class DummyServer : ClientProtocolEndpoint {
        public DummyServer(Socket socket, Protocol protocol) : base(socket, protocol) {
        }

        public override TransferResult transfer() {
            throw new NotImplementedException();
        }
    }

    public class DummyHandshakeResult : HandshakeResult {

            public enum HResult {
                ACCEPTED,
                REFUSED,
                AUTHN_ERROR
            };

            private HResult result;
            public HResult Result {
                get {
                    return result;
                }
            }
        }

       public class DummyTransferResult : TransferResult {

        }
    }