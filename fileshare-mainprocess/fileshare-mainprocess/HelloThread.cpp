#include "stdafx.h"
#include "HelloThread.h"

#include "KeepalivePacket.h"
#include <map>

HelloThread::HelloThread()
{
}


HelloThread::~HelloThread()
{
}

void onPacketReceived(shared_ptr<HelloPacket> packet) {
	PeersList& peers = PeersList::getInstance();
	KeepalivePacket* keepalive;
	PresentationPacket* presentation;
	QueryPacket* query;
	HelloNetworkModule network = HelloNetworkModule::getInstance();

	switch (packet->getType())
	{
	case HelloPacket::Type::Keepalive:
		cout << "Keepalive packet received." << endl;
		keepalive = dynamic_cast<KeepalivePacket*>(packet.get());
		if (!peers.exists(keepalive->name)) {
			cout << "Peer non noto!" << endl;
			// Send a Query packet to the unknown peer
			network.sendUnicast(QueryPacket("0.0.0.0"), keepalive->ipaddress);
		}
		else {
			cout << "Peer già noto!" << endl;
		}
		break;
	case HelloPacket::Type::Query:
		cout << "Query packet received." << endl;
		query = dynamic_cast<QueryPacket*>(packet.get());
		network.sendUnicast(PresentationPacket(Peer("MyName", "0.0.0.0")), query->ipaddress);
		break;
	case HelloPacket::Type::Presentation:
		cout << "Presentation packet received." << endl;
		presentation = dynamic_cast<PresentationPacket*>(packet.get());
		if (!peers.exists(presentation->getPeer().name)) {
			peers.insert(pair<std::string, Peer>(presentation->peer.name, presentation->peer));
			cout << "Peer non noto aggiunto: " << presentation->peer.name << endl;
			cout << "Tabella dei Peer" << endl;
			cout << "Chiave" << "\t|\t" << "Nome" << "\t|\t" << "Indirizzo" << endl;
			for (pair<string, Peer> pairObj : peers.getAll())
				cout << pairObj.first << "\t|\t" << pairObj.second.name << "\t|\t" << pairObj.second.ipaddress << endl;
		}
		else {
			cout << "Peer '" << presentation->getPeer().name << "' già noto" << endl;
		}
		break;
	default:
		break;
	}
}

void HelloThread::operator() (void) {
	// Obtaine a reference to the peers table
	PeersList& peers = PeersList::getInstance();
	// instantiate a network module
	HelloNetworkModule network = HelloNetworkModule::getInstance();

	// Run a thread dedicated to the sending of the
	// hello packets (one each 30 sec.)
	HelloSenderThread sender;
	thread senderThread(sender);
	//senderThread.detach();

	// Run a thread dedicated to the cleaning of the
	// peers table.
	HelloCleanupThread cleanup;
	thread cleanupThread(cleanup);

	network.onPacketReceived(onPacketReceived);
	while (1) {
		network.receive();
	}

	cleanupThread.join();
	senderThread.join();
}