
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
#include <vector>

#include "Console/Console.h"
#include "Signatures/SigScan.h"

using namespace std;

namespace
{
	std::unique_ptr<CConsole> gbl_mConsole;
}

extern "C" __declspec(dllexport) DWORD InitializeSDK(LPVOID lpParameter)
{
	printf("Icepick injection complete! \n");
	return 0;
}

extern "C" __declspec(dllexport) DWORD InitializeSDKConsole( LPVOID lpParameter )
{
	if ( !gbl_mConsole )
	{
		gbl_mConsole.reset( new CConsole() );
	}
	return 0;
}

extern "C" __declspec(dllexport) DWORD TestSigscan( LPVOID lpParameter )
{
	printf( "Testing c++ sigscan \n" );

// 	SignatureSearch( "sq_throwerror", &sq_throwerror, "\x48\x89\x5C\x24\x08\x48\x89\x74\x24\x10\x57\x48\x83\xEC\x30\x48\x8B\x59\x50", "xxxxxxxxxxxxxxxxxxx", 0 );
// 	SignatureSearch::Search();

	SignatureSearch::DebugInfo();
	SignatureSearch::TestScan();

	return 0;
}

void OnAttach()
{
	printf("Attached to process, created thread\n");

	// Print process injected into
	vector<wchar_t> pathBuf;
	DWORD copied = 0;
	do
	{
		pathBuf.resize(pathBuf.size() + MAX_PATH);
		copied = GetModuleFileName(0, &pathBuf.at(0), pathBuf.size());
	} while (copied >= pathBuf.size());
	pathBuf.resize(copied);
	wstring path(pathBuf.begin(), pathBuf.end());
	std::wcout << "Injected dll into module: " << path << "\n";

}

BOOL WINAPI DllMain(HMODULE hModule, DWORD dwReason, LPVOID lpReserved)
{
	switch (dwReason)
	{
	case DLL_PROCESS_ATTACH:
		// DisableThreadLibraryCalls(hModule);
		CreateThread(0, 0, (LPTHREAD_START_ROUTINE)OnAttach, 0, 0, 0);
		return TRUE;
	case DLL_PROCESS_DETACH:
		// TODO: Perform cleanup
		gbl_mConsole.reset();
		return FALSE;
	}
	return TRUE;
}
