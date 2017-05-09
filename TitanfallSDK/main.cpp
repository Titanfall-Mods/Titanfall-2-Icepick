
#define WIN32_LEAN_AND_MEAN 1
#include <windows.h>
#include <stdio.h>
#include <io.h>
#include <iostream>
#include <conio.h>
#include <string>
#include <fstream>
#include <float.h>
#include <memory>
#include <fcntl.h>

#include "Console/Console.h"
#include "Utils/Util.h"

using namespace std;

namespace
{
	std::unique_ptr<CConsole> gbl_mConsole;
}

extern "C" __declspec(dllexport) DWORD InitializeSDKConsole(LPVOID lpParameter)
{
	if (!gbl_mConsole)
	{
		gbl_mConsole.reset(new CConsole());
	}
	return 0;
}

extern "C" __declspec(dllexport) DWORD InitializeSDK(LPVOID lpParameter)
{
	printf("Spyglass injection complete! \n");
	return 0;
}

BOOL WINAPI DllMain(HMODULE hModule, DWORD dwReason, LPVOID lpReserved)
{
	switch (dwReason)
	{
	case DLL_PROCESS_ATTACH:
		DisableThreadLibraryCalls(hModule);
		return TRUE;
	case DLL_PROCESS_DETACH:
		// TODO: Perform cleanup
		gbl_mConsole.reset();
		return FALSE;
	}
	return TRUE;
}
