#pragma once
#include "HelloPacket.h"
#include "Peer.h"

class PresentationPacket : public HelloPacket
{
public:
	Peer peer;

	PresentationPacket();
	PresentationPacket(Peer peer);
	~PresentationPacket();

	Peer getPeer();

	virtual json toJson() {
		return json{ { "type", "presentation" },{ "peer", peer.toJson()} };
	}

	virtual void fromJson(json json) {
		peer.fromJson(json.at("peer"));
	}
};

