#include "stdafx.h"
#include "Peer.h"


Peer::Peer()
{
}

Peer::Peer(std::string name, std::string ipaddress)
{
	this->name = name;
	this->ipaddress = ipaddress;
}

Peer::Peer(const Peer & peer)
{
	this->name = peer.name;
	this->ipaddress = peer.ipaddress;
}


Peer::~Peer()
{
}
