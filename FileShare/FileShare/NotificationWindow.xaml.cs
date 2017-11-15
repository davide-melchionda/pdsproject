using FileShareConsole;
using FileTransfer;
using HelloProtocol;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window {
        private Page1 page;
        public NotificationWindow() {
            InitializeComponent();
             Page = new Page1();
            NotificationFrame.Navigate(page);

        }

        public Page1 Page { get => page; set => page = value; }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
             
            Application.Current.Shutdown();
        }
        
    }
}
