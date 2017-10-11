#include "stdafx.h"

#include "HelloThread.h"

#define DEBUG_OUTPUT

HelloThread::HelloThread()
{
}


HelloThread::~HelloThread()
{
}

void HelloThread::onPacketReceived(shared_ptr<HelloPacket> packet, std::string senderip) {
	// Retrieves the instance of the peers list
	PeersList& peers = PeersList::getInstance();
	
	// Only one of these pointers will be filled
	KeepalivePacket* keepalive;
	PresentationPacket* presentation;
	QueryPacket* query;
	
	// a generic peer
	Peer p;

	// Switch on the type of packet received
	switch (packet->getType())
	{
	case HelloPacket::Type::Keepalive:	// KEEPALIVE packet
#ifdef DEBUG_OUTPUT
		cout << "Keepalive packet received from " << senderip << endl;
#endif
		// The packet is a keepalive. Casts the pointer to an apropr	iate type
		keepalive = dynamic_cast<KeepalivePacket*>(packet.get());
		p = HelloNetworkModule::getInstance().getPeer();
		if (p.id == keepalive->id) {
			if (p.ipaddress != senderip)
				HelloNetworkModule::getInstance().updatePeerAddress(senderip);
		}
		else if (!peers.exists(keepalive->id)) { // Checks if the peer is already known
#ifdef DEBUG_OUTPUT
			cout << "Peer non noto: '" << keepalive->name << "'!" << endl;
			cout << "Invio un messaggio di Query." << endl;
#endif
			// If it's not, send a Query packet to the unknown peer
			HelloNetworkModule::getInstance()	// retrieves the instance of the network service
				.sendUnicast(QueryPacket("none"), senderip);
		}
		else {
#ifdef DEBUG_OUTPUT
			cout << "Peer già noto: '" << keepalive->name << "'! Ne aggiorno il timestamp" << endl;
#endif
			peers.updateTimestamp(keepalive->id);
		}
		break;
	
	case HelloPacket::Type::Query:	// QUERY packet
#ifdef DEBUG_OUTPUT
		cout << "Query packet received from " << senderip << endl;
#endif
		// The packet is a query. Casts the pointer to an apropriate type
		query = dynamic_cast<QueryPacket*>(packet.get());
		
#ifdef DEBUG_OUTPUT
		cout << "Invio un messaggio di presentation." << endl;
#endif

		// Send a Presentation packet to the requesting peer
		HelloNetworkModule::getInstance()	// retrieves the instance of the network service
							.sendUnicast(PresentationPacket(HelloNetworkModule::getInstance().getPeer()), senderip);
		break;
	
	case HelloPacket::Type::Presentation:	// PRESENTATION packet
#ifdef DEBUG_OUTPUT
		cout << "Presentation packet received from " << senderip << endl;
#endif
		// The packet is a presentation. Casts the pointer to an apropriate type
		presentation = dynamic_cast<PresentationPacket*>(packet.get());
		// If the peer is really unknown
		if (!peers.exists(presentation->getPeer().name)) {
			// Inserts the new peer in the list
			peers.insert(pair<std::string, PeerEntry>(presentation->peer.id, PeerEntry(time(0), presentation->peer)));

#ifdef DEBUG_OUTPUT
			cout << "Peer non noto aggiunto: " << presentation->peer.name << endl;
			cout << "Tabella dei Peer" << endl;
			cout << "Chiave" << "\t|\t" << "Nome" << "\t|\t" << "Indirizzo" << endl;
			for (pair<string, PeerEntry> pairObj : peers.getAll())
				cout << pairObj.first << "\t|\t" << pairObj.second.peer.name << "\t|\t" << pairObj.second.peer.ipaddress << endl;
#endif
		}
#ifdef DEBUG_OUTPUT
		else {
			cout << "Peer '" << presentation->getPeer().name << "' già noto" << endl;
		}
#endif
		break;
	default:
		break;
	}
#ifdef DEBUG_OUTPUT
	cout << "===============================================" << endl;
#endif DEBUG_OUTPUT
}

void HelloThread::operator() (void) {
	// Obtaine a reference to the peers table
	PeersList& peers = PeersList::getInstance();
	// instantiate a network module
	HelloNetworkModule& network = HelloNetworkModule::getInstance();

	// Run a thread dedicated to the sending of the
	// hello packets (one each 30 sec.)
	HelloSenderThread sender;
	thread senderThread(sender);
	//senderThread.detach();

	// Run a thread dedicated to the cleaning of the
	// peers table.
	HelloCleanupThread cleanup;
	thread cleanupThread(cleanup);

	// Registers the callback to call on the reception of a packet
	network.onPacketReceived(onPacketReceived);

	// Loops receiving packets
	while (1) {
		network.receive();
	}

	// Wait for the termination of the other threads
	cleanupThread.join();
	senderThread.join();
}