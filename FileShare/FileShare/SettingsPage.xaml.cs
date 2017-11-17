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

namespace FileShare
{
    /// <summary>
    /// Logica di interazione per SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private Settings settings;
        Window parent;

        public SettingsPage(Window parent)
        {
            settings = Settings.Instance;
            DataContext = this;
            InitializeComponent();
            this.parent = parent;
            UsernameTB.Text = settings.CurrentUsername;
            AutoReceiveCB.IsChecked = settings.AutoAcceptFiles;
            invisibleStateCB.IsChecked = settings.IsInvisible;

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            settings.CurrentUsername = UsernameTB.Text;
            settings.AutoAcceptFiles = AutoReceiveCB.IsChecked??false;
            settings.IsInvisible = invisibleStateCB.IsChecked??false;
            if (settings.IsInvisible)
            {
                HelloProtocol.HelloSenderThread.visibilityChange.Reset();
            }
            else
            {
                HelloProtocol.HelloSenderThread.visibilityChange.Reset();

            }
            this.parent.Close();
                
        }
    }
}
