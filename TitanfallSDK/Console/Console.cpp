
#include <windows.h>
#include <conio.h>
#include <FCNTL.H>
#include <io.h>
#include "console.h"
#include <iostream>

namespace
{
	class outbuf : public std::streambuf
	{
	public:
		outbuf()
		{
			setp(0, 0);
		}

		virtual int_type overflow(int_type c = traits_type::eof()) override
		{
			return fputc(c, stdout) == EOF ? traits_type::eof() : c;
		}
	};

	outbuf obuf;
	std::streambuf *sb = nullptr;
}

static BOOL WINAPI MyConsoleCtrlHandler(DWORD dwCtrlEvent) { return dwCtrlEvent == CTRL_C_EVENT; }

CConsole::CConsole() : m_OwnConsole(false)
{
	if (!AllocConsole()) return;

	SetConsoleCtrlHandler(MyConsoleCtrlHandler, TRUE);
	RemoveMenu(GetSystemMenu(GetConsoleWindow(), FALSE), SC_CLOSE, MF_BYCOMMAND);
	
	CONSOLE_SCREEN_BUFFER_INFO coninfo;

	// allocate a console for this app
	AllocConsole();

	// set the screen buffer to be big enough to let us scroll text
	GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), &coninfo);
	coninfo.dwSize.Y = MAX_CONSOLE_LINES;
	SetConsoleScreenBufferSize(GetStdHandle(STD_OUTPUT_HANDLE), coninfo.dwSize);

	// redirect output to console
	freopen_s(&m_conFile, "CONOUT$", "w", stdout);

	m_OwnConsole = true;
}

CConsole::~CConsole() {
	if (m_OwnConsole) {
		std::cout.rdbuf(sb);
		fclose(m_conFile);
		SetConsoleCtrlHandler(MyConsoleCtrlHandler, FALSE);
		FreeConsole();
	}
}
