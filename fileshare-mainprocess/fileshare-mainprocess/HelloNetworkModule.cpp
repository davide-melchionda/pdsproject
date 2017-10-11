#include "stdafx.h"

#include "HelloNetworkModule.h"

#define DEBUG_OUTPUT

using json = nlohmann::json;

HelloNetworkModule::HelloNetworkModule()
{
	// Retrieves the username info from the config class.
	string username = "Davide";	// PLACEHOLDER
	//ConfClass::getInfo().getUsername();
	
	// When the application starts, a random generated id is used.
	// The next part of the protocol will automatically set the correct id.
	srand(time(0));
	int randid = rand();
	char c[21];
	
	// Creates the local peer
	local = Peer(username + ":" + string(itoa(randid, c, 10)), username, "none");

#ifdef DEBUG_OUTPUT
	printf("\nInitialising Winsock...");
#endif
	// Winsock startup
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
	
	// Creates a socket
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

	// Prepares the sockaddr_in structure
	server.sin_family = AF_INET;
	server.sin_addr.s_addr = INADDR_ANY;
	server.sin_port = htons(MULTIDESTPORT);

	// Prepares the multicast ip address of the group to join
	struct ip_mreq mreq;
	mreq.imr_interface.s_addr = INADDR_ANY;
	mreq.imr_multiaddr.s_addr = inet_addr(MULTICAST_GROUP_ADDR);
	
	// Registers to the multicast group
	setsockopt(multiRecvSocket, IPPROTO_IP, IP_ADD_MEMBERSHIP, (char *)&mreq, sizeof(mreq));

	// Bind of the socket to the address
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

	// Creates the incoming socket
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



void HelloNetworkModule::sendUnicast(HelloPacket& packet, string dest)
{
	// Acquires the lock: avoid any other access of the send socket
	unique_lock<mutex> lock(sendSocketMutex);

	struct sockaddr_in si_other;	// The address of the receiver
	int slen = sizeof(si_other);	// The length of the ipaddress
	char sending[BUFLEN];

	// Setup address structure for the receiver Ip
	memset((char *)&si_other, 0, sizeof(si_other));
	si_other.sin_family = AF_INET;
	si_other.sin_port = htons(MULTIDESTPORT);
	si_other.sin_addr.S_un.S_addr = inet_addr(dest.c_str());

	// Transforms the packet in a json in order to send it
	string jsonPacket = parser.marshal(packet).dump();
	// Need to convert to c_string and put the termination character
	strcpy(sending, jsonPacket.c_str());
	sending[jsonPacket.length()] = '\0';

	// Send the message
	if (sendto(sendSocket, sending, jsonPacket.length() + 1, 0, (struct sockaddr *) &si_other, slen) == SOCKET_ERROR)
	{
#ifdef DEBUG_OUTPUT
		printf("sendto() failed with error code : %d", WSAGetLastError());
#endif
		exit(EXIT_FAILURE);
	}
}



void HelloNetworkModule::send(HelloPacket& packet)
{
	// Acquire the lock: avoid any other access of the send socket
	unique_lock<mutex> lock(sendSocketMutex);

	struct sockaddr_in si_other;	// The address of the receiver
	int slen = sizeof(si_other);	// The length of the ipaddress
	char sending[BUFLEN];

	// Setup address structure
	memset((char *)&si_other, 0, sizeof(si_other));
	si_other.sin_family = AF_INET;
	si_other.sin_port = htons(MULTIDESTPORT);
	si_other.sin_addr.S_un.S_addr = inet_addr(MULTICAST_GROUP_ADDR);

	// Transforms the packet in a json in order to send it
	string jsonPacket = parser.marshal(packet).dump();
	// Need to convert to c_string and put the termination character
	strcpy(sending, jsonPacket.c_str());
	sending[jsonPacket.length()] = '\0';

	// Send the message
	if (sendto(sendSocket,sending , jsonPacket.length()+1, 0, (struct sockaddr *) &si_other, slen) == SOCKET_ERROR)
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

	// Try to receive some data, this is a blocking call
	if ((recv_len = recvfrom(multiRecvSocket, buf, BUFLEN, 0, (struct sockaddr *) &si_other, &slen)) == SOCKET_ERROR)
	{
#ifdef DEBUG_OUTPUT
		printf("recvfrom() failed with error code : %d", WSAGetLastError());
#endif
		exit(EXIT_FAILURE);
	}

#ifdef DEBUG_OUTPUT
	printf("Received packet from %s:%d\n", inet_ntoa(si_other.sin_addr), ntohs(si_other.sin_port));
	printf("Data: %s\n", buf);
#endif
	
	string s(buf);	// From c_str to std::string
	json receivedJson = json::parse(s);	// unescaping
	// Unmarshalls the packet received from the network ad obtaines a pointer.
	shared_ptr<HelloPacket> packet = parser.unmarshal(receivedJson);

	// Retrieves the sender ip address
	string senderip = inet_ntoa(si_other.sin_addr);
	
	// Calls the registered callback on the received packet.
	// An exception is thrown if the function pointer is null.
	packetReceivedCallback(packet, senderip);
}



void HelloNetworkModule::onPacketReceived(void(*f)(shared_ptr<HelloPacket> packet, string senderip))
{
	// Registers the provided callback
	packetReceivedCallback = f;
}


const Peer HelloNetworkModule::getPeer()
{
	return local;
}


void HelloNetworkModule::updatePeerAddress(std::string ipaddress)
{
	// Set the new address as peer ip address
	local.ipaddress = ipaddress;
	// Updates the id of the peer to be coherent with protocol specs
	local.id = local.name + ":" + ipaddress;
}