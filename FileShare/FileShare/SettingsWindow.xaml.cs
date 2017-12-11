using MahApps.Metro.Controls;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace FileShare
{
   
    public partial class SettingsWindow : MetroWindow {

        private static SettingsWindow instance;
        public static SettingsWindow Instance {
            get {
                if (instance == null)
                    instance = new SettingsWindow();
                return instance;
            }
        }

        private SettingsWindow()
        {
            InitializeComponent();
            SettingsPage page = new SettingsPage();
            page.OnClosed += Page_OnClosed;
            Closed += (object sender, EventArgs e) => {
                instance = null;
            };
            this.SettingsFrame.Navigate(page);
        }

        private void Page_OnClosed() {
            instance.Close();
            instance = null;
        }

        

    }
}
