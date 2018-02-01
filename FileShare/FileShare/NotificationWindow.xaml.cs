using MahApps.Metro.Controls;
using System.Windows;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class NotificationWindow : MetroWindow {

        NotificationPage page;

        public NotificationWindow() {
            InitializeComponent();

            page = new NotificationPage();
            NotificationFrame.Navigate(page);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            SettingsWindow sw = SettingsWindow.Instance;
            sw.Show();
        }

        private void Clear_All_Click(object sender, RoutedEventArgs e) {
            page.ClearAllCompleted();
        }
    }

}

