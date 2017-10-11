#include "stdafx.h"
#include "PeerEntry.h"


PeerEntry::PeerEntry()
{
}

PeerEntry::PeerEntry(time_t t, Peer p)
{
	this->timestamp = t;
	this->peer = p;
}


PeerEntry::~PeerEntry()
{
}
