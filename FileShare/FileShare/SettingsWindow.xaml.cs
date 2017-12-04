using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace FileShare
{
   
    public partial class SettingsWindow : Window
    {
      
        public SettingsWindow()
        {
            InitializeComponent();
            SettingsPage page = new SettingsPage();
            page.OnClosed += Page_OnClosed;
            this.SettingsFrame.Navigate(page);
        }

        private void Page_OnClosed()
        {
            this.Close();        }

        

    }
}
