using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/**
 * This class represents the configuration of the entire application.
 * It implements the Singleton pattern, and the fields of the unique instance
 * of this class are the parameter of the configuration retrieved from a file
 * at the boot of the application.
 */
class Settings {

    /**
     * SINGLETON CREATIONAL PATTERN
     * The unique instance of the class.
     */
    private static Settings instance;
    /**
     * SINGLETON CREATIONAL PATTERN
     * The property which represents the unique instance of the class.
     */
    public static Settings Instance {
        get {
            if (instance == null)
                instance = new Settings();
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
        set {
            // TODO
            // Try to write on file the new setting configurations
            // Throw an exception if something goes wrong

            // ... after the file update
            localPeer = value;
        }
    }

    /**
     * The port on which the hello protocol works for sending 
     * multicast packet.
     */
    private int mcastHelloPort = /* DEFAULT */ 8888;
    /**
     * mcastHelloPort property
     */
    public int MCAST_HELLO_PORT {
        get {
            return mcastHelloPort;
        }
        set {
            // TODO
            // Try to write on file the new setting configurations
            // Throw an exception if something goes wrong

            // ... after the file update
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
            // TODO
            // Try to write on file the new setting configurations
            // Throw an exception if something goes wrong

            // ... after the file update
            mcastHelloIPAddress = value;
        }
    }

    /**
     * Packet buffer size (maximum number of characters in a packet
     */
    private int bufSize = /* DEFAULT */ 256;
    /**
     * bufSize property
     */
    public int BUFSIZE {
        get {
            return bufSize;
        }
        set {
            // TODO
            // Try to write on file the new setting configurations
            // Throw an exception if something goes wrong

            // ... after the file update
            bufSize = value;
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

    public void updatePeerAddress(string newAddress) {
        localPeer.Ipaddress = newAddress;
        localPeer.Id = localPeer.Name + ":" + localPeer.Ipaddress;
    }

    /**
     * SINGLETON CREATIONAL PATTERN
     * The protected constructor.
     * Retrieves the settings from a file.
     */
    protected Settings() {

        // TODO
        // Retrieve configuration from a file

        // For now: the local peer is randomly initialized
        localPeer = new Peer("Davide:"+new Random().Next(), "Davide", "unknown_ip");

    }

}