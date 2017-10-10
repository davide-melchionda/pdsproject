#pragma once

#include "HelloNetworkModule.h"
#include "PeersList.h"
#include "HelloPacket.h"
#include "HelloSenderThread.h"
#include "HelloCleanupThread.h"
#include "Peer.h"
#include "KeepalivePacket.h"
#include "QueryPacket.h"
#include "PresentationPacket.h"

class HelloThread
{
public:
	HelloThread();
	~HelloThread();

	void operator() ();
};

