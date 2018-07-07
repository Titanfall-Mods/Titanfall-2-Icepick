using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icepick.Api
{
	public class IcepickRegistry
	{
		private const string RegistryKey = @"SOFTWARE\TitanfallIcepick";
		private const string GameLocationKey = "InstallPath";

		public static void WriteValue( string keyName, object value )
		{
			RegistryKey key = Registry.LocalMachine.CreateSubKey( RegistryKey );
			key.SetValue( keyName, value );
			key.Close();
		}

		public static T ReadValue<T>( string keyName )
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey( RegistryKey );
			if( key  != null )
			{
				T value = (T) key.GetValue( keyName );
				key.Close();
				return value;
			}
			else
			{
				return default(T);
			}
		}

		public static void ClearRegistry()
		{
			Registry.LocalMachine.DeleteSubKey( RegistryKey );
		}

		public static void WriteGameInstallPath( string path )
		{
			WriteValue( GameLocationKey, path );
		}

		public static string ReadGameInstallPath()
		{
			return ReadValue<string>( GameLocationKey );
		}

	}
}
