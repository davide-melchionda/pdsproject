
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public class Peer : INotifyPropertyChanged{

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var handler = PropertyChanged;
        if (handler != null)
            handler(this, new PropertyChangedEventArgs(propertyName));
    }
    
    /**
     * Unique identifier of the peer in the network.
     */
    private string id;

    /**
     * The name of the peer in the network. It's not a key
     * for the peer.
     */
    private string name;

    /**
     * The ip address of a peer in the network. This information
     * should not be directly used to forward traffic to the peer.
     */
    private string ipaddress;

   
    /**
     * Properties coresponding to the fields.
     */
    public string Id {
        get {
            return id;
        }
        set {
            id = value;
            NotifyPropertyChanged();
        }
    }

    public string Name {
        get {
            return name;
        }
        set {
            name = value;
            NotifyPropertyChanged();
        }
    }

    public string Ipaddress {
        get {
            return ipaddress;
        }
        set {
            ipaddress = value;
        }
    }


    //private ImageSource icon;
    //public ImageSource Icon {
    //    get {
    //        if (icon == null)
    //            return new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\user.ico"));
    //        return icon;
    //    }
    //    set {
    //        icon = value;
    //        NotifyPropertyChanged();
    //    }
    //}

    /// <summary>
    /// Byte array which represents the profile picture of this peer
    /// </summary>
    private byte[] byteIcon;
    /// <summary>
    /// Property associeted to byteIcon
    /// </summary>
    public byte[] ByteIcon {
        get {
            //// Then sets byteIcon
            //byte[] bytes = null;
            //if (icon != null) {
            //    BitmapSource bitmapSource = icon as BitmapSource;
            //    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            //    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            //    using (var stream = new MemoryStream()) {
            //        encoder.Save(stream);
            //        bytes = stream.ToArray();
            //    }
            //}
            //return bytes;
            return byteIcon;
        }
        set {
            //// if no byte: no icon set
            //if (value.Length == 0) {
            //    Icon = null;
            //    return;
            //}

            //// Sets byte icon
            //BitmapImage biImg = new BitmapImage();
            //MemoryStream ms = new MemoryStream(value);
            //biImg.BeginInit();
            //biImg.StreamSource = ms;
            //biImg.EndInit();

            //Icon = biImg as BitmapImage;
            byteIcon = value;
            NotifyPropertyChanged();
        }
    }
    
    public Peer() { }

    public Peer(string id, string name, string ipaddress) {
        Id = id;
        Name = name;
        Ipaddress = ipaddress;
        byteIcon = new byte[0];
        NotifyPropertyChanged();
    }

    public Peer(string id, string name, string ipaddress, ImageSource icon) {
        Id = id;
        Name = name;
        Ipaddress = ipaddress;
        byteIcon = new byte[0];
        NotifyPropertyChanged();
    }

}