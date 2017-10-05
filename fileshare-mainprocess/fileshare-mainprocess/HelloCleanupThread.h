#pragma once

class HelloCleanupThread
{
public:
	HelloCleanupThread();
	~HelloCleanupThread();

	void operator() ();
};

