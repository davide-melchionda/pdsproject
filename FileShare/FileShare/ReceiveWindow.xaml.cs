using FileTransfer;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace FileShare
{

    public partial class ReceiveWindow : MetroWindow

    {
        private ToAccept receivingRequest;
        public ReceiveWindow(ToAccept request)
        {
            InitializeComponent();
            ReceivePage page = new ReceivePage(request);
            page.OnClosed += Page_OnClosed;
            this.ReceiveFrame.Navigate(page);
            this.receivingRequest = request;
        }
        private void Page_OnClosed()
        {
            this.Close();
        }

    }
}
