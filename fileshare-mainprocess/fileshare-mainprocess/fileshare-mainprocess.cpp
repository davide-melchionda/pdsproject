// fileshare-mainprocess.cpp : definisce il punto di ingresso dell'applicazione console.
//

#include "stdafx.h"
#include "WrapperClass.cpp"
#include <condition_variable>
#include <map>
#include <mutex>

int main()
{
	 WrapperClass<string>& wc = WrapperClass<string>::getInstance();
	 WrapperClass<string>& wc1 = WrapperClass<string>::getInstance();

	wc.insert(pair< string, string>("zio", "nicola"));
	wc1.insert(pair< string, string>("zio", "gigi"));

	

    return 0;
}

