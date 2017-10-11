#pragma once

#include "wrapperclass.cpp"
#include "PeerEntry.h"
#include "Peer.h"

class PeersList : public WrapperClass<PeerEntry>
{
private:
	PeersList() {};
public:
	static PeersList& getInstance() //"Meyers" Singleton
	{
		static PeersList instance;
		return instance;
	};
	
	void updateTimestamp(std::string id);
};

