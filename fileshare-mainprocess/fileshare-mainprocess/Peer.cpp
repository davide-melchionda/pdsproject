#include "stdafx.h"
#include "Peer.h"


Peer::Peer(std::string name, std::string ipaddress)
{
	this->name = name;
	this->ipaddress = ipaddress;
}


Peer::~Peer()
{
}
