#pragma once
#include "HelloPacket.h"

class KeepalivePacket : public HelloPacket
{
public:
	std::string name;
	std::string ipaddress;
	std::string id;

	KeepalivePacket();
	KeepalivePacket(std::string id, std::string name,std::string ipaddress);
	~KeepalivePacket();

	virtual json toJson() {
		return json{ { "type", "keepalive" }, { "id", id },{ "name", name },{ "ipaddress", ipaddress } };
	}

	virtual void fromJson(json json) {
		id = json.at("id").get<std::string>();
		name = json.at("name").get<std::string>();
		ipaddress = json.at("ipaddress").get<std::string>();
	}
};

