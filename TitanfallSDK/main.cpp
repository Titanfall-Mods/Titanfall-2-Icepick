
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

#include <squirrel.h> 
#include <sqstdio.h> 
#include <sqstdaux.h> 
#include <sq/sqvm.h>
#include <sq/sqstate.h>

#ifdef _MSC_VER
#pragma comment (lib ,"squirrel.lib")
#pragma comment (lib ,"sqstdlib.lib")
#endif

// #include "SquirrelTypes.h"
#include "Console/Console.h"
#include "Signatures/SigScan.h"

using namespace std;

namespace
{
	std::unique_ptr<CConsole> gbl_mConsole;
}

namespace TitanfallSDK
{

	struct SQVMSQCollectable
	{
		SQUnsignedInteger __vtref; // V-table reference?
		SQUnsignedInteger _uiRef;
		struct SQWeakRef *_weakref;
		SQCollectable *_next;
		SQCollectable *_prev;
		SQSharedState *_sharedstate;
	};

	CREATE_SIGNATURE( "client.dll", SQRESULT, cl_sq_throwerror, hk_cl_sq_throwerror, "\x48\x89\x5C\x24\x08\x48\x89\x74\x24\x10\x57\x48\x83\xEC\x30\x48\x8B\x59\x50", "xxxxxxxxxxxxxxxxxxx", 0, HSQUIRRELVM, const SQChar * );
// 	CREATE_SIGNATURE( "server.dll", SQRESULT, sv_sq_throwerror, hk_sv_sq_throwerror, "\x48\x89\x5C\x24\x08\x48\x89\x6C\x24\x10\x48\x89\x74\x24\x18\x57\x48\x83\xEC\x20\x48\x8B\x2A", "xxxxxxxxxxxxxxxxxxxxxxx", 0, HSQUIRRELVM, const SQChar * );

	void printfunc( HSQUIRRELVM v, const SQChar *s, ... )
	{
		va_list arglist;
		va_start( arglist, s );
		vprintf( s, arglist );
		va_end( arglist );
	}

	SQRESULT hk_cl_sq_throwerror( HSQUIRRELVM v, const SQChar *err )
	{
		std::cout << "[Client] Error on VM: " << v << ": " << err << std::endl;

		const SQVM & vm = *v;
		SQVMSQCollectable * asdf = (SQVMSQCollectable*) ((void*)v);
		SQSharedState *_sharedstate = asdf->_sharedstate;
		printf( " pre print func: %p \n", _sharedstate->_printfunc );
		_sharedstate->_printfunc = printfunc;
		printf( "post print func: %p \n", _sharedstate->_printfunc );

		return cl_sq_throwerror( v, err );
	}

// 	SQRESULT hk_sv_sq_throwerror( HSQUIRRELVM v, const SQChar *err )
// 	{
// 		std::cout << "[Server] Error on VM: " << v << ": " << err << std::endl;
// 		return sv_sq_throwerror( v, err );
// 	}
};

extern "C" __declspec(dllexport) DWORD InitializeSDK(LPVOID lpParameter)
{
	printf("Icepick injection complete! \n");
	OpenProcess( PROCESS_ALL_ACCESS, FALSE, GetCurrentProcessId() );
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
	SignatureSearch::Search();

// 	SignatureSearch::DebugInfo();
// 	SignatureSearch::TestScan();

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
