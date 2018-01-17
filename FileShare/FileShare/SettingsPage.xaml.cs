using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page {

        [DllImport("shell32.dll", EntryPoint = "#261",
            CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void GetUserTilePath(string username,
                                                  UInt32 whatever, // 0x80000000
                                                  System.Text.StringBuilder picpath, int maxLength);

        private Settings settings;
        public delegate void Close();
        public event Close OnClosed;

        public SettingsPage() {
            InitializeComponent();

            settings = Settings.Instance;
            DataContext = new { Settings = settings, Me = new ListedPeer(settings.LocalPeer) };

        }


        private void Close_Button_Click(object sender, RoutedEventArgs e) {
            OnClosed();
        }

        private void Path_Button_Click(object sender, RoutedEventArgs e) {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = DefaultPath.Text;
            dialog.ShowDialog();
            settings.DefaultRecvPath = dialog.SelectedPath;
        }

        private void Profile_Button_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "File immagine|*.jpg;*.jpeg;*.png;*.ico;*.bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
                settings.PicturePath = dialog.FileName;
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
            var sb = new System.Text.StringBuilder(1000);
            GetUserTilePath(username, 0x80000000, sb, sb.Capacity);
            return sb.ToString();
        }

        private static string GetUserTile(string username) {
            return GetUserTilePath(username);
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
                    if (!System.Char.IsLetterOrDigit(ch))
                    {
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
