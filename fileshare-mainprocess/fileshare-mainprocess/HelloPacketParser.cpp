#include "stdafx.h"

using namespace std;

#include "HelloPacketParser.h"

#include <vector>

HelloPacketParser::HelloPacketParser()
{
}


HelloPacketParser::~HelloPacketParser()
{
}

shared_ptr<HelloPacket> HelloPacketParser::unmarshal(std::string stream)
{
	//	DEBUG	DEBUG	DEBUG	DEBUG	DEBUG
	vector<string> names = { "Mario","Fabio","Lucia","Franco","Flavio" };
	vector<string> ips = { "127.0.0.1","127.0.0.1","127.0.0.1","127.0.0.1","127.0.0.1" };

	//HelloPacket *packet;

	//HelloPacket::Content c;

	Peer p(names[rand() % 5], ips[rand() % 5]);

	//Read packet from socket
	//##############################################
	switch (rand() % 3) {
	//switch(2) {
	case 0:
		/*c.keepalive.name = names[rand() % 5];
		c.keepalive.ipaddress = ips[rand() % 5];
		return HelloPacket(HelloPacket::Type::Keepalive, c);*/
		//return new KeepalivePacket(names[rand() % 5], ips[rand() % 5]);
		return shared_ptr<KeepalivePacket>(new KeepalivePacket(names[rand() % 5], ips[rand() % 5]));
	case 1:
		/*c.presentation.peer = Peer(names[rand() % 5], ips[rand() % 5]);
		return HelloPacket(HelloPacket::Type::Presentation, c);*/
		return shared_ptr<PresentationPacket>(new PresentationPacket(Peer(names[rand() % 5], ips[rand() % 5])));
	default:
		//return HelloPacket(HelloPacket::Type::Query, c);
		return shared_ptr<QueryPacket>(new QueryPacket(ips[rand()%5]));
	}
	//##############################################
}

std::string HelloPacketParser::marshal(HelloPacket packet)
{
	return "{type: keepalive, name: \"Mario\", ipaddress: 192.168.1.23}";
}

