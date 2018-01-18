using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
         * The object on which acquire the lock in order to create the instance
         */
        private static object syncRoot = new Object();
        /**
         * SINGLETON CREATIONAL PATTERN
         * The property which represents the unique instance of the class.
         */
        public static HelloNetworkModule Instance {
            get {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null)
                            instance = new HelloNetworkModule();
                    }
                }
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
        private object e1;

        /**
         * SINGLETON CREATIONAL PATTERN
         * The protected constructor.
         */
        protected HelloNetworkModule() {

            // When the application starts, a random generated id is used.
            // The next part of the protocol will automatically set the correct id.
            Random r = new Random();
            int randId = r.Next();

            try {
                // Initialize the outgoing (multicast) socket
                mcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPAddress localIP = IPAddress.Any;
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
        public bool receive() {
            // Reception buffer
            byte[] buf = new Byte[Settings.Instance.BUFSIZE];

            // Prepares an object to contain the sender address
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            try {
                // RECEIVE SOMETHING FROM THE NETWORK
                mcastSocket.ReceiveFrom(buf, ref remoteEP);
            } catch (SocketException e) {
                return false;
            } catch (ObjectDisposedException e1) {
                return false;
            }

            // Transfoms the byte sequence in a string
            string jsonRecvPacket = Encoding.UTF8.GetString(buf, 0, buf.Length);

            // Deserialize the packet
            JObject o = JObject.Parse(jsonRecvPacket);
            HelloPacket packet;
            if (o["Type"].ToString() == HelloPacket.PacketType.Keepalive.ToString())            // KEEPALIVE
                packet = JsonConvert.DeserializeObject<KeepalivePacket>(jsonRecvPacket);
            else if (o["Type"].ToString() == HelloPacket.PacketType.Query.ToString())           // QUERY
                packet = JsonConvert.DeserializeObject<QueryPacket>(jsonRecvPacket);
            else if (o["Type"].ToString() == HelloPacket.PacketType.Presentation.ToString())    // PRESENTATION
                packet = JsonConvert.DeserializeObject<PresentationPacket>(jsonRecvPacket);
            else if (o["Type"].ToString() == HelloPacket.PacketType.GoodBye.ToString())    // PRESENTATION
                packet = JsonConvert.DeserializeObject<GoodByePacket>(jsonRecvPacket);
            else
                throw new Exception("Error parsing the received string into an HelloPacket: unknown type.");    // DEBUG

            // Call the delegate passing the received packet
            // Processing on a dedicated thread so to not be blocking
            Task.Run(() => {
                HelloPacketReception?.Invoke(packet, ((IPEndPoint)remoteEP).Address.ToString());
            });

            return true;
        }

        /**
         * Allows to send a unicast hello packet to a specific IPaddress
         */
        public bool sendUnicast(HelloPacket packet, string address) {

            // The buffer in which the packet will be put
            byte[] buf = new Byte[Settings.Instance.BUFSIZE];

            // Serialize the packet to Json and puts the result string into
            // the byte buffer
            string jPacket = JsonConvert.SerializeObject(packet);
            buf = Encoding.UTF8.GetBytes(jPacket);

            // Creates the receiver endpoint (the port will be the usual MCAST_HELLO_PORT)
            EndPoint remoteEP = new IPEndPoint(IPAddress.Parse(address), Settings.Instance.MCAST_HELLO_PORT);

            try {
                // Send the packet on the network
                sendSocket.SendTo(buf, remoteEP);
            } catch (SocketException e) {
                return false;
            } catch (ObjectDisposedException e1) {
                return false;
            }

            return true;
        }


        /**
         * Allows to send a single hello packet to the common multicast 
         * address used by the hello protocol
         */
        public bool send(HelloPacket packet) {

            // The buffer in which the packet will be put
            byte[] buf = new Byte[Settings.Instance.BUFSIZE];

            // Serialize the packet to Json and puts the result string into
            // the byte buffer
            string jPacket = JsonConvert.SerializeObject(packet);
            buf = Encoding.UTF8.GetBytes(jPacket);

            // Creates the receiver endpoint (the port will be the usual MCAST_HELLO_PORT)
            EndPoint remoteEP = new IPEndPoint(Settings.Instance.MCAST_HELLO_IP_ADDRESS, Settings.Instance.MCAST_HELLO_PORT);

            try {
                // Send the packet on the network
                sendSocket.SendTo(buf, remoteEP);
            } catch (SocketException e1) {
                return false;
            } catch (ObjectDisposedException e) {
                return false;
            }

            return true;

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