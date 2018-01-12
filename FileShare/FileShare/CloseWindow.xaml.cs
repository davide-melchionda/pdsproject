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
    /// Logica di interazione per CloseWindow.xaml
    /// </summary>
    public partial class CloseWindow : MahApps.Metro.Controls.MetroWindow
    {
        public CloseWindow()
        {
            InitializeComponent();
            ClosePage closePage = new ClosePage();
            closePage.OnClosed += ClosePage_OnClosed; ;
            CloseFrame.Navigate(closePage);

            DataContext = Settings.Instance.Resources;
        }

        private void ClosePage_OnClosed()
        {
            this.Close();

        }
    }
}
