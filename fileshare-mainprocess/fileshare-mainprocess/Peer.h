#pragma once

#include <string>

class Peer
{
public:
	std::string name;
	std::string ipaddress;

	Peer(std::string name, std::string ipaddress);
	~Peer();
};

