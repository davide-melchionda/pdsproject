using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileShareConsole
{
    /**
     * The thread that actually listen for incoming request from the FileChooser via IPC
     */
    class PipeDaemon : ExecutableThread
    {
        /// <summary>
        /// The size in milliseconds of the window inside which we consider the incoming
        /// requests as part of the same logical operation.
        /// </summary>
        private const int GROUP_REQUESTS_WINDOW_MILLIS = 400;

        public delegate void onPopCallbackType(List<string> fileName);
        public event onPopCallbackType popHappened;

        protected override void execute()
        {
            PipeModule.InstantiateServer();

            ConcurrentBag<string> paths = new ConcurrentBag<string>();

            object lockForPathsList = new object();

            while (true)
            {
                //Console.Write(PipeModule.Pop());
                string s = PipeModule.Pop();
                paths.Add(s);
                if (paths.Count == 1) {
                    Task.Run(() => {
                        System.Threading.Thread.Sleep(GROUP_REQUESTS_WINDOW_MILLIS);
                        lock (lockForPathsList) {
                            popHappened?.Invoke(new List<string>(paths));
                            paths = new ConcurrentBag<string>();
                        }
                    });
                }
            }
        }
    }
}
