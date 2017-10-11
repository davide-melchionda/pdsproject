#pragma once

#include "Peer.h"

class PeerEntry
{
public:

	time_t timestamp;
	Peer peer;

	PeerEntry();
	PeerEntry(time_t t, Peer p);
	~PeerEntry();
};

