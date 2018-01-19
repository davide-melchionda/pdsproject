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
                nics = NetworkInterface.GetAllNetworkInterfaces()
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            foreach (NetworkInterface nic in netlist.SelectedItems.Cast<NetworkInterface>().ToList())
                Settings.Instance.NetworkName = nic.Name;   // It will be only one due to the list selection config
            OnGoOn?.Invoke();   // go on
        }

    }
}
