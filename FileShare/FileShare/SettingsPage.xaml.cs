using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace FileShare
{
    /// <summary>
    /// Logica di interazione per SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page 
    {
        private Settings settings;

        public SettingsPage()
        {
            settings = Settings.Instance;
            DataContext = settings;
            InitializeComponent();
        
            

        }


        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            //settings.CurrentUsername = UsernameTB.Text;
            //settings.AutoAcceptFiles = AutoReceiveCB.IsChecked??false;
            //settings.IsInvisible = InvisibleStateCB.IsChecked??false;
            //settings.AlwaysUseDefault = alwaysDefault.IsChecked ?? false;

            //if (settings.IsInvisible)
            //{
            //    HelloProtocol.HelloSenderThread.visibilityChange.Reset();
            //}
            //else
            //{
            //    HelloProtocol.HelloSenderThread.visibilityChange.Reset();

            //}

                
        }
        private void Path_Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = DefaultPath.Text;
            dialog.ShowDialog();
            settings.DefaultRecvPath = dialog.SelectedPath;
        }
    }
}
