#include "stdafx.h"
#include "QueryPacket.h"

#include "json.hpp"

using json = nlohmann::json;
using nlohmann::json;

QueryPacket::QueryPacket()
{
	type = HelloPacket::Query;
}

QueryPacket::QueryPacket(std::string ipaddress)
{
	type = HelloPacket::Query;
	this->ipaddress = ipaddress;
}


QueryPacket::~QueryPacket()
{
}

namespace ns {
	static void to_json(json& j, const QueryPacket& p) {
		j = json{ { "type", "query" }, { "ipaddress", p.ipaddress } };
	}

	static void from_json(const json& j, QueryPacket& p) {
		p.ipaddress = j.at("ipaddress").get<std::string>();
	}
} // namespace ns