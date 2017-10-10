#include "stdafx.h"

//#define DEBUG_OUTPUT

#include "HelloNetworkModule.h"

HelloNetworkModule::HelloNetworkModule()
{
#ifdef DEBUG_OUTPUT
	printf("\nInitialising Winsock...");
#endif
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
	{
#ifdef DEBUG_OUTPUT
		printf("Failed. Error Code : %d", WSAGetLastError());
#endif
		exit(EXIT_FAILURE);
	}
#ifdef DEBUG_OUTPUT
	printf("Initialised.\n");
#endif
	//Create a socket
	if ((multiRecvSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) == INVALID_SOCKET)
	{
#ifdef DEBUG_OUTPUT
		printf("Could not create socket : %d", WSAGetLastError());
#endif
		exit(EXIT_FAILURE);
	}
#ifdef DEBUG_OUTPUT
	printf("Socket created.\n");
#endif

	//Prepare the sockaddr_in structure
	server.sin_family = AF_INET;
	server.sin_addr.s_addr = INADDR_ANY;
	server.sin_port = htons(MULTIDESTPORT);

	//join_source_group(s, str_to_uint32("244.12.12.12"), INADDR_ANY);
	// Prepares the multicast ip address of the group to join to
	struct ip_mreq mreq;
	mreq.imr_interface.s_addr = INADDR_ANY;
	mreq.imr_multiaddr.s_addr = inet_addr(MULTICAST_GROUP_ADDR);
	// Registers to the multicast group
	setsockopt(multiRecvSocket, IPPROTO_IP, IP_ADD_MEMBERSHIP, (char *)&mreq, sizeof(mreq));

	//Bind of the socket to the address
	if (::bind(multiRecvSocket, (struct sockaddr *)&server, sizeof(server)) == SOCKET_ERROR)
	{
#ifdef DEBUG_OUTPUT
		printf("Bind failed with error code : %d", WSAGetLastError());
#endif
		exit(EXIT_FAILURE);
	}
#ifdef DEBUG_OUTPUT
	puts("Bind done");
#endif

	// Create socket incoming socket
	if ((sendSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) == SOCKET_ERROR)
	{
#ifdef DEBUG_OUTPUT
		printf("socket() failed with error code : %d", WSAGetLastError());
#endif
		exit(EXIT_FAILURE);
	}
}



HelloNetworkModule::~HelloNetworkModule()
{
}



void HelloNetworkModule::sendUnicast(HelloPacket packet, string dest)
{
	struct sockaddr_in si_other;	// The address of the receiver
	int slen = sizeof(si_other);	// The length of the ipaddress

	// Setup address structure for the receiver Ip
	memset((char *)&si_other, 0, sizeof(si_other));
	si_other.sin_family = AF_INET;
	si_other.sin_port = htons(MULTIDESTPORT);
	si_other.sin_addr.S_un.S_addr = inet_addr(dest.c_str());

	// Makes the packet a string
	string jsonPacket = parser.marshal(packet);

	// Send the message
	if (sendto(sendSocket, jsonPacket.c_str(), jsonPacket.length(), 0, (struct sockaddr *) &si_other, slen) == SOCKET_ERROR)
	{
#ifdef DEBUG_OUTPUT
		printf("sendto() failed with error code : %d", WSAGetLastError());
#endif
		exit(EXIT_FAILURE);
	}
}



void HelloNetworkModule::send(HelloPacket packet)
{
	struct sockaddr_in si_other;	// The address of the receiver
	int slen = sizeof(si_other);	// The length of the ipaddress

	// Setup address structure
	memset((char *)&si_other, 0, sizeof(si_other));
	si_other.sin_family = AF_INET;
	si_other.sin_port = htons(MULTIDESTPORT);
	si_other.sin_addr.S_un.S_addr = inet_addr(MULTICAST_GROUP_ADDR);

	// Makes the packet a string
	string jsonPacket = parser.marshal(packet);
	
	// Send the message
	if (sendto(sendSocket, jsonPacket.c_str(), jsonPacket.length(), 0, (struct sockaddr *) &si_other, slen) == SOCKET_ERROR)
	{
#ifdef DEBUG_OUTPUT
		printf("sendto() failed with error code : %d", WSAGetLastError());
#endif
		exit(EXIT_FAILURE);
	}
}



void HelloNetworkModule::receive()
{
	char buf[BUFLEN];				// Buffer which will contain the received string
	struct sockaddr_in si_other;	// Structure which will contain the address of the sender
	int slen = sizeof(si_other),	// Will contain the length of the sender address
		recv_len;					// Will contain the number of byte received from the network					 

	//fflush(stdout);

	// Clear the buffer by filling null, it might have previously received data
	//memset(buf, '\0', BUFLEN);

	// Try to receive some data, this is a blocking call
	if ((recv_len = recvfrom(multiRecvSocket, buf, BUFLEN, 0, (struct sockaddr *) &si_other, &slen)) == SOCKET_ERROR)
	{
#ifdef DEBUG_OUTPUT
		printf("recvfrom() failed with error code : %d", WSAGetLastError());
#endif
		exit(EXIT_FAILURE);
	}

	// Print details of the client/peer and the data received
#ifdef DEBUG_OUTPUT
	printf("Received packet from %s:%d\n", inet_ntoa(si_other.sin_addr), ntohs(si_other.sin_port));
	printf("Data: %s\n", buf);
#endif
	
	// Unmarshalls the packet received from the network ad obtaines a pointer.
	shared_ptr<HelloPacket> packet = parser.unmarshal(buf);

	// Calls the registered callback on the received packet.
	// An exception is thrown if the function pointer is null.
	packetReceivedCallback(packet);
}



void HelloNetworkModule::onPacketReceived(void(*f)(shared_ptr<HelloPacket> packet))
{
	packetReceivedCallback = f;
}


