
#pragma once

#include <string>
#include <vector>
#include <Psapi.h>

// typedef SQRESULT( *sq_throwerror )(HSQUIRRELVM, const SQChar *);
// sq_throwerror fpSqThrowError = nullptr;

#define CREATE_SIGNATURE( ModuleName, ReturnType, FuncName, HookAddress, Signature, Mask, Offset, ... ) \
	typedef ReturnType( *FuncName ## ptr )( __VA_ARGS__ ); \
	FuncName ## ptr FuncName = nullptr; \
	ReturnType HookAddress( __VA_ARGS__ ); \
	SignatureSearch FuncName ## search( ModuleName, #FuncName, &FuncName, &HookAddress, Signature, Mask, Offset );

struct oldSignatureF
{
	const char* funcname;
	const char* signature;
	const char* mask;
	int offset;
	void* address;
};

struct FSignature
{
	std::string FunctionName;
	std::string ModuleName;
	std::string Signature;
	std::string Mask;
	void * HookFunction;
	int Offset;
	void * Address;
};

class SignatureSearch
{
public:

	SignatureSearch( const std::string ModuleName, const std::string FuncName, void * Address, void * HookAddress, const std::string Signature, const std::string Mask, int Offset );
	static void Search();
	static void HookSignature( HANDLE hProcess, FSignature & Sig );
	static uintptr_t FindPattern( char* base, unsigned int size, const char* pattern, const char* mask );
	static uintptr_t FindPatternEx( HANDLE hProcess, MODULEINFO hModule, const char* pattern, const char* mask );

	static HMODULE GetProcessModule( HANDLE hProcess, const TCHAR* targetModuleName );

	static void DebugInfo();
	static void TestScan();
};
