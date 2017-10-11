#pragma once

#include "HelloNetworkModule.h"

class HelloSenderThread
{
public:
	HelloSenderThread();
	~HelloSenderThread();

	void operator() ();
};

