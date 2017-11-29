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
using System.Windows.Shapes;

namespace FileShare
{
    /// <summary>
    /// Logica di interazione per ProfileSetupWindow.xaml
    /// </summary>
    public partial class ProfileSetupWindow : Window
    {
        public ProfileSetupWindow()
        {
            InitializeComponent();
            ProfileSetupPage page = new ProfileSetupPage();
            page.OnClosed += Page_OnClosed;
            this.SetupFrame.Navigate(page);
        }

        private void Page_OnClosed()
        {
            this.Close();
        }
    }
}
