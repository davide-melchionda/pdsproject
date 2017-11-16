using FileTransfer;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Shapes;

namespace FileShare
{
    /*
      * Pagina  selezione dei peer destinati a ricevere un file o directory, la conferma della selezione
      * tramite bottone scatena un evento al quale è registro chi si occupa di chiamare JobScheduler
      */
    public partial class SelectionPage : Page
    {

        public delegate void onPeersSelected(List<Peer> peers, string path);
        public event onPeersSelected OnselectHappened;
        SelectionWindow parent;
        public SelectionPage(SelectionWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
            DataContext = new FileShareDataContext();
            this.fileName.Text = (parent.File);
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            List<Peer> selected = null;
            if (thelist.SelectedItems.Count > 0)
            {
                selected = thelist.SelectedItems.Cast<Peer>().ToList();
                parent.Close();

                OnselectHappened?.Invoke(selected, this.parent.File);

            }
            else MessageBox.Show("Devi selezionare almeno un destinatario");
        }
    }
}