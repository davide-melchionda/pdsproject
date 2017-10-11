using namespace std;

#pragma once

#include <string>
#include "Peer.h"

#include "json.hpp"

using nlohmann::json;

class HelloPacket
{
public:
	enum Type {
		Keepalive, Query, Presentation
	};

	HelloPacket::Type getType() const;

	virtual json toJson() = 0;
	virtual void fromJson(json json) = 0;

	virtual ~HelloPacket() {};
protected:
	HelloPacket::Type type;
};
