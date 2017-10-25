using System;

namespace FileShareConsole
{
   /**
    * The thread that actually listen for incoming request from the FileChooser via IPC
    */
    class PipeDaemon : ExecutableThread
    {
      
        protected override void execute()
        {
            while (true)
            {

                /* INVIA QUESTO VALORE ALLA GUI PERCHé LO USER POSSA REALIZZARE UNO O PIù TASK LEGATI A QUESTO FILE selezionando i riceventi> */
                Console.WriteLine(PipeModule.Pop());
            }
        }
    }
}
