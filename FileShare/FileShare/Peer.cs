
public class Peer {
    
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
        }
    }
    public string Name {
        get {
            return name;
        }
        set {
            name = value;
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
    public Peer() { }

    public Peer(string id, string name, string ipaddress) {
        Id = id;
        Name = name;
        Ipaddress = ipaddress;
    }
}