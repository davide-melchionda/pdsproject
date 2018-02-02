using Microsoft.WindowsAPICodePack.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace FileShare {
    public class NetworkInfo {

        private Network net;
        public Network Network {
            get {
                return net;
            }
        }
        private NetworkInterface nic;
        public NetworkInterface Nic {
            get {
                return nic;
            }
        }

        public NetworkInfo(Network net, NetworkInterface nic) {
            this.net = net;
            this.nic = nic;
        }

    }
}
