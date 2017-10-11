#pragma once
#include "HelloPacket.h"

class QueryPacket : public HelloPacket
{
public:
	std::string ipaddress;

	QueryPacket();
	QueryPacket(std::string ipaddress);
	~QueryPacket();

	virtual json toJson() {
		return json{ { "type", "query" }, { "ipaddress", ipaddress } };
	}

	virtual void fromJson(json json) {
		ipaddress = json.at("ipaddress").get<std::string>();
	}
};

