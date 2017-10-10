#include "stdafx.h"
#include "QueryPacket.h"


QueryPacket::QueryPacket(std::string ipaddress)
{
	type = HelloPacket::Query;
	this->ipaddress = ipaddress;
}


QueryPacket::~QueryPacket()
{
}
