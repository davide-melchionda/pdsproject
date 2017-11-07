using System;
using System.IO;
using System.IO.Pipes;

namespace FileShareConsole
{
    /**
     * The class that provides specific functions to communicate on the IPC system modelled with a Pipe
     */

    class PipeModule : IPCModule
    {
        private static string IPC_PIPE = "FileSharePipe";
        private static NamedPipeServerStream pipeServer;
        private static StreamReader reader;

        public static void InstantiateServer()
        {
            pipeServer = new NamedPipeServerStream(IPC_PIPE);
            reader = new StreamReader(pipeServer);
        }
        public static string Pop()
        {
            pipeServer.WaitForConnection();
            string receivedFileName = reader.ReadLine();
            pipeServer.Disconnect();
            return receivedFileName;
        }

        public static void Push(string input)
        {
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(IPC_PIPE);
            StreamWriter writer = new StreamWriter(pipeClient);

            pipeClient.Connect(1000);
            writer.WriteLine(input);
            writer.Flush();
            pipeClient.Close();
        }
    }
}