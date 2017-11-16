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

        private Window notifyWindow;
        private bool isWindowVisible;

        public BackgroundForm(Window notifyWindow) {
            InitializeComponent();

            this.notifyWindow = notifyWindow;
            notifyWindow.Hide();
            isWindowVisible = false;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {

        }

        private void notifyIcon1_Click(object sender, EventArgs e) {

        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
            if (isWindowVisible) {
                notifyWindow.Hide();
                isWindowVisible = false;
            } else {
                notifyWindow.Show();
                isWindowVisible = true;
            }
        }
        private void SettingsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

            SettingsWindow sw = new SettingsWindow();
            sw.Show();


        }

        private void ExitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            base.OnClosed(e);
            System.Windows.Application a = System.Windows.Application.Current;
            a.Shutdown();
        }

        private void ShowToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            notifyWindow.Show();
        }
    }
}
