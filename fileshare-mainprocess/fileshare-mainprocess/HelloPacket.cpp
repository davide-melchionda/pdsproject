#include "stdafx.h"
#include "HelloPacket.h"

/*HelloPacket::HelloPacket(HelloPacket::Type type, HelloPacket::Content content)
{
	this->type = type;
	if (type == HelloPacket::Keepalive) {
		this->content.keepalive.name = content.keepalive.name;
		this->content.keepalive.ipaddress = content.keepalive.ipaddress;
	}
	else if (type == HelloPacket::Type::Presentation) {
		this->content.presentation.peer = Peer(content.presentation.peer);
	}
}*/

/*HelloPacket::Content::Content(const Content & c)
{
	memcpy(this, &c, sizeof(union HelloPacket::Content))w;
	/*try {
		this->keepalive.name = c.keepalive.name;
	}
	catch (exception) {

	}
	try {
		this->keepalive.ipaddress = c.keepalive.ipaddress;
	}
	catch (exception) {

	}
	try {
		this->presentation.peer= Peer(c.presentation.peer);
	}
	catch (exception) {

	}*/
//}

const HelloPacket::Type HelloPacket::getType()
{
	return type;
}
