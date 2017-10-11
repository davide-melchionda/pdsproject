#include "stdafx.h"
#include "KeepalivePacket.h"

KeepalivePacket::KeepalivePacket()
{
	type = HelloPacket::Keepalive;
}

KeepalivePacket::KeepalivePacket(std::string id, std::string name, std::string ipaddress)
{
	type = HelloPacket::Keepalive;
	this->id = id;
	this->name = name;
	this->ipaddress = ipaddress;
}


KeepalivePacket::~KeepalivePacket()
{
}