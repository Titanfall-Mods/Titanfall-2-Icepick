using Microsoft.Win32;
using System.IO;

namespace Icepick.Api
{
	public class IcepickRegistry
	{
		private const string RegistrySubKey = @"SOFTWARE\TitanfallIcepick";
		private const string GameLocationKey = "InstallPath";
		private const string DisableCrashReportsKey = "DisableCrashReports";
		private const string EnableDeveloperModeKey = "EnableDeveloperMode";
		private const string MachineUUIDKey = "MachineUUID";

		private const string RespawnRegistrySubKey = @"SOFTWARE\WOW6432Node\Respawn\Titanfall2";
		private const string RespawnInstallDirKey = "Install Dir";
		private const string GameExecutable = "Titanfall2.exe";

		public static void WriteValue( string keyName, object value )
		{
			RegistryKey key = Registry.CurrentUser.CreateSubKey( RegistrySubKey, RegistryKeyPermissionCheck.ReadWriteSubTree );
			key.SetValue( keyName, value );
			key.Close();
		}

		public static T ReadValue<T>( string keyName )
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey( RegistrySubKey );
			T value = default(T);
			if ( key != null )
			{
				object val = key.GetValue( keyName );
				if ( val != null )
				{
					value = (T)val;
				}
			}

			key?.Close();
			return value;
		}

		public static void ClearRegistry()
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey( RegistrySubKey );
			if ( key != null )
			{
				key.Close();
				Registry.CurrentUser.DeleteSubKey( RegistrySubKey );
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

		public static void WriteDisableCrashReports( bool disableCrashReports )
		{
			WriteValue( DisableCrashReportsKey, disableCrashReports ? 1 : 0 );
		}

		public static bool ReadDisableCrashReports()
		{
			return ReadValue<int>( DisableCrashReportsKey ) == 1 ? true : false;
		}

		public static void WriteEnableDeveloperMode(bool enableDeveloperMode)
		{
			WriteValue(EnableDeveloperModeKey, enableDeveloperMode ? 1 : 0);
		}

		public static bool ReadEnableDeveloperMode()
		{
			return ReadValue<int>(EnableDeveloperModeKey) == 1 ? true : false;
		}

		public static void WriteMachineUUID( string uuid )
		{
			WriteValue( MachineUUIDKey, uuid );
		}

		public static string ReadMachineUUID()
		{
			return ReadValue<string>( MachineUUIDKey );
		}

		public static string AttemptReadRespawnRegistryPath()
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey( RespawnRegistrySubKey );
			if ( key != null )
			{
				string value = (string) key.GetValue( RespawnInstallDirKey );
				key.Close();
				return value != null ? Path.Combine(value, GameExecutable) : null;
			}
			else
			{
				return null;
			}
		}

	}
}
