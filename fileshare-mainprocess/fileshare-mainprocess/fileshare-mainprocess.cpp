// fileshare-mainprocess.cpp : definisce il punto di ingresso dell'applicazione console.
//

#pragma once

#include "stdafx.h"
#include "WrapperClass.cpp"
#include "HelloThread.h"
#include <condition_variable>
#include <map>
#include <mutex>
#include <vector>

int main()
{
	 /*WrapperClass<string>& wc = WrapperClass<string>::getInstance();
	 WrapperClass<string>& wc1 = WrapperClass<string>::getInstance();

	wc.insert(pair< string, string>("zio", "nicola"));
	wc1.insert(pair< string, string>("zio", "gigi"));

	vector<pair<string,string>> vectore= wc.getAll();
	cout << vectore.back().first;*/
	HelloThread hello;
	std::thread helloThread(hello);
	helloThread.join();

    return 0;
}

