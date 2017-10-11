#include "stdafx.h"

#include "HelloPacketParser.h"

HelloPacketParser::HelloPacketParser()
{
}


HelloPacketParser::~HelloPacketParser()
{
}


shared_ptr<HelloPacket> HelloPacketParser::unmarshal(json json) {
	
	string type = json.at("type");

	// Checks the type of json object and returns an instance
	// of the correct sub-class of HelloPacket
	if (type == "keepalive") {
		KeepalivePacket* p = new KeepalivePacket();
		p->fromJson(json);
		return shared_ptr<KeepalivePacket>(p);
	} else if (type == "presentation") {
		PresentationPacket* p = new PresentationPacket();
		p->fromJson(json);
		return shared_ptr<PresentationPacket>(p);
	} else if (type == "query") {
		QueryPacket* p = new QueryPacket();
		p->fromJson(json);
		return shared_ptr<QueryPacket>(p);
	} else {
		// If the unmarshalling was not possible, 
		// returns a null pointer
		return nullptr;
	}
}

json HelloPacketParser::marshal(HelloPacket& packet)
{
	// Returns the json representation of the packet
	json json = packet.toJson();
	return json;
}

