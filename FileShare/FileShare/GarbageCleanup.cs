using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileShare
{
    class GarbageCleanup : ExecutableThread
    {
        private static string temporaryFilesDirectory = Settings.Instance.AppDataPath + "\\temp";
    
        protected override void execute()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(temporaryFilesDirectory);
            if (di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }
    }
}
