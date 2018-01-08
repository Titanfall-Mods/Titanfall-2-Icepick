
#include <string>
#include <sstream>
#include <vector>
#include <iterator>

#include "Util.h"

namespace Util
{
	template<typename Out>
	void SplitString( const std::string &s, char delim, Out result )
	{
		std::stringstream ss( s );
		std::string item;
		while ( std::getline( ss, item, delim ) )
		{
			*(result++) = item;
		}
	}

	std::vector<std::string> SplitString( const std::string &s, char delim )
	{
		std::vector<std::string> elems;
		SplitString( s, delim, std::back_inserter( elems ) );
		return elems;
	}

	std::vector<std::wstring> SplitString( const std::wstring &str, wchar_t delim )
	{
		std::wstring temp;
		std::vector<std::wstring> parts;
		std::wstringstream wss( str );
		while ( std::getline( wss, temp, delim ) )
		{
			parts.push_back( temp );
		}
		return parts;
	}
}
