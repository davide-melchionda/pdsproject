#pragma once

#include<winsock2.h>
#include <Ws2tcpip.h>
#include <mswsock.h>

#pragma comment(lib, "Ws2_32.lib")

#include<stdio.h>
#include <string>

#define BUFLEN 512  //Max length of buffer
#define PORT 8888   //The port on which to listen for incoming data

#ifndef u_int32
#define u_int32 UINT32  // Unix uses u_int32
#endif // !u_int32

#include <iostream>

#include "HelloPacket.h"
#include "HelloPacketParser.h"
#include <condition_variable>

#define MULTICAST_GROUP_ADDR "234.5.6.7"

class HelloNetworkModule
{
private:
	void(*packetReceivedCallback)(shared_ptr<HelloPacket>);
	SOCKET s;
	struct sockaddr_in server, si_other;
	int slen, recv_len;
	WSADATA wsa;
	HelloPacketParser parser;

protected:
	HelloNetworkModule();

public:
	static HelloNetworkModule& getInstance() //"Meyers" Singleton
	{
		static HelloNetworkModule instance;
		return instance;
	};

	void sendUnicast(HelloPacket packet, string dest);
	void send(HelloPacket packet);
	void receive();
	void onPacketReceived(void (*f)(shared_ptr<HelloPacket> packet));
	
	~HelloNetworkModule();
};

