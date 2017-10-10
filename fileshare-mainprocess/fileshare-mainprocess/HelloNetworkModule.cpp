#include "stdafx.h"

/*#include<winsock2.h>
#include <Ws2tcpip.h>
#include <mswsock.h>*/

#include "HelloNetworkModule.h"

HelloNetworkModule::HelloNetworkModule()
{
	slen = sizeof(si_other);

	//Initialise winsock
	printf("\nInitialising Winsock...");
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
	{
		printf("Failed. Error Code : %d", WSAGetLastError());
		exit(EXIT_FAILURE);
	}
	printf("Initialised.\n");

	//Create a socket
	if ((s = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) == INVALID_SOCKET)
	{
		printf("Could not create socket : %d", WSAGetLastError());
	}
	printf("Socket created.\n");

	//Prepare the sockaddr_in structure
	server.sin_family = AF_INET;
	server.sin_addr.s_addr = INADDR_ANY;
	server.sin_port = htons(PORT);

	//join_source_group(s, str_to_uint32("244.12.12.12"), INADDR_ANY);
	struct ip_mreq mreq;
	mreq.imr_interface.s_addr = INADDR_ANY;
	mreq.imr_multiaddr.s_addr = inet_addr(MULTICAST_GROUP_ADDR);
	setsockopt(s, IPPROTO_IP, IP_ADD_MEMBERSHIP, (char *)&mreq, sizeof(mreq));

	//Bind
	if (::bind(s, (struct sockaddr *)&server, sizeof(server)) == SOCKET_ERROR)
	{
		printf("Bind failed with error code : %d", WSAGetLastError());
		exit(EXIT_FAILURE);
	}
	puts("Bind done");
}


HelloNetworkModule::~HelloNetworkModule()
{
}

void HelloNetworkModule::sendUnicast(HelloPacket packet, string dest)
{
	struct sockaddr_in si_other;
	int s, slen = sizeof(si_other);
	WSADATA wsa;

	//create socket
	if ((s = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) == SOCKET_ERROR)
	{
		printf("socket() failed with error code : %d", WSAGetLastError());
		exit(EXIT_FAILURE);
	}

	//setup address structure
	memset((char *)&si_other, 0, sizeof(si_other));
	si_other.sin_family = AF_INET;
	si_other.sin_port = htons(PORT);
	si_other.sin_addr.S_un.S_addr = inet_addr(dest.c_str());

	string jsonPacket = parser.marshal(packet);

	//send the message
	if (sendto(s, jsonPacket.c_str(), jsonPacket.length(), 0, (struct sockaddr *) &si_other, slen) == SOCKET_ERROR)
	{
		printf("sendto() failed with error code : %d", WSAGetLastError());
		exit(EXIT_FAILURE);
	}
}

void HelloNetworkModule::send(HelloPacket packet)
{
	struct sockaddr_in si_other;
	int s, slen = sizeof(si_other);
	WSADATA wsa;

	//create socket
	if ((s = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) == SOCKET_ERROR)
	{
		printf("socket() failed with error code : %d", WSAGetLastError());
		exit(EXIT_FAILURE);
	}

	//setup address structure
	memset((char *)&si_other, 0, sizeof(si_other));
	si_other.sin_family = AF_INET;
	si_other.sin_port = htons(PORT);
	si_other.sin_addr.S_un.S_addr = inet_addr(MULTICAST_GROUP_ADDR);

	string jsonPacket = parser.marshal(packet);
	
	//send the message
	if (sendto(s, jsonPacket.c_str(), jsonPacket.length(), 0, (struct sockaddr *) &si_other, slen) == SOCKET_ERROR)
	{
		printf("sendto() failed with error code : %d", WSAGetLastError());
		exit(EXIT_FAILURE);
	}
}

void HelloNetworkModule::receive()
{
	char buf[BUFLEN];

	fflush(stdout);

	//clear the buffer by filling null, it might have previously received data
	memset(buf, '\0', BUFLEN);

	//try to receive some data, this is a blocking call
	if ((recv_len = recvfrom(s, buf, BUFLEN, 0, (struct sockaddr *) &si_other, &slen)) == SOCKET_ERROR)
	{
		printf("recvfrom() failed with error code : %d", WSAGetLastError());
		exit(EXIT_FAILURE);
	}

	//print details of the client/peer and the data received
	printf("Received packet from %s:%d\n", inet_ntoa(si_other.sin_addr), ntohs(si_other.sin_port));
	printf("Data: %s\n", buf);
	
	shared_ptr<HelloPacket> packet = parser.unmarshal(buf);

	packetReceivedCallback(packet);
}

void HelloNetworkModule::onPacketReceived(void(*f)(shared_ptr<HelloPacket> packet))
{
	packetReceivedCallback = f;
}


