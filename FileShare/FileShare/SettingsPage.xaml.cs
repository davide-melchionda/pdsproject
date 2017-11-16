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
        Settings settings;
        public SettingsPage()
        {
            settings=Settings.Instance;
            DataContext = this;
            InitializeComponent();
            this.AutoReceiveCB.IsChecked = settings.AutoAcceptFiles;
            this.invisibleStateCB.IsChecked = settings.IsInvisible;
        }
        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            settings.AutoAcceptFiles = (bool)AutoReceiveCB.IsChecked;
            settings.AutoAcceptFiles = (bool)invisibleStateCB.IsChecked;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            settings.CurrentUsername = UsernameTB.Text;
            settings.AutoAcceptFiles = (bool)AutoReceiveCB.IsChecked;
            settings.IsInvisible = (bool)invisibleStateCB.IsChecked;


        }
    }
}
