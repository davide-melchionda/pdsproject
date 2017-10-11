/**
* Represents the operations to be performed to implement a periodi check on 
* the peers in an instance of PeersList in order to remove those that are 
* considered no more active on the network.
*/

#pragma once

#include "PeersList.h"

#include <Windows.h>

/** The number of second of inactivity after which the peer entry
	can be considered no more valid. */
#define THRESHOLD_SECONDS 20

/** Number of milliseconds to sleep for */
#define SLEEP_TIME 10000 * 2	// DEBUG 20 sec
//#define SLEEP_TIME 60000 /*milliseconds*/ * 2 /*minutes*/

class HelloCleanupThread
{
public:
	HelloCleanupThread();
	~HelloCleanupThread();

	// Execution
	void operator() ();
};

