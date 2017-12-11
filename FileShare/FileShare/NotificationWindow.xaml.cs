using MahApps.Metro.Controls;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class NotificationWindow : MetroWindow
    {

        public NotificationWindow() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            SettingsWindow sw = SettingsWindow.Instance;
            sw.Show();
        }
    }

}

