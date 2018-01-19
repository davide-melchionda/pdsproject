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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per Page1.xaml
    /// </summary>
    public partial class ProfileSetupIntroPage : Page {

        public delegate void GoOn();
        public event GoOn OnGoOn;

        public ProfileSetupIntroPage() {
            InitializeComponent();

            DataContext = new {
                Resources = Settings.Instance.Resources
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            OnGoOn?.Invoke();
        }
    }
}
