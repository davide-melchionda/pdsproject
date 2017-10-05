#include "stdafx.h"
#include "PresentationPacket.h"

PresentationPacket::PresentationPacket(Peer peer)
{
	type = HelloPacket::Presentation;
	this->peer = new Peer(peer);
}


PresentationPacket::~PresentationPacket()
{
}

Peer PresentationPacket::getPeer() {
	return *peer;
}
