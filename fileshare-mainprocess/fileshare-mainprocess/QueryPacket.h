#pragma once
#include "HelloPacket.h"

class QueryPacket : public HelloPacket
{
public:
	std::string ipaddress;

	QueryPacket(std::string ipaddress);
	~QueryPacket();
};

