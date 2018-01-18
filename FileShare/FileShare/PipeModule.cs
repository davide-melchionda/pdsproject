using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace FileShareConsole {
    /**
     * The class that provides specific functions to communicate on the IPC system modelled with a Pipe
     */
    class PipeModule : IPCModule {

        private static string IPC_PIPE = "\\\\.\\pipe\\FileSharePipe";
        private static NamedPipeServerStream pipeServer;
        private static StreamReader reader;

        public delegate void OnResultReceived(string result);

        ManualResetEvent signal = new ManualResetEvent(false);

        public void InstantiateServer() {
            pipeServer = new NamedPipeServerStream(IPC_PIPE, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            reader = new StreamReader(pipeServer);
        }
        public void Pop(OnResultReceived callback) {
            //try {
            //  string returned = null;
            //    pipeServer.BeginWaitForConnection((IAsyncResult result) => {
            //        if ()
            //        returned = reader.ReadLine();
            //        pipeServer.Disconnect();
            //        callback(returned);
            //    }, returned);
            //    //string receivedFileName = 
            //    //return receivedFileName;
            //} catch (Exception e) {
            //    reader.Close();
            //    pipeServer.Close();
            //    //eeturn null;
            //}
            var asyncResult = pipeServer.BeginWaitForConnection((result) => {
                signal.Set();
            }, null);

            signal.WaitOne();
            if (asyncResult.IsCompleted) {
                pipeServer.EndWaitForConnection(asyncResult);
                // success
                string result = reader.ReadLine();
                callback(result);
                signal.Reset();
                pipeServer.Disconnect();
            }
        }

        public void Push(string input) {
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(IPC_PIPE);
            StreamWriter writer = new StreamWriter(pipeClient);

            pipeClient.Connect(1000);
            writer.WriteLine(input);
            writer.Flush();
            pipeClient.Close();
        }

        public void ClosePipe() {
            //reader.Dispose();
            //reader.Close();
            //pipeServer.Close();
            //pipeServer.EndWaitForConnection();
            signal.Set();
        }
    }
}