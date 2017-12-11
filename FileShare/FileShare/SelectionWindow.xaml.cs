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
    ///<summary>
    /// This Window hosts the page which allows the user to select a list of peers designated to 
    /// receive the file or directory. This file or directory is specified through a parametere 
    /// (the file path) passed in the constructor of the window.
    ///</summary>
    public partial class SelectionWindow : MetroWindow
    {
        /// <summary>
        /// Delegate which will be the type of the evet triggered when peers will be selected.
        /// </summary>
        /// <param name="peers"></param>
        /// <param name="path"></param>
        public delegate void OnSelected(List<Peer> peers, List<string> path);
        /// <summary>
        /// The event triggered when peers are selected.
        /// </summary>
        public event OnSelected Selected;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath"></param>
        public SelectionWindow(List<string> filePaths)
        {
            InitializeComponent();
            // The page which will contain the view
            SelectionPage page = new SelectionPage(filePaths);
            page.Selected += (List<Peer> peers, List<string> paths) => {
                Selected?.Invoke(peers, paths);
            };
            Page.Navigate(page);
        }
    }
}
