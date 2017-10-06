#pragma once

#include "wrapperclass.cpp"
#include "Peer.h"

class PeersList : public WrapperClass<Peer>
{
private:
	PeersList() {};
public:
	static PeersList& getInstance() //"Meyers" Singleton
	{
		static PeersList instance;
		return instance;
	};
};

