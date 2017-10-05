#pragma once
#include "HelloPacket.h"

class KeepalivePacket : public HelloPacket
{
public:
	std::string name;
	std::string ipaddress;
	
	KeepalivePacket(std::string name,std::string ipaddress);
	~KeepalivePacket();
};

