#include "stdafx.h"
#include "HelloSenderThread.h"
#include <Windows.h>

HelloSenderThread::HelloSenderThread()
{
}


HelloSenderThread::~HelloSenderThread()
{
}

void HelloSenderThread::operator() () {
	while (1) {
		//string ipadd = NetConf::getLocalipaddr() == "none" ? "noip:RAND:" + rand() : NetConf::getLocalipaddr();
		HelloNetworkModule& network = HelloNetworkModule::getInstance();
		network.send(KeepalivePacket(network.getPeer().id,network.getPeer().name,network.getPeer().ipaddress));
		Sleep(10000);
	}

}