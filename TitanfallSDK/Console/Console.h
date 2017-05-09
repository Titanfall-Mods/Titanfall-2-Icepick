
#ifndef __CONSOLE_HEADER__
#define __CONSOLE_HEADER__

// Credit: Christopher Oicles
// http://stackoverflow.com/questions/16500726/open-write-to-console-from-a-c-dll
// Stack overflow post

#include <stdio.h>

static const WORD MAX_CONSOLE_LINES = 500;

class CConsole {
	bool m_OwnConsole;
	FILE * m_conFile;
public:
	CConsole();
	~CConsole();
};

#endif
