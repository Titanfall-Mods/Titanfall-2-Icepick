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
		private const string RegistrySubKey = @"SOFTWARE\TitanfallIcepick";
		private const string GameLocationKey = "InstallPath";

		private const string RespawnRegistrySubKey = @"SOFTWARE\WOW6432Node\Respawn\Titanfall2";
		private const string RespawnInstallDirKey = "Install Dir";
		private const string GameExecutable = "Titanfall2.exe";

		public static void WriteValue( string keyName, object value )
		{
			RegistryKey key = Registry.LocalMachine.CreateSubKey( RegistrySubKey, RegistryKeyPermissionCheck.ReadWriteSubTree );
			key.SetValue( keyName, value );
			key.Close();
		}

		public static T ReadValue<T>( string keyName )
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey( RegistrySubKey );
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
			RegistryKey key = Registry.LocalMachine.OpenSubKey( RegistrySubKey );
			if ( key != null )
			{
				key.Close();
				Registry.LocalMachine.DeleteSubKey( RegistrySubKey );
			}
		}

		public static void WriteGameInstallPath( string path )
		{
			WriteValue( GameLocationKey, path );
		}

		public static string ReadGameInstallPath()
		{
			return ReadValue<string>( GameLocationKey );
		}

		public static string AttemptReadRespawnRegistryPath()
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey( RespawnRegistrySubKey );
			if ( key != null )
			{
				string value = (string) key.GetValue( RespawnInstallDirKey );
				key.Close();
				return value != null ? value + GameExecutable : null;
			}
			else
			{
				return null;
			}
		}

	}
}
