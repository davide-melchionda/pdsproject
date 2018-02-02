using System.Net.NetworkInformation;
using System.Windows;
using System.Linq;
using System.Windows.Controls;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per NetworkSetupPage.xaml
    /// </summary>
    public partial class NetworkSetupPage : Page {

        public delegate void Close();
        public event Close OnClosed;

        public delegate void GoOn();
        public event GoOn OnGoOn;

        public NetworkSetupPage() {

            InitializeComponent();

            DataContext = new {
                nets = Settings.Instance.AvailableNetworks
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            foreach (NetworkInfo net in netlist.SelectedItems.Cast<NetworkInfo>().ToList())
                Settings.Instance.Network = net;   // It will be only one due to the list selection config
            OnGoOn?.Invoke();   // go on
        }

    }
}
