#pragma once

#include <string>

class Peer
{
public:
	std::string name;
	std::string ipaddress;

	Peer();
	Peer(std::string name, std::string ipaddress);
	Peer(const Peer& peer);
	~Peer();
};

