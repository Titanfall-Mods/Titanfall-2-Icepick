
#ifndef __UTIL_HEADER__
#define __UTIL_HEADER__

#include <exception>
#include <vector>
#include <string>
#include <sstream>

namespace Util
{
	// String split from http://stackoverflow.com/a/236803
	void SplitString( const std::string &s, char delim, std::vector<std::string> &elems );
	std::vector<std::string> SplitString( const std::string &s, char delim );

	std::vector<std::wstring> SplitString( const std::wstring &str, wchar_t delim );
}

#endif // __UTIL_HEADER__
