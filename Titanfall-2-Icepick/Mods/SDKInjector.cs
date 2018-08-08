using Icepick.Extensions;
using Syringe;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace Icepick.Mods
{
	public class SDKInjector
	{
		[StructLayout( LayoutKind.Sequential, Pack = 8 )]
		struct SDKSettings
		{
			[CustomMarshalAs( CustomUnmanagedType.LPStr )] public string BasePath;
			public bool DeveloperMode;
		};

		private const float InjectionTimeout = 30;
		private const string OriginProcessName = "Origin";
		public const string SDKDllName = "TTF2SDK.dll";
		private const string SDKDataPath = @"data\";
		private const string InitializeFunction = "InitialiseSDK";

		public delegate void InjectorEventDelegate( string message = null );
		public static event InjectorEventDelegate OnLaunchingProcess;
		public static event InjectorEventDelegate OnInjectingIntoProcess;
		public static event InjectorEventDelegate OnInjectionComplete;
		public static event InjectorEventDelegate OnInjectionException;

		public static void LaunchAndInject( string gamePath )
		{
			if( OnLaunchingProcess != null )
			{
				OnLaunchingProcess();
			}

			Process.Start( new ProcessStartInfo( gamePath ) );
			Task watchAndInjectTask = WatchAndInject( gamePath );
		}

		protected static async Task WatchAndInject( string gamePath )
		{
			string gameProcessName = System.IO.Path.GetFileNameWithoutExtension( gamePath );
			DateTime startTime = DateTime.Now;

			while ( (DateTime.Now - startTime).TotalSeconds < InjectionTimeout )
			{
				Process[] ttfProcesses = Process.GetProcessesByName( gameProcessName );
				if( ttfProcesses.Length > 0 )
				{
					Process ttfProcess = ttfProcesses[ 0 ];
					try
					{
						Process potentialOriginProcess = ttfProcess.GetParentProcess();
						if( potentialOriginProcess != null && potentialOriginProcess.ProcessName == OriginProcessName )
						{
							foreach ( ProcessModule module in ttfProcess.Modules )
							{
								if ( module.ModuleName == "tier0.dll" )
								{
									InjectSDK(ttfProcess);
									return;
								}
							}
						}
					}
					catch ( Win32Exception e )
					{
						if ( OnInjectionException != null )
						{
							OnInjectionException( e.Message + ", Error Code " + e.NativeErrorCode );
						}
					}
					catch ( Exception e )
					{
						if ( OnInjectionException != null )
						{
							OnInjectionException( e.Message );
						}
					}
				}

				await Task.Delay( 1000 );
			}

			// Will only reach here if injection doesn't occur within the timeout period, so log an event and show a popup
			string timeoutError = string.Format( "Timed out after {0} seconds. Could not find Titanfall 2 process.", InjectionTimeout );
			if ( OnInjectionException != null )
			{
				OnInjectionException( timeoutError );
			}
			MessageBox.Show( timeoutError, "Injection Failed", MessageBoxButton.OK, MessageBoxImage.Exclamation );
		}
		
		protected static void InjectSDK( Process targetProcess )
		{
			if( OnInjectingIntoProcess != null )
			{
				OnInjectingIntoProcess();
			}

			Injector syringe = new Injector( targetProcess );
			syringe.SetDLLSearchPath( System.IO.Directory.GetCurrentDirectory() );
			syringe.InjectLibrary( SDKDllName );

			SDKSettings settings = new SDKSettings();
			settings.BasePath = AppDomain.CurrentDomain.BaseDirectory + SDKDataPath;
			settings.DeveloperMode = Api.IcepickRegistry.ReadEnableDeveloperMode();
			syringe.CallExport( SDKDllName, InitializeFunction, settings );

			if( OnInjectionComplete != null )
			{
				OnInjectionComplete();
			}
		}

	}
}
