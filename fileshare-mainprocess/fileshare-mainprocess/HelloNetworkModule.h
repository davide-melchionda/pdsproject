/*********************************************************************************
*	The HelloNetworkModule offers a Singleton instance which allows to access to
*	the services needed to implement the HelloProtocol. The HelloNetworkModule
*	instance offers methods to receive() and send() packet (you can send unicast
*	or multicast packet).
**********************************************************************************/

#pragma once

#include<winsock2.h>
#include <Ws2tcpip.h>
#include <mswsock.h>

#pragma comment(lib, "Ws2_32.lib")
#pragma comment(lib, "iphlpapi.lib")

#ifndef u_int32
#define u_int32 UINT32  // Unix uses u_int32
#endif // !u_int32

#include<stdio.h>
#include <string>
#include <iostream>
#include <condition_variable>
#include <ctime>

#include "HelloPacket.h"
#include "HelloPacketParser.h"

/** The multicast IP address used by the application. */
#define MULTICAST_GROUP_ADDR "234.5.6.7"

/** Max length of buffer which will contain a Jason-ized 
	HellPacket.*/
#define BUFLEN 512

/** The port on which to listen for incoming data. */
#define MULTIDESTPORT 8888

class HelloNetworkModule
{
private: // Private instance fields
	
	Peer local;

	/** Incoming socket */
	SOCKET multiRecvSocket;
	
	/** Outgoing socket */
	SOCKET sendSocket;

	/** The callback called to manage a received packet. */
	void(*packetReceivedCallback)(shared_ptr<HelloPacket>, string);

		/** The address of the socket exposed by this applciation. */
	struct sockaddr_in	server;
	
	/** Winsock structure. Contains information about the
		winsock implementation. */
	WSADATA wsa;
	
	/** An instance of the packet parser which allows to trasnform
		Json strings in HelloPacket instances and vice versa. */
	HelloPacketParser parser;

	/** Mutex for the management of the access to the shared send socket. */
	mutex sendSocketMutex;

protected:
	/** The constructor is protected: the class implements a Singleton. */
	HelloNetworkModule();

public:
	/** Allows the caller to obtain a reference to the unique instance
		of the class. */
	static HelloNetworkModule& getInstance()
	{
		static HelloNetworkModule instance;
		return instance;
	};

	/** Sends a unicast packet to the destination ip address. The port
		on which the packet is sent is that one used by the application. */
	void sendUnicast(HelloPacket& packet, string dest);
	
	/** Sends a multicast packet to all the servers listening on the 
		common address and port. */
	void send(HelloPacket& packet);
	
	/** Requires to listen on the multiRecvSocket in order to receive
		packets from the network. */
	void receive();

	/** Registers a callback which will be invocked when a new packet
		is received from the network. */
	void onPacketReceived(void (*f)(shared_ptr<HelloPacket> packet, string senderip));
	
	/** Returns the peer coresponding to the current user. */
	const Peer getPeer();

	/** Updates the ip address of the peer and (consequently) his id. */
	void updatePeerAddress(std::string ipaddress);

	// Destructor
	~HelloNetworkModule();
};