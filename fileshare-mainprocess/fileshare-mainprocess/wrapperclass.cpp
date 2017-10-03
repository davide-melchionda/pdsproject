#include "stdafx.h"
#include <string>
#include <iostream>
#include <stdio.h>
#include <fstream>
#include <thread>
#include<windows.h>
#include <condition_variable>
#include <map>
#include <mutex>

#pragma once
using namespace std;
template<class T>
class WrapperClass
{
private:
	map<string, T > myMap;
	WrapperClass() {}

public:
	condition_variable cv;
	mutex m;
	static WrapperClass& getInstance() //"Meyers" Singleton
	{
		static WrapperClass instance;
		return instance;
	}
	int insert(pair<string, T > element)
	{
		pair<map<string, string>::iterator, bool> ret;

		unique_lock<mutex> ul(m);
		ret = this->myMap.insert(element);
		if (ret.second == false) {
			std::cout << "element already existed";
			std::cout << " with a value of " << ret.first->second << '\n';
		}
		cv.notify_all();
		return 0;
	}

	//del elimina l'entry corrispondente a key e ritorna il numero di entry cancellate
	int WrapperClass<T>::del(string key)
	{
		unique_lock<mutex> ul(m);
		int ret = this->myMap.erase(key);
		cv.notify_all();
		return ret;

	}
	//restituisce 1 se va a buon fine e riempie il T passato come parametro
	int WrapperClass<T>::get(string key, T *container)
	{

		try {
			*container = myMap.at(key);
		}
		catch (out_of_range& ofr) {
			return 0;
		}

		return 1;
	}
};
