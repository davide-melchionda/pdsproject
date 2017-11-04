using System;

namespace FileShareConsole
{
    /**
     * The thread that actually listen for incoming request from the FileChooser via IPC
     */
    class PipeDaemon : ExecutableThread
    {
        public delegate void onPopCallbackType(string fileName);
        public event onPopCallbackType popHappened;

        protected override void execute()
        {
            while (true)
            {
                popHappened?.Invoke(PipeModule.Pop());
            }
        }
    }
}
