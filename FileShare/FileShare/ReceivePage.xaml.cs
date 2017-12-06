using FileTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace FileShare
{

    public partial class ReceivePage : Page
    {
        public delegate void Close();
        public event Close OnClosed;
        private ToAccept request;
        private Settings settings;
        private string taskType;
        public ReceivePage(ToAccept request)
        {
            InitializeComponent();
            this.Request = request;
            this.Settings = Settings.Instance;
            //if (request.TasktoAccept.Info.Type == FileInfo.FType.DIRECTORY)
            //    TaskType = "directory";
            //else TaskType = "file";
            DataContext = this;
        }
        private void Accept_Button_Click(object sender, RoutedEventArgs e)
        {
            request.Response = true;
            request.Path = ChosedPath.Text;
            OnClosed();

        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            request.Response = false;
            request.Path = null;
            OnClosed();

        }



        private void Path_Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = ChosedPath.Text;
            dialog.ShowDialog();
            ChosedPath.Text = dialog.SelectedPath;
        }



        public Settings Settings { get => settings; set => settings = value; }
        public ToAccept Request { get => request; set => request = value; }
        public string TaskType { get => taskType; set => taskType = value; }
    }
}
