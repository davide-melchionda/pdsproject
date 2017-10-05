#include "stdafx.h"
#include "KeepalivePacket.h"


KeepalivePacket::KeepalivePacket(std::string name, std::string ipaddress)
{
	type = HelloPacket::Keepalive;
	this->name = name;
	this->ipaddress = ipaddress;
}


KeepalivePacket::~KeepalivePacket()
{
}
