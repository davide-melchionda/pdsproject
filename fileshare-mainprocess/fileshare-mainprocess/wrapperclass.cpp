#include "stdafx.h"
#include <string>
#include <iostream>
#include <stdio.h>
#include <fstream>
#include <thread>
#include <vector>
#include<windows.h>
#include <condition_variable>
#include <map>
#include <mutex>

#pragma once
using namespace std;
template<class T>
class WrapperClass
{
protected:
	map<string, T > myMap;			// The map to wrap
	//condition_variable cv;			
	condition_variable insertCv;	// Event: an insert appened
	condition_variable deleteCv;	// Event: a delete appened
	mutex m;

	WrapperClass() {}				// Constructor

public:
	int insert(pair<string, T > element)
	{
		pair<map<string, T>::iterator, bool> ret;

		unique_lock<mutex> ul(m);
		ret = this->myMap.insert(element);
		if (ret.second == false) {
			std::cout << "element already existed" << std::endl;
			//std::cout << " with a value of " << ret.first->second << '\n';
		}
		//cv.notify_all();
		insertCv.notify_all();
		return 0;
	}

	//del elimina l'entry corrispondente a key e ritorna il numero di entry cancellate
	int WrapperClass<T>::del(string key)
	{
		unique_lock<mutex> ul(m);
		int ret = this->myMap.erase(key);
		//cv.notify_all();
		deleteCv.notify_all();
		return ret;
	}

	/* Restituisce 1 se va a buon fine e riempie il T passato come parametro 
		@throws out_of_range exception */
	int WrapperClass<T>::get(string key, T& container)
	{

		/* Problema: il paradigma di ricerca con restituzione di un oggetto null
		   in caso di assenza dell'oggetto cercato nella lista non è implementabile
		   in c++. In questo caso viene lanciata un'eccezione di tipo out_of_range
		   se non è presente l'oggetto. Il chiamante si deve preoccupare di catchare
		   l'eccezione. */
		container = myMap.at(key);

		/* 
		// Old version: doesn't throw the exception
		try {
			container = myMap.at(key);
		}
		catch (out_of_range& ofr) {
			return 0;
		}
		*/

		return 1;
	}
	vector<pair<string, T>>  WrapperClass<T>::getAll()
	{
		vector<pair<string, T>> v;

		for (map<string, T >::iterator it = myMap.begin(); it != myMap.end(); ++it) {
			v.push_back(pair< string, T>(it->first, it->second));
		}
		return v;
	}

	bool WrapperClass<T>::exists(string key) {
		return (myMap.find(key) != myMap.end());
	}

	void onInsert(void(*f)()) {
		thread t(&WrapperClass<T>::waitingFunctionIns, this, 0, f);
		t.detach();
	}

	void onDelete(void(*f)()) {
		thread t(&WrapperClass<T>::waitingFunctionDel, this, 1, f);
		t.detach();
	}

	// DEBUG - waitingFunctionIns e waitingFunctionDel devono essere un'unica funzione
	//		   che differenzia la cv su cui ascoltare tramite l'intero i.
	void waitingFunctionIns(int i, void(*f)()) {
		while (1) {
			unique_lock<mutex> ul(m);
			insertCv.wait(ul);
			f();
		}
	}

	// DEBUG - waitingFunctionIns e waitingFunctionDel devono essere un'unica funzione
	//		   che differenzia la cv su cui ascoltare tramite l'intero i.
	void waitingFunctionDel(int i, void(*f)()) {
		while (1) {
			unique_lock<mutex> ul(m);
			insertCv.wait(ul);
			f();
		}
	}

};
