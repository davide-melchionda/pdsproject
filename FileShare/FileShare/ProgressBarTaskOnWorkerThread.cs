using FileTransfer;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WpfTutorialSamples.Misc_controls {
    public partial class ProgressBarTaskOnWorkerThread : Window {

        ProgressBar bar;
        Job j;

        public ProgressBarTaskOnWorkerThread(ProgressBar bar, Job j) {
            this.bar = bar;
            this.j = j;
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e) {
            while (true) {
                (sender as BackgroundWorker).ReportProgress((int)j.Percentage);
                if (j.Percentage == 100)
                    break;
                Thread.Sleep(100);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            bar.Value = e.ProgressPercentage;
        }
    }
}
