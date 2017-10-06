#pragma once

#include "wrapperclass.cpp"
#include "Job.h"

class JobsList : public WrapperClass<Job>
{
private:
	JobsList() {};
public:
	static JobsList& getInstance() //"Meyers" Singleton
	{
		static JobsList instance;
		return instance;
	};
};

