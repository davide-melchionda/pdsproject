#include "stdafx.h"
#include "QueryPacket.h"


QueryPacket::QueryPacket()
{
	type = HelloPacket::Query;
}


QueryPacket::~QueryPacket()
{
}
