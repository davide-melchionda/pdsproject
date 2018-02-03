using FileTransfer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;


namespace NetworkTransmission
{
    /**
     * TransferNetworkModule offers a whole range of functions usefull to send and receive our specific packets on the network 
     */

    class TransferNetworkModule
    {

        /**
         * Allows to receive a generic byte stream and convert it in a specific packet object
         * Returns NULL if the received stream doesn't correspond with a known Transmission Packet
         */

        public TransmissionPacket receivePacket(Socket handler)
        {

            string data = "";
            TransmissionPacket received = null;
            int bytesRec;
            JObject o = null;
            while (true)
            {
                byte[] bytes = new byte[1024];
                bytesRec = handler.Receive(bytes);
                data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                //Console.WriteLine(data);
                if (data.IndexOf("<EOF>") > -1)
                {
                    break;
                }
            }

            data = data.Substring(0, data.Length - 5);      //remove the end_of_file

            o = (JObject)JsonConvert.DeserializeObject(data);

            if (o["Type"].ToString() == TransmissionPacket.PacketType.request.ToString())                 // REQUEST
                received = JsonConvert.DeserializeObject<RequestPacket>(data);
            else if (o["Type"].ToString() == TransmissionPacket.PacketType.response.ToString())           // RESPONSE
                received = JsonConvert.DeserializeObject<ResponsePacket>(data);

            return received;
        }

        /**
         * Implements a send in our application logic
         */

        public int SendPacket(Socket client, byte[] message)
        {
            int bytesSent = 0;
            while (bytesSent != message.Length)
            {
                bytesSent = client.Send(message);
            }
            return bytesSent;
        }

        /**
         * Returns a byte stream representation of a Request starting from the task 
         */

        public byte[] generateRequestStream(FileTransfer.Task task)
        {
            RequestPacket request = new RequestPacket(task);
            byte[] requestStream = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request) + "<EOF>");
            return requestStream;

        }


        /**
         * Returns a byte stream representation of a Response 
         */
        public byte[] generateResponsetStream(bool procede)    
        {
            ResponsePacket response = new ResponsePacket(procede);
            byte[] responseStream = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response) + "<EOF>");
            return responseStream;

        }

    }
}
