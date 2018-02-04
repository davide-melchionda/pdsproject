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

                // Infos of the process
                firstProc.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase), @"FileShare.exe");

                bool created;   // will be true if mutex created by me
                Mutex m = new Mutex(true, "_startFileShareProcMutex", out created); // try to create a new mutex
                if (created) {  // if created, I own it
                    // I own the mutex. I have the responsibility to run FileShare
                    DialogResult result = MessageBox.Show("Devi prima avviare l'applicazione File Share per inviare file. Vuoi avviarla?", "Attenzione",
                                                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    // If user accept
                    if (result == DialogResult.OK)
                        firstProc.Start();  // Start the process
                    
                    m.ReleaseMutex();
                }
            } else {
                try {
                    // Send path on the Pipe
                    PipeModule.Push(args[0]);
                } catch (TimeoutException e) {  // In this case app File Share is not responding on the pipe
                    // As above: the first own the mutex and manage the shown of the dialog
                    bool created;
                    Mutex m = new Mutex(true, "_showErrorDialogOnPipeTimeoutExceptionMutex", out created);
                    if (created) { 
                        MessageBox.Show("Impossibile contattare l'applicazione. Verifica File Share sia in esecuzione e configurato.", "Impossibile conttare File Share",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                        m.ReleaseMutex();
                    }
                }
            }

            // When process ends the CloseHandle() is automatically called
        }
    }
}