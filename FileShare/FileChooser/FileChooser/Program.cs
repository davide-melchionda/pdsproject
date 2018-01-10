using System.IO.Pipes;
using System.IO;
using FileShareConsole;
using System.Diagnostics;
using System.Threading;
using System;
using System.Windows.Forms;

namespace FileChooser {
    class Program {
        static void Main(string[] args) {

            if (Process.GetProcessesByName("FileShare").Length == 0) {

                Process firstProc = new Process();

                //al momento uso il mio percorso statico, la versione finale userà il percorso definito nell'installer
                //qui devi definire il tuo percorso
                //firstProc.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), @"File Share.exe");//@"C:\Users\franc\Documents\GitKraken\pdsproject\FileShare\bin\Debug\FileShare.exe";
                firstProc.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase), @"FileShare.exe");

                bool created;
                Mutex m = new Mutex(true, "_startFileShareProcMutex", out created);
                if (created) {
                    DialogResult result = MessageBox.Show("Devi prima avviare l'applicazione File Share per inviare file. Vuoi avviarla?", "Attenzione",
                                                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.OK)
                        firstProc.Start();
                }
            } else {
                try {
                    PipeModule.Push(args[0]);
                } catch (TimeoutException e) {
                    // TODO What should we do here? Probably we should show a dialog
                }
            }
        }
    }
}