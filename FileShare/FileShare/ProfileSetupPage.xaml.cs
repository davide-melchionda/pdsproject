using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per ProfileSetupPage.xaml
    /// </summary>
    public partial class ProfileSetupPage : Page {

        [DllImport("shell32.dll", EntryPoint = "#261",
            CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void GetUserTilePath(string username,
                                                  UInt32 whatever, // 0x80000000
                                                  StringBuilder picpath, int maxLength);

        public delegate void Close();
        public event Close OnClosed;

        public ProfileSetupPage() {
            InitializeComponent();
            Settings settings = Settings.Instance;
            DataContext = new { Settings = settings, Me = new ListedPeer(settings.LocalPeer) };
        }

        private void Profile_Button_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            dialog.Filter = "File immagine|*.jpg;*.jpeg;*.png;*.ico;*.bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
                Settings.Instance.PicturePath = dialog.FileName;
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e) {
            OnClosed?.Invoke();
        }

        private void Import_Button_Click(object sender, RoutedEventArgs e) {
            //if (String.Compare(getOSInfo,)
            //WindowsIdentity wi = WindowsIdentity.GetCurrent();
            if (Environment.UserName.Length > Settings.Instance.UsernameMaxLength)
                Settings.Instance.CurrentUsername = Environment.UserName.Substring(0, Settings.Instance.UsernameMaxLength);//wi.Name;
            else
                Settings.Instance.CurrentUsername = Environment.UserName;

            //DirectoryInfo dir = new DirectoryInfo(Environment.GetEnvironmentVariable("AppData")+ @"\Microsoft\Windows\AccountPictures\");
            Settings.Instance.PicturePath = GetUserTile(Environment.UserName);
            Settings.Instance.PicturePath = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Temp\" + Environment.UserName + @".bmp";

        }

        private static string GetUserTilePath(string username) {   // username: use null for current user
            var sb = new StringBuilder(1000);
            GetUserTilePath(username, 0x80000000, sb, sb.Capacity);
            return sb.ToString();
        }

        private static string GetUserTile(string username) {
            return GetUserTilePath(username);
        }

        private void UsernameTB_TextChanged(object sender, TextChangedEventArgs e) {

        }
        string oldText = "";
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            if (textBox != null)
            {
                int selStart = textBox.SelectionStart;
                int selLength = textBox.SelectionLength;
                foreach (char ch in textBox.Text.ToCharArray())
                {
                    if (@"/\:*?<>|".Contains(Char.ToString(ch))) {
                        textBox.Text = oldText;
                        textBox.SelectionStart = selStart;
                        textBox.SelectionLength = selLength;
                        return;
                    }
                }
                oldText = textBox.Text;
            }
        }

    }
}
