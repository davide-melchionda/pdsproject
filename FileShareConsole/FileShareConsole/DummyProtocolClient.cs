using System;
using System.Net.Sockets;
using NetworkTransmission;
using Protocol;
using static StorageModule;
using System.Text;
using static Protocol.ProtocolEndpoint;
using FileShareConsole;
using System.IO;

namespace FileTransfer {

    public class DummyClient : ClientProtocolEndpoint {

        private FileIterator iterator;
        private Task task;

        public DummyClient(Socket socket, FileIterator iterator, Task task) : base(socket) {
            this.iterator = iterator;
            this.task = task;
        }

        public override HandshakeResult handshake() {
            throw new NotImplementedException();
        }

        public override TransferResult transfer() {
            byte[] buf = new Byte[JobZipStorageModule.READ_BLOCK_SIZE+1];
            while (iterator.hasNext()) {
                int read = iterator.next(buf);
                FileStream f = File.Create(@"C:\Users\vm-dm-win\Desktop\exampleFile.txt");
                f.Write(buf, 0, read);
                f.Close();
            }

            System.Threading.Thread.Sleep(10000);

            return new DummyTransferResult();
        }
    }

    public class DummyServer : ClientProtocolEndpoint {
        public DummyServer(Socket socket) : base(socket) {
        }

        public override HandshakeResult handshake() {
            throw new NotImplementedException();
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