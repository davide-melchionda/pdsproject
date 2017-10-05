#pragma once
#include "HelloPacket.h"
#include "Peer.h"

class PresentationPacket : public HelloPacket
{
public:
	Peer* peer;
	
	PresentationPacket(Peer peer);
	~PresentationPacket();

	Peer getPeer();
};

