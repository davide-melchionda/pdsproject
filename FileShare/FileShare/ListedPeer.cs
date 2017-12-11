using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileShare {

    public class ListedPeer : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        
        /// <summary>
        /// Property coresponding to the peer instance field
        /// </summary>
        public Peer Peer {
            get;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="peer"></param>
        public ListedPeer(Peer peer) {
            Peer = peer;
            Icon = ImageAdapter.IconFromByteArray(Peer.ByteIcon);
            Peer.PropertyChanged += (object sender, PropertyChangedEventArgs args) => {
                if (args.PropertyName.Equals("ByteIcon"))
                    Icon = ImageAdapter.IconFromByteArray(Peer.ByteIcon);
            };
        }

        /**
         * The profile picture of the peer
         */
        private ImageSource icon;
        /**
         * Property associated to 'picture'. 
         * Manages the default picture
         */
        public ImageSource Icon {
            get {
                if (Peer.ByteIcon.Length == 0)
                    return new BitmapImage(Settings.Instance.Resources.DefaultUserPicture);
                return icon;
            }
            set {
                
                icon = value;
                NotifyPropertyChanged();
            }
        }




    }
}
