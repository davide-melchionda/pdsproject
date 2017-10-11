#include "stdafx.h"
#include "PeersList.h"

void PeersList::updateTimestamp(std::string id)
{
	unique_lock<mutex> ul(m);
	myMap[id].timestamp = time(0);
}
