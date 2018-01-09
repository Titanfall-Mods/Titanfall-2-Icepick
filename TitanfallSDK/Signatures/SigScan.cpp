
#include <iostream>
#include <Windows.h>
#include <tlhelp32.h>
#include <tchar.h>

#include "SigScan.h"
#include "../Utils/Util.h"

#include "MinHook.h"
#if defined _M_X64
#pragma comment(lib, "libMinHook-x64-v140-mt.lib")
#elif defined _M_IX86
#pragma comment(lib, "libMinHook-x86-v140-mt.lib")
#endif

using namespace Util;

// Squirrel 2.2.3 stable
typedef __int64 SQInteger;
typedef SQInteger SQRESULT;
typedef char SQChar;
typedef struct SQVM* HSQUIRRELVM;

typedef int (WINAPI *MESSAGEBOXW)(HWND, LPCWSTR, LPCWSTR, UINT);
typedef SQRESULT( *sq_throwerror )(HSQUIRRELVM, const SQChar *);

std::vector<SignatureF>* allSignatures = NULL;
sq_throwerror fpSqThrowError = nullptr;

MODULEINFO GetModuleInfo( std::string szModule )
{
	MODULEINFO modinfo = { 0 };
	HMODULE hModule = GetModuleHandle( (LPCWSTR) szModule.c_str() );
	if ( hModule == 0 )
		return modinfo;
	GetModuleInformation( GetCurrentProcess(), hModule, &modinfo, sizeof( MODULEINFO ) );
	return modinfo;
}

SignatureSearch::SignatureSearch( const char* funcname, void* adress, const char* signature, const char* mask, int offset )
{
	// lazy-init, container gets 'emptied' when initialized on compile.
	if ( !allSignatures )
	{
		allSignatures = new std::vector<SignatureF>();
	}

	SignatureF ins = { funcname, signature, mask, offset, adress };
	allSignatures->push_back( ins );
}

void SignatureSearch::Search()
{
	printf( "Scanning for signatures.\n" );
	std::vector<SignatureF>::iterator it;
	for ( it = allSignatures->begin(); it < allSignatures->end(); it++ )
	{
// 		*((void**) it->address) = (void*) (FindPattern( "client.dll", it->funcname, it->signature, it->mask ) + it->offset);
	}
}

//Internal Pattern scan, external pattern scan is just a wrapper around this
uintptr_t SignatureSearch::FindPattern( char* base, unsigned int size, char* pattern, char* mask )
{
	size_t patternLength = strlen( mask );

	for ( uintptr_t i = 0; i < size - patternLength; i++ )
	{
		bool found = true;
		for ( uintptr_t j = 0; j < patternLength; j++ )
		{
			if ( mask[j] != '?' && pattern[j] != *(char*) (base + i + j) )
			{
				found = false;
				break;
			}
		}

		if ( found )
		{
			return (uintptr_t) base + i;
		}
	}
	return 0;
}

//Scan just one module
uintptr_t SignatureSearch::FindPatternEx( HANDLE hProcess, MODULEINFO hModule, char* pattern, char* mask )
{
	//Grab module information from External Process
	uintptr_t start = (uintptr_t) hModule.lpBaseOfDll;
	uintptr_t end = start + hModule.SizeOfImage;

	uintptr_t currentChunk = start;
	SIZE_T bytesRead;

	while ( currentChunk < end )
	{
		//make data accessible to ReadProcessMemory
		DWORD oldprotect;
		VirtualProtectEx( hProcess, (void*) currentChunk, 4096, PROCESS_VM_READ, &oldprotect );

		//Copy chunk of external memory into local storage
		byte buffer[4096];
		ReadProcessMemory( hProcess, (void*) currentChunk, &buffer, 4096, &bytesRead );

		//if readprocessmemory failed, return
		if ( bytesRead == 0 )
		{
			return 0;
		}

		//Find pattern in local buffer, if pattern is found return address of matching data
		uintptr_t InternalAddress = FindPattern( (char*) &buffer, bytesRead, pattern, mask );

		//if Find Pattern returned an address
		if ( InternalAddress != 0 )
		{
			//convert internal offset to external address and return
			uintptr_t offsetFromBuffer = InternalAddress - (uintptr_t) &buffer;
			return currentChunk + offsetFromBuffer;
		}

		//pattern not found in this chunk
		else
		{
			//advance to next chunk
			currentChunk = currentChunk + bytesRead;
		}
	}
	return 0;
}

HMODULE SignatureSearch::GetProcessModule( HANDLE hProcess, TCHAR* targetModuleName )
{
	HMODULE hMods[1024];
	DWORD cbNeeded;
	unsigned int i;

	if ( EnumProcessModules( hProcess, hMods, sizeof( hMods ), &cbNeeded ) )
	{
		for ( i = 0; i < (cbNeeded / sizeof( HMODULE )); i++ )
		{
			TCHAR szModName[MAX_PATH];
			if ( GetModuleFileNameEx( hProcess, hMods[i], szModName, sizeof( szModName ) / sizeof( TCHAR ) ) )
			{
				std::wstring szModName_w( szModName );
				auto modNameParts = SplitString( szModName_w, L'\\' );
				const wchar_t * moduleName = modNameParts[modNameParts.size() - 1].c_str();
				int cmpName = _tcscmp( moduleName, targetModuleName );

				_tprintf( TEXT( "\t%s (0x%8X) [%s == %i]\n" ), szModName, hMods[i], moduleName, cmpName );
				if ( cmpName == 0 )
				{
					return hMods[i];
				}

			}
		}
	}

	return NULL;
}

void SignatureSearch::DebugInfo()
{
	if ( allSignatures )
	{
		printf( "Requested signatures: %u \n", allSignatures->size() );
		std::vector<SignatureF>::iterator it;
		for ( it = allSignatures->begin(); it < allSignatures->end(); it++ )
		{
			printf( "%s address: %08X \n", it->funcname, it->address );
		}
	}
	else
	{
		printf( "No signatures requested. \n" );
	}

	printf( "Process ID: %u \n", GetCurrentProcessId() );
	printf( "Modules info: \n" );

	HANDLE hProcess;
	HMODULE hMods[1024];
	DWORD cbNeeded;
	unsigned int i;

	hProcess = OpenProcess( PROCESS_ALL_ACCESS, FALSE, GetCurrentProcessId() );
	if ( NULL == hProcess )
		return;

	MODULEINFO modinfo = { 0 };
	if ( EnumProcessModules( hProcess, hMods, sizeof( hMods ), &cbNeeded ) )
	{
		for ( i = 0; i < (cbNeeded / sizeof( HMODULE )); i++ )
		{
			TCHAR szModName[MAX_PATH];
			if ( GetModuleFileNameEx( hProcess, hMods[i], szModName, sizeof( szModName ) / sizeof( TCHAR ) ) )
			{
				_tprintf( TEXT( "\t%s (0x%8X)\n" ), szModName, hMods[i] );
			}
		}
	}
	
	CloseHandle( hProcess );
}

// -----------------------------------------------------------------------------

SQRESULT hk_sq_throwerror( HSQUIRRELVM v, const SQChar *err )
{
	std::cout << "[ERROR] " << err << std::endl;
	return fpSqThrowError( v, err );
}

void SignatureSearch::TestScan()
{
	HANDLE hProcess;
	hProcess = OpenProcess( PROCESS_ALL_ACCESS, FALSE, GetCurrentProcessId() );
	if ( NULL == hProcess )
	{
		printf( "Could not open process \n" );
		return;
	}

	HMODULE hClient = SignatureSearch::GetProcessModule( hProcess, _T( "client.dll" ) );
	if ( hClient == NULL )
	{
		printf( "Could not find client module \n" );
		return;
	}

	printf( "Found client module! \n" );

	MODULEINFO hClientInfo = { 0 };
	GetModuleInformation( GetCurrentProcess(), hClient, &hClientInfo, sizeof( MODULEINFO ) );
	printf( "Client module info: \n" );
	printf( "\t EntryPoint: 0x%8X", hClientInfo.EntryPoint );
	printf( "\t lpBaseOfDll: 0x%8X", hClientInfo.lpBaseOfDll );
	printf( "\t SizeOfImage: 0x%8X", hClientInfo.SizeOfImage );
	printf( "\n" );

	printf( "Attempt sigscan... \n" );
	if ( hClientInfo.EntryPoint == 0 )
	{
		printf( "Not sigscanning as entry point was invalid. \n" );
		return;
	}

	const char* sFuncName = "sq_throwerror";
	char* sPattern = "\x48\x89\x5C\x24\x08\x48\x89\x74\x24\x10\x57\x48\x83\xEC\x30\x48\x8B\x59\x50";
	char* sMask = "xxxxxxxxxxxxxxxxxxx";

	uintptr_t patternPointer = FindPatternEx( hProcess, hClientInfo, sPattern, sMask );
	printf( "\t Found patternPointer?: 0x%8X \n", patternPointer );

	MH_STATUS stInit = MH_Initialize();
	if ( stInit == MH_OK )
	{
		printf( "MinHook Initialized! \n" );
	}
	else
	{
		printf( "MinHook failed to initialize (%i) \n", stInit );
		return;
	}

	MH_STATUS stCreateHook = MH_CreateHook( (LPVOID) patternPointer, &hk_sq_throwerror, reinterpret_cast<LPVOID*>(&fpSqThrowError) );
	if ( stCreateHook == MH_OK )
	{
		printf( "MH_CreateHook success! \n" );
	}
	else
	{
		printf( "MH_CreateHook failed (%i) \n", stCreateHook );
		return;
	}

	MH_STATUS stEnableHook = MH_EnableHook( (LPVOID) patternPointer );
	if ( stEnableHook == MH_OK )
	{
		printf( "MH_EnableHook success! \n" );
	}
	else
	{
		printf( "MH_EnableHook failed (%i) \n", stEnableHook );
		return;
	}

}
