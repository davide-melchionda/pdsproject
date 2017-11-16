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
        //private bool isWindowVisible;

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
            if (e.Button != MouseButtons.Right) {
                
                // If there is no notification window actually shown
                if (notificationWindow == null) {
                    
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
                    
                } else {    // ... otherwise, if a window already is shown
                    // Close the window and set the field to null
                    notificationWindow.Close();
                    notificationWindow = null;
                    // N.B. This case never appen
                }
            }
        }

        /**
         * When the option 'Exit' is selected in the context menu, the application is
         * shut down.
         */
                    private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
