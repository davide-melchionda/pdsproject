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
    /// <summary>
    /// Logica di interazione per ClosePage.xaml
    /// </summary>
    public partial class ClosePage : Page
    {
        public delegate void Close();
        public event Close OnClosed;

        Form f;

        public ClosePage(Form f)
        {
            InitializeComponent();

            this.f = f;
        }
      
        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application a = System.Windows.Application.Current;
            //a.Shutdown();
            ((BackgroundForm)f).CloseBackgorundForm();

        }
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            OnClosed();

        }
    }
   
}
