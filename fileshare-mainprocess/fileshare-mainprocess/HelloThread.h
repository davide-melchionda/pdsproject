/*********************************************************************************
* Represents the operations to be performed to implement the logic of a thread
* that starts the different components of the Hello Protocol. The operation to
* be performed are: 1) start a thread that sends a KeepalivePacket each X seconds;
* 2) start a thread that cleans the PeerList removing the peers which are no more
* active; 3) wait for hello packet from other peers and manage them.
* In order to perform these operations, this class heavly relies on the 
* HelloNetworkModule class.
***********************************************************************************/

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
#include <map>

class HelloThread
{
public:
	HelloThread();
	~HelloThread();

	// Execution
	void operator() ();

	/** Static function that describes the way to manage a generic HelloPacket. It implements
		the part of the hello protocol which is responsible to manage a received packet and 
		performs the consequential actions. */
	static void onPacketReceived(shared_ptr<HelloPacket> packet, std::string senderip);
};

