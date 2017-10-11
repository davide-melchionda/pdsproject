/****************************************************************************************
* A parser class that allows to obtain a json from an HelloPacket and vice versa.
* It relies on the fact that the class T which is the object of the marshalling or
* unmarshalling operation provides the methods void fromJson(json) and json toJson(T t),
* and a no-argument constructor.
* Relies on the nlohmann::json parsing library.
****************************************************************************************/
using namespace std;

#pragma once

#include <string>
#include <vector>
#include <memory>
#include <iostream>
#include <vector>

#include "HelloPacket.h"
#include "KeepalivePacket.h"
#include "PresentationPacket.h"
#include "QueryPacket.h"

#include "json.hpp"

using nlohmann::json;

class HelloPacketParser
{
public:
	HelloPacketParser();
	~HelloPacketParser();

	/** Given a json object, provides a shared pointer to an instance 
		of the HelloPacket sub-class represented by that json object. */
	shared_ptr<HelloPacket> unmarshal(json json);

	/** Given an hello packet, provides its correct json representation. */
	json marshal(HelloPacket& packet);
	
};

