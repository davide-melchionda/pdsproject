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
        public delegate void Close();
        public event Close OnClosed;

        public SettingsPage() {
            InitializeComponent();

            settings = Settings.Instance;
            DataContext = new { Settings = settings,  Me = new ListedPeer(settings.LocalPeer) };

        }


        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            OnClosed();  
        }

        private void Path_Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = DefaultPath.Text;
            dialog.ShowDialog();
            settings.DefaultRecvPath = dialog.SelectedPath;
        }

        private void Profile_Button_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "File immagine|*.jpg;*.jpeg;*.png;*.ico;*.bmp" ;
            if (dialog.ShowDialog() == DialogResult.OK)
                settings.PicturePath = dialog.FileName;
        }

    }
}
