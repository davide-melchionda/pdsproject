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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per Page1.xaml
    /// </summary>
    public partial class NotificationPage : Page {

        public NotificationPage() {
            InitializeComponent();

            DataContext = FileShareDataContext.Instance;
        }

        internal void ClearAllCompleted()
        {
            (DataContext as FileShareDataContext).ClearAllCompleted();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e) {

        }

        private void ListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e) {

        }

        /// <summary>
        /// Process the operation of deleting a job.
        /// Need to be async so to avoid blocked gui.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void DeactivateJob(object sender, RoutedEventArgs args) {
            Button button = sender as Button;
            ListedJob item = button.DataContext as ListedJob;
            FileShareDataContext f = DataContext as FileShareDataContext;
            await Task.Run(() => {
                f.DeactivateJob(item); 
            });
        }

        private void ProgBarLoaded(object sender, RoutedEventArgs args) {
            ProgressBar prog = sender as ProgressBar;
            (DataContext as FileShareDataContext).manageProgressBar(prog);
        }

    }
}
