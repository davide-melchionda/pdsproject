﻿using FileShare;
using Microsoft.WindowsAPICodePack.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

/**
 * This class represents the configuration of the entire application.
 * It implements the Singleton pattern, and the fields of the unique instance
 * of this class are the parameter of the configuration retrieved from a file
 * at the boot of the application.
 */
[Serializable]

public class Settings : System.ComponentModel.INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
        var handler = PropertyChanged;
        if (handler != null)
            handler(this, new PropertyChangedEventArgs(propertyName));
    }
    /**
     * SINGLETON CREATIONAL PATTERN
     * The unique instance of the class.
     */
    private static Settings instance;
    /**
     * The object on which acquire the lock in order to create the instance
     */
    private static object syncRoot = new Object();
    /**
     * SINGLETON CREATIONAL PATTERN
     * The property which represents the unique instance of the class.
     */
    public static Settings Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new Settings();
                }
            }
            return instance;
        }
    }

    /**
     * The instance of Peer class which contains all the
     * informations about the current user.
     */
    private Peer localPeer;
    /**
     * localPeer property
     */
    public Peer LocalPeer {
        get {
            return localPeer;
        }
    }

    /**
     * The port on which the hello protocol works for sending 
     * multicast packet.
     */
    private int mcastHelloPort = /* DEFAULT */ 17177;
    /**
     * mcastHelloPort property
     */
    public int MCAST_HELLO_PORT {
        get {
            return mcastHelloPort;
        }
        set {
            mcastHelloPort = value;
        }
    }

    /**
     * The multicast addres on which the incoming socket will receive
     */
    private IPAddress mcastHelloIPAddress = /* DEFAULT */ IPAddress.Parse("234.56.78.90");
    /**
     * mcastHelloIPAddress property
     */
    public IPAddress MCAST_HELLO_IP_ADDRESS {
        get {
            return mcastHelloIPAddress;
        }
        set {
            mcastHelloIPAddress = value;
        }
    }

    /**
     * Packet buffer size (maximum number of characters in a packet
     */
    private int bufSize = /* DEFAULT */ 65535;    // Max datagram size KB
    /**
     * bufSize property
     */
    public int BUFSIZE {
        get {
            return bufSize;
        }
        set {bufSize = value;
        }
    }

    /**
     * The max age (in milliseconds) of an entry in the peers table.
     * If the timestamp is not updated for this period of time, the entry
     * can be considered osolete
     */
    private int maxageMillis = 20000;
    /**
     * maxageMillis property
     */
    public int MAXAGE_MILLIS {
        get {
            return maxageMillis;
        }
    }

    /**
     * Number of milliseconds the cleanup thread must sleep.
     */
    private int helloCleanupSleepTime = 20000;
    /**
     * helloCleanupSleepTime property
     */
    public int HELLO_CLEANUP_SLEEP_TIME {
        get {
            return helloCleanupSleepTime;
        }
    }


    /**
     * Number of milliseconds between two Keepalive packets
     */
    private int helloInterval = 10000;
    /**
     * helloInterval property
     */
    public int HELLO_INTERVAL {
        get {
            return helloInterval;
        }
        set {
            helloInterval = value;
        }
    }

    /**
     * The port on which the tcp server will accept connection requests.
     */
    private int servAcceptingPort = /* DEFAULT */ 7070;
    /**
     * servAcceptingPort property
     */
    public int SERV_ACCEPTING_PORT {
        get {
            return servAcceptingPort;
        }
    }

    public void updatePeerAddress(string newAddress) {
        localPeer.Ipaddress = newAddress;
        localPeer.Id = localPeer.Name + "_" + localPeer.Ipaddress;// + ":" + DateTime.Now;
    }

    private int tcpReceivingPort = /* DEFAULT */ 9000;
    /**
     * tcpReceivingPort property
     */
    public int TCP_RECEIVING_PORT {
        get {
            return tcpReceivingPort;
        }
        set {
            tcpReceivingPort = value;
        }
    }

    private bool autoAcceptFiles = /* DEFAULT */ true;
    /**
     * autoAcceptFiles property
     */
    public bool AutoAcceptFiles {
        get {
            return autoAcceptFiles;
        }
        set {
            autoAcceptFiles = value;
            NotifyPropertyChanged();

        }
    }

    private bool isInvisible = /* DEFAULT */ false;

    /**
     * If the state is invisible or not. In invisible state the application doesn't show itself to the network
     */
    public bool IsInvisible {
        get {
            return isInvisible;
        }
        set {
            isInvisible = value;
            NotifyPropertyChanged();

        }
    }
    private bool dontShowSetup = /* DEFAULT */ false;

    /**
   * If the state is invisible or not. In invisible state the application doesn't show itself to the network
   */
    public bool DontShowSetup {
        get {
            return dontShowSetup;
        }
        set {
            dontShowSetup = value;
            NotifyPropertyChanged();

        }
    }

    /**
     * The default path on the file system in which save settings file.
     */
    private string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\File Share";
    /**
     * defaultRecvPath property
     */
    public string AppDataPath {
        get {
            return appDataPath;
        }
        set {
            appDataPath = value; ;
            NotifyPropertyChanged();
        }
    }

    /**
     * The default path on the file system in which save received files.
     */
    private string defaultRecvPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    /**
     * defaultRecvPath property
     */
    public string DefaultRecvPath {
        get {
            return defaultRecvPath;
        }
        set {
            if (!String.IsNullOrEmpty(value)) {
                defaultRecvPath = value;
                NotifyPropertyChanged();
            }
        }
    }


    /**
    * Current Username
    */


    /**
    * defaultRecvPath property
    */
    public string CurrentUsername {
        get {
            return this.LocalPeer.Name;
        }
        set {
            if (!String.IsNullOrEmpty(value)) {
                this.LocalPeer.Name = value;
                NotifyPropertyChanged();
            }
        }
    }

    /// <summary>
    /// The default size for all icons used as profile pictures for the peers
    /// </summary>
    private Size iconSize = new Size(128, 128);
    /// <summary>
    /// Property binded to iconSize private instance field
    /// </summary>
    public Size IconSize {
        get {
            return iconSize;
        }
        set {
            iconSize = value;
        }
    }

    /// <summary>
    /// All the available netowrks
    /// </summary>
    public List<NetworkInfo> AvailableNetworks {
        get {
            List<NetworkInfo> netInfos = new List<NetworkInfo>();
            // get all nics
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            //Get the networks that are currently connected to
            var networks = NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected);

            // Filter
            foreach (Network network in networks) {
                foreach (NetworkConnection conn in network.Connections) {
                    foreach (NetworkInterface nic in nics) {
                        if (("{" + conn.AdapterId.ToString() + "}").ToUpper() == nic.Id.ToUpper())
                            netInfos.Add(new NetworkInfo(network, nic));
                    }
                }
            }

            return netInfos;
        }
    }

    //public List<NetworkInterface> MachineNetworkInterfaces {
    //    get {
    //        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
    //        List<NetworkInterface> interfaces = new List<NetworkInterface>();
    //        foreach (NetworkInterface n in nics)
    //            if (n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
    //                n.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
    //                //if (n.SupportsMulticast && n.Description.Netw)
    //                    interfaces.Add(n);
    //            }
    //        return interfaces;
    //    }
    //} 

    /// <summary>
    /// Path in the file system of the icon image used as profile
    /// picture by the user.
    /// </summary>
    private string picturePath;
    /// <summary>
    /// Property linked to picturePath field
    /// </summary>
    public string PicturePath {
        get {
            return picturePath;
        }
        set {
            if (!String.IsNullOrEmpty(value) && File.Exists(value)) {
                Bitmap tmpBitmap;
                try {
                    tmpBitmap = new Bitmap(value);
                    LocalPeer.ByteIcon = ImageAdapter.ByteArrayFromImage(tmpBitmap, MaxThumbnailPictureMemorySize);
                    tmpBitmap.Dispose();
                    picturePath = value;
                    NotifyPropertyChanged();
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }

            }
        }
    }

    /// <summary>
    /// The maximum size of a picture. This limit is necessary to allow the picture be sent 
    /// inside a UDP datagram.
    /// </summary>
    public int MaxThumbnailPictureMemorySize {
        get {
            return 40 * 1024;   /* DEFAULT */
        }
    }

    public Resources Resources { get; set; }
    public int UsernameMaxLength {
        get {
            return 20;   /* DEFAULT */
        }
    }

    /// <summary>
    /// The name of the network on which the user decided to operate.
    /// </summary>
    public NetworkInfo Network {
        get;
        internal set;
    }

    /**
     * SINGLETON CREATIONAL PATTERN
     * The protected constructor.
     * Retrieves the settings from a file.
     */
    protected Settings() {

        // TODO
        // Retrieve configuration from a file

        // For now: the local peer is initialized with dumb values
        // It will be corrected as soon as possible when network information will be available
        localPeer = new Peer(); // /*"Anonymouse user" + ":" + new Random().Next(), */"Anonymouse user", "unknown_ip");
        localPeer.Name = "Anonimous user";
        localPeer.Ipaddress = "unknown_ip";
        Resources = new Resources();
    }

}