using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HelloProtocol {

    internal class HelloNetworkModule {

        /**
         * Define delegate to manage the packet reception.
         */
        public delegate void Handler(HelloPacket packet, string senderip);

        /**
         * SINGLETON CREATIONAL PATTERN
         * The unique instance of the class.
         */
        private static HelloNetworkModule instance;
        /**
         * SINGLETON CREATIONAL PATTERN
         * The property which represents the unique instance of the class.
         */
        public static HelloNetworkModule Instance {
            get {
                if (instance == null)
                    instance = new HelloNetworkModule();
                return instance;
            }
        }

        /**
         * The delegate to register to in order to define how
         * to manage the packet reception.
         */
        public event Handler HelloPacketReception;

        /**
         * The icoming (multicast) socket
         */
        private Socket mcastSocket;

        /**
         * The outgoing socket
         */
        private Socket sendSocket;

        /**
         * Endpoint for the multicast group
         */
        IPEndPoint groupEP; // DEBUG

        /**
         * SINGLETON CREATIONAL PATTERN
         * The protected constructor.
         */
        protected HelloNetworkModule() {

            // When the application starts, a random generated id is used.
            // The next part of the protocol will automatically set the correct id.
            Random r = new Random();
            int randId = r.Next();

            // DEBUG ################################################################################################## DEBUG
            // Retrieve the local peer or creates a new one.
            // The id of the peer is no more reliable. We must change it in any case.
            //if (Settings.Instance.LocalPeer == null)
            //    Settings.Instance.LocalPeer = new Peer("no_user" + ":" + randId, "no_user", "none");
            //else
            //    Settings.Instance.LocalPeer.Id = Settings.Instance.LocalPeer.Name + ":" + randId;
            // DEBUG ################################################################################################## DEBUG

            try {
                // Initialize the outgoing (multicast) socket
                mcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPAddress localIP = IPAddress.Any; // Retrieves local ip
                EndPoint localEP = new IPEndPoint(localIP, Settings.Instance.MCAST_HELLO_PORT);
                mcastSocket.Bind(localEP);  // Bind

                // Creates the multicast option
                MulticastOption mcastOption = new MulticastOption(Settings.Instance.MCAST_HELLO_IP_ADDRESS, localIP);
                // Registers to the multicast group
                mcastSocket.SetSocketOption(SocketOptionLevel.IP,
                                                SocketOptionName.AddMembership,
                                                mcastOption);
                
                // Initialize the endpoint for the multiacst group
                groupEP = new IPEndPoint(Settings.Instance.MCAST_HELLO_IP_ADDRESS,
                                         Settings.Instance.MCAST_HELLO_PORT);

                // Creates the outgoing socket
                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            } catch (Exception e) {
                Console.WriteLine("Socket connection error: " + e.Message);  // DEBUG
            }      
        }
        
        /**
         * Receives an hello packet from the network and gives it to
         * the function received as parameter.
         */
        public void receive() {
            
            // Reception buffer
            byte[] buf = new Byte[Settings.Instance.BUFSIZE];

            // Prepares an object to contain the sender address
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            // RECEIVE SOMETHING FROM THE NETWORK
            mcastSocket.ReceiveFrom(buf, ref remoteEP);

            // Transfoms the byte sequence in a string
            string jsonRecvPacket = Encoding.ASCII.GetString(buf, 0, buf.Length);

            // Deserialize the packet
            JObject o = JObject.Parse(jsonRecvPacket);
            HelloPacket packet;
            if (o["Type"].ToString() == HelloPacket.PacketType.Keepalive.ToString())            // KEEPALIVE
                packet = JsonConvert.DeserializeObject<KeepalivePacket>(jsonRecvPacket);
            else if (o["Type"].ToString() == HelloPacket.PacketType.Query.ToString())           // QUERY
                packet = JsonConvert.DeserializeObject<QueryPacket>(jsonRecvPacket);
            else if (o["Type"].ToString() == HelloPacket.PacketType.Presentation.ToString())    // PRESENTATION
                packet = JsonConvert.DeserializeObject<PresentationPacket>(jsonRecvPacket);
            else
                throw new Exception("Error parsing the received string into an HelloPacket: unknown type.");    // DEBUG

            // Call the delegate passing the received packet
            // HelloPacketReception?.Invoke(packet); // WHAT IS THIS? // DEBUG
            if (HelloPacketReception != null)
                HelloPacketReception(packet, ((IPEndPoint)remoteEP).Address.ToString());
        }

        /**
         * Allows to send a unicast hello packet to a specific IPaddress
         */
        public void sendUnicast(HelloPacket packet, string address) {

            // The buffer in which the packet will be put
            byte[] buf = new Byte[Settings.Instance.BUFSIZE];

            // Serialize the packet to Json and puts the result string into
            // the byte buffer
            string jPacket = JsonConvert.SerializeObject(packet);
             buf = Encoding.ASCII.GetBytes(jPacket);

            // Creates the receiver endpoint (the port will be the usual MCAST_HELLO_PORT)
            EndPoint remoteEP = new IPEndPoint(IPAddress.Parse(address), Settings.Instance.MCAST_HELLO_PORT);
            // Send the packet on the network
            sendSocket.SendTo(buf, remoteEP);

        }

        /**
         * Allows to send a unicast hello packet to the common multicast 
         * address used by the hello protocol
         */
        public void send(HelloPacket packet) {

            // The buffer in which the packet will be put
            byte[] buf = new Byte[Settings.Instance.BUFSIZE];

            // Serialize the packet to Json and puts the result string into
            // the byte buffer
            string jPacket = JsonConvert.SerializeObject(packet);
            buf = Encoding.ASCII.GetBytes(jPacket);

            // Creates the receiver endpoint (the port will be the usual MCAST_HELLO_PORT)
            EndPoint remoteEP = new IPEndPoint(Settings.Instance.MCAST_HELLO_IP_ADDRESS, Settings.Instance.MCAST_HELLO_PORT);
            // Send the packet on the network
            sendSocket.SendTo(buf, remoteEP);

        }

        /**
         * Close booth the outgoing and incoming sockets.
         * Who uses this class has the responsibility to call this method.
         */
        public void closeSockets() {
            mcastSocket.Close();
            sendSocket.Close();
        }

    }
    
}