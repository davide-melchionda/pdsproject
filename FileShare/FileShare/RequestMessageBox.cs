using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileShare
{
    class RequestMessageBox
    {
        public static bool show(FileTransfer.Task task)
        {
            if (MessageBox.Show("Do you want to accept " + task.Info.Name + " of size: " + BytesToMegabytes(task.Info.Size) + " kb", task.Sender +
                 " wants to send you a file", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes) return true;
            return false;
        }
        private static double BytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}
