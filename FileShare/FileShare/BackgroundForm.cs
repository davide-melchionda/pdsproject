using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FileShare {
    public partial class BackgroundForm : Form {

        private Window notificationWindow;
        int lastDeactivateTick;
        bool lastDeactivateValid;
        public enum ErrorNotificationType {
            Receiving, Sending, File, Path
        };

        public BackgroundForm() {
            InitializeComponent();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {

        }

        private void notifyIcon1_Click(object sender, EventArgs e) {

        }

        /**
         * When a click on the notify icon occours the notification window must be created
         * and showed in foreground to the user. This window must be Active and its behaviour 
         * is to close when a click outside itself occours.
         */
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {

            // If the click was performed with the left button
            if (e.Button != MouseButtons.Right && notificationWindow == null) {
                if (lastDeactivateValid && Environment.TickCount - lastDeactivateTick < 200) return;

                // Create a new window
                notificationWindow = new NotificationWindow();

                // Register a callback on the Deactivated event (when a click outside the
                //  window occours)
                notificationWindow.Deactivated += (Object window, EventArgs args) => {
                    // Close the window and set the variable to null
                    lastDeactivateTick = Environment.TickCount;
                    lastDeactivateValid = true;
                    notificationWindow.Close();
                    notificationWindow = null;
                };

                /* Compute the position of the window beore showing it */
                var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
                var x = System.Windows.Forms.Cursor.Position.X;
                notificationWindow.Top = desktopWorkingArea.Bottom - notificationWindow.Height;
                if (((double)x) + (notificationWindow.Width / 2) >= desktopWorkingArea.Right)
                    notificationWindow.Left = desktopWorkingArea.Right - notificationWindow.Width - 10;
                else
                    notificationWindow.Left = ((double)x) - (notificationWindow.Width / 2);

                /* Show the window in front (nothing above) and activate it */
                notificationWindow.Topmost = true;
                notificationWindow.Show();
                notificationWindow.Activate();
            }
        }

        /// <summary>
        /// Show a balloon tip to notify an error condition to the user
        /// </summary>
        /// <param name="type">Which kind of error occourred.</param>
        internal void NotifyError(ErrorNotificationType type) {
            switch (type) {
                case ErrorNotificationType.Receiving:
                    notifyIcon1.ShowBalloonTip(5000, "Errore durante la ricezione", "Alcuni trasferimenti in ricezione non sono andati a buon fine.", ToolTipIcon.Error);
                    break;
                case ErrorNotificationType.Sending:
                    notifyIcon1.ShowBalloonTip(5000, "Errore durante l'invio", "Alcuni trasferimenti in uscita non sono andati a buon fine.", ToolTipIcon.Error);
                    break;
                case ErrorNotificationType.File:
                    notifyIcon1.ShowBalloonTip(5000, "Errore durante l'invio", "Il file è aperto da un altro processo", ToolTipIcon.Error);
                    break;
                case ErrorNotificationType.Path:
                    notifyIcon1.ShowBalloonTip(5000, "Errore durante la ricezione", "Il percorso specificato non è valido", ToolTipIcon.Error);
                    break;
            }
        }

        /**
        * When the option 'Exit' is selected in the context menu, the application is
        * shut down.
        */
        private void SettingsToolStripMenuItem_Click(object sender, System.EventArgs e) {

            SettingsWindow sw = SettingsWindow.Instance;
            sw.Show();


        }

        private void ExitToolStripMenuItem_Click(object sender, System.EventArgs e) {
            base.OnClosed(e);
            System.Windows.Application a = System.Windows.Application.Current;

            if (FileShareDataContext.Instance.receivingJobs.Count != 0 || FileShareDataContext.Instance.sendingJobs.Count != 0) //chiedi all'utente se è sicuro di annullare i trasferimenti in corso
            {
                CloseWindow cw = new CloseWindow();
                cw.ShowDialog();
            } else {
                a.Shutdown();
            }

        }

        private void ShowToolStripMenuItem_Click(object sender, System.EventArgs e) {
            //notifyWindow.Show();
            // Create a new window
            notificationWindow = new NotificationWindow();

            // Register a callback on the Deactivated event (when a click outside the
            //  window occours)
            notificationWindow.Deactivated += (Object window, EventArgs args) => {
                // Close the window and set the variable to null
                notificationWindow.Close();
                notificationWindow = null;
            };

            /* Compute the position of the window beore showing it */
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            var x = System.Windows.Forms.Cursor.Position.X;
            notificationWindow.Top = desktopWorkingArea.Bottom - notificationWindow.Height;
            if (((double)x) + (notificationWindow.Width / 2) >= desktopWorkingArea.Right)
                notificationWindow.Left = desktopWorkingArea.Right - notificationWindow.Width - 10;
            else
                notificationWindow.Left = ((double)x) - (notificationWindow.Width / 2);

            /* Show the window in front (nothing above) and activate it */
            notificationWindow.Topmost = true;
            notificationWindow.Show();
            notificationWindow.Activate();
        }
    }
}
