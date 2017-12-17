using System.IO.Pipes;
using System.IO;
using FileShareConsole;
using System.Diagnostics;
using System.Threading;
using System;

namespace FileChooser
{
    class Program
    {
        static void Main(string[] args)
        {

            if (Process.GetProcessesByName("FileShare").Length == 0)
            {
                
                Process firstProc = new Process();

                //al momento uso il mio percorso statico, la versione finale userà il percorso definito nell'installer
                //qui devi definire il tuo percorso
                firstProc.StartInfo.FileName = @"C:\Users\franc\Documents\GitKraken\pdsproject\FileShare\bin\Debug\FileShare.exe";
                firstProc.Start();


            }
            Thread.Sleep(1000);
            PipeModule.Push(args[0]);
        }
    }
}