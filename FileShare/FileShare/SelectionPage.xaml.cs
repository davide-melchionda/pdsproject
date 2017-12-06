using FileTransfer;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Shapes;

namespace FileShare
{
    /// <summary>
    /// Page which allow the selection of the peers designated to receive the file or directory. The 
    /// confirm on the button triggers a specific event.
    /// </summary>
    public partial class SelectionPage : Page
    {
        /// <summary>
        /// Delegate which will be the type of the evet triggered when peers will be selected
        /// </summary>
        /// <param name="peers"></param>
        /// <param name="path"></param>
        public delegate void OnPeersSelected(List<Peer> peers, List<string> paths);
        /// <summary>
        /// The event triggered when peers are selected.
        /// </summary>
        public event OnPeersSelected Selected;

        /// <summary>
        /// The path of the file for which the selection will be performed
        /// </summary>
        private List<string> filePaths;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath">The path of the file</param>
        public SelectionPage(List<string> filePaths/*SelectionWindow parent*/)
        {
            InitializeComponent();

            this.filePaths = filePaths;

            // Needed in order to show the peers list
            DataContext = FileShareDataContext.Instance;
        }

        /// <summary>
        /// On confirmation button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) {
            List<Peer> selected = new List<Peer>();
            // If at least one peer was selected
            if (thelist.SelectedItems.Count > 0) {
                /* Triggers the evet */
                foreach (ListedPeer p in thelist.SelectedItems.Cast<ListedPeer>().ToList())
                    selected.Add(p.Peer);
                Selected?.Invoke(selected, filePaths);
            } else // Otherwise the user must repeat the selection
                MessageBox.Show("Devi selezionare almeno un destinatario.");
        }
    }
}