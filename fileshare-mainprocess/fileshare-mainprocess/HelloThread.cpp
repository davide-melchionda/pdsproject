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

void HelloThread::operator() (void) {
	WrapperClass<Peer>& peers = WrapperClass<Peer>::getInstance();

	HelloSenderThread sender;
	thread senderThread(sender);
	//senderThread.detach();

	HelloCleanupThread cleanup;
	thread cleanupThread(cleanup);
	//cleanupThread.detach();

	//##########################################
	//##########################################
	//	DEBUG	DEBUG	DEBUG	DEBUG	DEBUG
	vector<string> names = { "Mario","Fabio","Lucia","Franco","Flavio" };
	vector<string> ips = { "192.168.1.2","192.168.1.3","192.168.1.4","192.168.1.5","192.168.1.6" };
	//	DEBUG	DEBUG	DEBUG	DEBUG	DEBUG
	//##########################################
	//##########################################

	while (true) {

		HelloPacket* packet;
		//Read packet from socket
		//##############################################
		switch (rand() % 3) {
		case 0:
			packet = new KeepalivePacket(names[rand()%5],ips[rand()%5]);
			break;
		case 1:
			packet = new QueryPacket();
			break;
		default:
			packet = new PresentationPacket(*(new Peer(names[rand() % 5], ips[rand() % 5])));
		}
		//##############################################
		KeepalivePacket* keepalive;
		PresentationPacket* presentation;
		//Peer p("","");
		switch (packet->getType())
		{
		case HelloPacket::Type::Keepalive:
			cout << "Keepalive packet received." << endl;
			keepalive = dynamic_cast<KeepalivePacket*>(packet);
			if (peers.exists(keepalive->name)) {
				cout << "Peer già noto!" << endl;
				break;
			}
			// Send a Query packet to the unknown peer
			//###########################################
			// sendQueryPacket();
			//###########################################
			break;
		case HelloPacket::Type::Query:
			cout << "Query packet received." << endl;
			//###########################################
			// sendPresentationPacket();
			//###########################################
			break;
		case HelloPacket::Type::Presentation:
			cout << "Presentation packet received." << endl;
			presentation = dynamic_cast<PresentationPacket*>(packet);
			//###########################################
			// savePresentationPacket();
			//###########################################
			peers.insert(pair<std::string,Peer>(presentation->getPeer().name, presentation->getPeer()));
			cout << "Peer non noto aggiunto: " << presentation->getPeer().name << endl;
			cout << "Tabella dei Peer" << endl;
			cout << "Chiave" << "\t|\t" << "Nome" << "\t|\t" << "Indirizzo" << endl;
			for (pair<string, Peer> pairObj : peers.getAll())
				cout << pairObj.first << "\t|\t" << pairObj.second.name << "\t|\t" << pairObj.second.ipaddress << endl;
			break;
		default:
			break;
		}
		cout << "========================================" << endl;
		Sleep(5000);

	}
	cleanupThread.join();
	senderThread.join();
}