﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileShare
{
    class destroyedclass

    {
        public static bool Show(FileTransfer.Task task) {
            return MessageBox.Show("Do you want to accept " + task.Info[0].Name + " of size: " + BytesToMegabytes(task.Info[0].Size) + " kb", task.Sender +
                 " wants to send you a file", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes;
        }

        private static double BytesToMegabytes(long bytes) {
            return (bytes / 1024f) / 1024f;
        }
    }
}
