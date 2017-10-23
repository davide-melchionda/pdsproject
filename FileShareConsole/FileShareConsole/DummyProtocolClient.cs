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

        private FileIterator iterator;
        private Task task;

        public DummyClient(Socket socket, Protocol protocol, FileIterator iterator, Task task) : base(socket, protocol) {
            this.iterator = iterator;
            this.task = task;
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