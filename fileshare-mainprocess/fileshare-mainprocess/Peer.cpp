#include "stdafx.h"
#include "Peer.h"

Peer::Peer()
{
}

Peer::Peer(std::string id, std::string name, std::string ipaddress)
{
	this->id = id;
	this->name = name;
	this->ipaddress = ipaddress;
}

Peer::Peer(const Peer & peer)
{
	this->id = peer.id;
	this->name = peer.name;
	this->ipaddress = peer.ipaddress;
}


Peer::~Peer()
{
}

//
//namespace ns {
//	void to_json(json& j, const Peer& p) {
//		j = json{ { "id", p.id },{ "name", p.name },{ "ipaddress", p.ipaddress } };
//	}
//
//	void from_json(const json& j, Peer& p) {
//		p.id = j.at("id").get<std::string>();
//		p.name = j.at("name").get<std::string>();
//		p.ipaddress = j.at("ipaddress").get<std::string>();
//	}
//} // namespace ns
