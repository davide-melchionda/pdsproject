using namespace std;

#pragma once

#include <string>
#include "Peer.h"

class HelloPacket
{
public:
	enum Type {
		Keepalive, Query, Presentation
	};

	/*union Content {
		struct Keepalive {
			string name;
			string ipaddress;
		} keepalive;
		struct Query {
			// No contet
		} query;
		struct Presentation {
			Peer peer;
		} presentation;
		Content() {};
		Content(const Content& c);
		~Content(){}
	} content;

	HelloPacket(HelloPacket::Type type, HelloPacket::Content content);*/

	const HelloPacket::Type getType();

	virtual ~HelloPacket() {};
protected:
	HelloPacket::Type type;
};
