
#pragma once

#include <string>
#include <vector>
#include <Psapi.h>

struct SignatureF
{
	const char* funcname;
	const char* signature;
	const char* mask;
	int offset;
	void* address;
};

class SignatureSearch
{
public:
	SignatureSearch( const char* funcname, void* address, const char* signature, const char* mask, int offset );
	static void Search();
	static uintptr_t FindPattern( char* base, unsigned int size, char* pattern, char* mask );
	static uintptr_t FindPatternEx( HANDLE hProcess, MODULEINFO hModule, char* pattern, char* mask );

	static HMODULE GetProcessModule( HANDLE hProcess, TCHAR* targetModuleName );

	static void DebugInfo();
	static void TestScan();
};

