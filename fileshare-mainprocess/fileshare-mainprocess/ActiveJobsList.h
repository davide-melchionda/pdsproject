#pragma once

#include "wrapperclass.cpp"
#include "Job.h"

class ActiveJobsList : public WrapperClass<Job>
{
private:
	ActiveJobsList() {};
public:
	static ActiveJobsList& getInstance() //"Meyers" Singleton
	{
		static ActiveJobsList instance;
		return instance;
	};
};

