
#include <iostream>
#include <Windows.h>
#include <tlhelp32.h>
#include <tchar.h>

#include "SigScan.h"
#include "../SquirrelTypes.h"
#include "../Utils/Util.h"

#include "MinHook.h"
#if defined _M_X64
#pragma comment(lib, "libMinHook-x64-v140-mtd.lib")
#elif defined _M_IX86
#pragma comment(lib, "libMinHook-x86-v140-mtd.lib")
#endif

using namespace Util;

std::vector<FSignature> * AllSignatures;

MODULEINFO GetModuleInfo( std::string szModule )
{
	MODULEINFO modinfo = { 0 };
	HMODULE hModule = GetModuleHandle( (LPCWSTR) szModule.c_str() );
	if ( hModule == 0 )
		return modinfo;
	GetModuleInformation( GetCurrentProcess(), hModule, &modinfo, sizeof( MODULEINFO ) );
	return modinfo;
}

SignatureSearch::SignatureSearch( const std::string ModuleName, const std::string FuncName, void * Address, void * HookAddress, const std::string Signature, const std::string Mask, int Offset )
{
	if ( AllSignatures == NULL )
	{
		AllSignatures = new std::vector<FSignature>();
	}

	FSignature NewSig;
	NewSig.ModuleName = ModuleName;
	NewSig.FunctionName = FuncName;
	NewSig.Signature = Signature;
	NewSig.Mask = Mask;
	NewSig.Offset = Offset;
	NewSig.Address = Address;
	NewSig.HookFunction = HookAddress;
	AllSignatures->push_back( NewSig );
}

void SignatureSearch::Search()
{
	printf( "Scanning for signatures.. \n" );

	// Initialize minhook
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

	// Open the process
	HANDLE hProcess;
	hProcess = OpenProcess( PROCESS_ALL_ACCESS, FALSE, GetCurrentProcessId() );
	if ( NULL == hProcess )
	{
		printf( "Could not open process! \n" );
		return;
	}

	// Attempt to hook all registered signatures
	for( FSignature & Sig : *AllSignatures )
	{
		HookSignature( hProcess, Sig );
	}

	// Done, close the process
	CloseHandle( hProcess );
}

void SignatureSearch::HookSignature( HANDLE hProcess, FSignature & Sig )
{
	std::cout << "Scanning for: " << Sig.FunctionName << " in " << Sig.ModuleName << std::endl;

	std::wstring wModuleName = std::wstring( Sig.ModuleName.begin(), Sig.ModuleName.end() );
	HMODULE hModule = SignatureSearch::GetProcessModule( hProcess, wModuleName.c_str() );
	if ( hModule == NULL )
	{
		std::cout << "Could not find module: " << Sig.ModuleName << std::endl;
		return;
	}
	std::cout << "Found module: " << Sig.ModuleName << std::endl;

	MODULEINFO hModuleInfo = { 0 };
	GetModuleInformation( GetCurrentProcess(), hModule, &hModuleInfo, sizeof( MODULEINFO ) );
	printf( "\t EntryPoint: 0x%p", hModuleInfo.EntryPoint );
	printf( "\t lpBaseOfDll: 0x%p", hModuleInfo.lpBaseOfDll );
	printf( "\t SizeOfImage: 0x%08X", hModuleInfo.SizeOfImage );
	printf( "\n" );
	if ( hModuleInfo.EntryPoint == 0 )
	{
		printf( "Not sigscanning as entry point was invalid. \n" );
		return;
	}

	uintptr_t patternPointer = FindPatternEx( hProcess, hModuleInfo, Sig.Signature.c_str(), Sig.Mask.c_str() );
	printf( "\t Found patternPointer?: 0x%llx \n", (unsigned long long) patternPointer );

	MH_STATUS stCreateHook = MH_CreateHook( (LPVOID) patternPointer, Sig.HookFunction, reinterpret_cast<LPVOID*>(Sig.Address) );
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

// Internal Pattern scan, external pattern scan is just a wrapper around this
uintptr_t SignatureSearch::FindPattern( char* base, unsigned int size, const char* pattern, const char* mask )
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

// Scan just one module
uintptr_t SignatureSearch::FindPatternEx( HANDLE hProcess, MODULEINFO hModule, const char* pattern, const char* mask )
{
	// Grab module information from External Process
	uintptr_t start = (uintptr_t) hModule.lpBaseOfDll;
	uintptr_t end = start + hModule.SizeOfImage;

	uintptr_t currentChunk = start;
	SIZE_T bytesRead;

	while ( currentChunk < end )
	{
		// make data accessible to ReadProcessMemory
		DWORD oldprotect;
		VirtualProtectEx( hProcess, (void*) currentChunk, 4096, PROCESS_VM_READ, &oldprotect );

		// Copy chunk of external memory into local storage
		byte buffer[4096];
		ReadProcessMemory( hProcess, (void*) currentChunk, &buffer, 4096, &bytesRead );

		// if readprocessmemory failed, return
		if ( bytesRead == 0 )
		{
			return 0;
		}

		// Find pattern in local buffer, if pattern is found return address of matching data
		uintptr_t InternalAddress = FindPattern( (char*) &buffer, bytesRead, pattern, mask );

		// if Find Pattern returned an address
		if ( InternalAddress != 0 )
		{
			// convert internal offset to external address and return
			uintptr_t offsetFromBuffer = InternalAddress - (uintptr_t) &buffer;
			return currentChunk + offsetFromBuffer;
		}

		// pattern not found in this chunk
		else
		{
			// advance to next chunk
			currentChunk = currentChunk + bytesRead;
		}
	}
	return 0;
}

HMODULE SignatureSearch::GetProcessModule( HANDLE hProcess, const TCHAR* targetModuleName )
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

				// _tprintf( TEXT( "\t%s (0x%p) [%s == %i]\n" ), szModName, hMods[i], moduleName, cmpName );
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
				_tprintf( TEXT( "\t%s (0x%p)\n" ), szModName, hMods[i] );
			}
		}
	}
	
	CloseHandle( hProcess );
}
