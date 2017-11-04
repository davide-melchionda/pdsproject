using System;

namespace FileChooser
{
    class PipesDaemon : ExecutableThread
    {
        protected PipesDaemon()
        {
        }

        protected override void execute()
        {
            while (true)
            {

                /* INVIA QUESTO VALORE A QUALCUNO----> */
                Console.WriteLine(PipeModule.Pop());
            }
        }
    }
}
