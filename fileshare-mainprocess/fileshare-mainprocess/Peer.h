#pragma once

#include <string>

#include "json.hpp"
using nlohmann::json;

class Peer
{
public:
	std::string name;
	std::string ipaddress;
	std::string id;

	Peer();
	Peer(std::string id, std::string name, std::string ipaddress);
	Peer(const Peer& peer);
	~Peer();

	virtual json toJson() {
		return json{ { "id", id },{ "name", name },{ "ipaddress", ipaddress } };
	}

	virtual void fromJson(json json) {
		id = json.at("id").get<std::string>();
		name = json.at("name").get<std::string>();
		ipaddress = json.at("ipaddress").get<std::string>();
	}
};

