using MahApps.Metro.Controls;
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
    public partial class ProfileSetupWindow : MetroWindow
    {
        public ProfileSetupWindow()
        {
            InitializeComponent();
            ProfileSetupIntroPage firstPage = new ProfileSetupIntroPage();
            //this.SetupFrame.Navigate(firstPage);
            this.Content = firstPage;
            firstPage.OnGoOn += () => {
                ProfileSetupPage page = new ProfileSetupPage();
                page.ShowsNavigationUI = false;
                //this.SetupFrame.Navigate(page);
                this.Content = page;
                page.OnClosed += Page_OnClosed;
            };
        }

        private void Page_OnClosed()
        {
            this.Close();
        }
    }
}
