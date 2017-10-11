#include "stdafx.h"
#include "PresentationPacket.h"

//#include "json.hpp"
//
//using json = nlohmann::json;
//using nlohmann::json;

PresentationPacket::PresentationPacket()
{
	type = HelloPacket::Presentation;
}

PresentationPacket::PresentationPacket(Peer peer)
{
	type = HelloPacket::Presentation;
	this->peer = Peer(peer);
}


PresentationPacket::~PresentationPacket()
{
}

Peer PresentationPacket::getPeer() {
	return peer;
}

//
//namespace ns {
//	static void to_json(json& j, const PresentationPacket& p) {
//		j = json{ { "type", "presentation" },{ "peer", json(p.peer) } };
//	}
//
//	static void from_json(const json& j, PresentationPacket& p) {
//		p.peer = j.at("peer").get<Peer>();
//	}
//} // namespace ns
