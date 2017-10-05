#pragma once

#include <string>

class HelloPacket
{
public:
	enum Type {
		Keepalive, Query, Presentation
	};

	/*HelloPacket(Type t) :type(t) {};
	HelloPacket(Type t, std::string b) :type(t), body(b) {};*/

	virtual Type getType();

	~HelloPacket() {};

protected:
	Type type;
};
