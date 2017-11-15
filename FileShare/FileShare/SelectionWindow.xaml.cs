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
    /*
    * Finestra che ospita la pagina di selezione dei peer destinati a ricevere un file o directory
    */
    public partial class SelectionWindow : Window
    {
        public string file;
        public SelectionPage page;

        public SelectionWindow(string filePath)
        {
            InitializeComponent();
            file = filePath;
            page = new SelectionPage(this);
            this.Page.Navigate(page);
        }

        public   string File { get => file; set => file = value; }
    }
}
