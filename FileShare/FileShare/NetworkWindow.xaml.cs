using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per NetworkWindow.xaml
    /// </summary>
    public partial class NetworkWindow : MetroWindow {
        public NetworkWindow() {
            InitializeComponent();

            NetworkSetupPage page = new NetworkSetupPage();
            NetworkFrame.Navigate(page);

            page.OnGoOn += () => {
                Close();
            };

        }
    }
}
