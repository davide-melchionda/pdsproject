#pragma once

#include <string>
#include "HelloPacket.h"
#include "KeepalivePacket.h"
#include "PresentationPacket.h"
#include "QueryPacket.h"
#include <vector>
#include <memory>

class HelloPacketParser
{
public:
	HelloPacketParser();
	~HelloPacketParser();

	shared_ptr<HelloPacket> unmarshal(std::string stream);
	std::string marshal(HelloPacket packet);
	
};

