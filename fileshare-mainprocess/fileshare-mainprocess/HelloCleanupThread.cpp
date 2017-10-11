#include "stdafx.h"
#include "HelloCleanupThread.h"

#define DEBUG_OUTPUT

HelloCleanupThread::HelloCleanupThread()
{
}


HelloCleanupThread::~HelloCleanupThread()
{
}

void HelloCleanupThread::operator()() {
	while (1) {
		// Sleep for a certain time interval
		Sleep(10000 * 2);
		
		// The instant he woke up
		time_t wakeup = time(0);
		
		// Retrieves the peers list
		PeersList& peers = PeersList::getInstance();	
		
		// Checks if anyone of the peers entry has to be removed
		for (std::pair<std::string, PeerEntry> entryPair : peers.getAll()) {
			// Calculates the numbero of SECONDS the peer entry has not been 
			// updated in the peers list.
			int notupdated = wakeup - entryPair.second.timestamp;
			// If this time is over a cetain threshold
			if (notupdated > 20 /* seconds */) {
#ifdef DEBUG_OUTPUT
				std::cout << "Cancello il peer '" << entryPair.first << "'" << endl;
#endif
				// Removes the entry from the peers list
				peers.del(entryPair.first);
			}
		}

#ifdef DEBUG_OUTPUT
		cout << "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" << endl;
		cout << "Tabella dei Peer" << endl;
		cout << "Chiave" << "\t|\t" << "Nome" << "\t|\t" << "Indirizzo" << endl;
		for (pair<string, PeerEntry> pairObj : peers.getAll())
			cout << pairObj.first << "\t|\t" << pairObj.second.peer.name << "\t|\t" << pairObj.second.peer.ipaddress << endl;
		cout << "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" << endl;
#endif

	}
}