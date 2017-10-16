using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Script.Serialization;

namespace HelloProtocol {

    class Program {

        static void Main(string[] args) {
            HelloThread hello = new HelloThread();
            hello.run();

            ////Peer p = new Peer("identificativo", "nome del per", "192.168.1.23");
            //HelloPacket p1 = new KeepalivePacket("keepalive1", "nome peer", "192.168.1.24");
            //HelloPacket p2 = new KeepalivePacket("keepalive2", "nome peer", "192.168.1.24");
            //HelloPacket p3 = new PresentationPacket(new Peer("presentation1", "nome peer", "192.168.1.24"));
            //HelloPacket p4 = new QueryPacket();
            //HelloPacket p5 = new PresentationPacket(new Peer("presentation2", "nome peer", "192.168.1.24"));
            //HelloPacket p6 = new PresentationPacket(new Peer("presentation3", "nome peer", "192.168.1.24"));
            //HelloPacket p7 = new QueryPacket();
            //HelloPacket p8 = new KeepalivePacket("keepalive3", "nome peer", "192.168.1.24");
            //HelloPacket p9 = new QueryPacket();
            //HelloPacket p10 = new PresentationPacket(new Peer("presentation4", "nome peer", "192.168.1.24"));
            //HelloPacket p11 = new QueryPacket();
            //HelloPacket p12 = new KeepalivePacket("keepalive4", "nome peer", "192.168.1.24");

            //string[] ss = new String[12];
            //ss[0] = JsonConvert.SerializeObject(p1);
            //ss[1] = JsonConvert.SerializeObject(p2);
            //ss[2] = JsonConvert.SerializeObject(p3);
            //ss[3] = JsonConvert.SerializeObject(p4);
            //ss[4] = JsonConvert.SerializeObject(p5);
            //ss[5] = JsonConvert.SerializeObject(p6);
            //ss[6] = JsonConvert.SerializeObject(p7);
            //ss[7] = JsonConvert.SerializeObject(p8);
            //ss[8] = JsonConvert.SerializeObject(p9);
            //ss[9] = JsonConvert.SerializeObject(p10);
            //ss[10] = JsonConvert.SerializeObject(p11);
            //ss[11] = JsonConvert.SerializeObject(p12);

            //foreach (string s in ss) {
            //    JObject o = JObject.Parse(s);
            //    HelloPacket packet;
            //    if (o["Type"].ToString() == HelloPacket.PacketType.Keepalive.ToString())
            //        packet = JsonConvert.DeserializeObject<KeepalivePacket>(s);
            //    else if (o["Type"].ToString() == HelloPacket.PacketType.Query.ToString())
            //        packet = JsonConvert.DeserializeObject<QueryPacket>(s);
            //    else if (o["Type"].ToString() == HelloPacket.PacketType.Presentation.ToString())
            //        packet = JsonConvert.DeserializeObject<QueryPacket>(s);
            //    else
            //        throw new Exception("Error parsing the received string into an HelloPacket: unknown type.");    // DEBUG
            //}
        }
    }
}
