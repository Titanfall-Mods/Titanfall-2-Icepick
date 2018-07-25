using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Icepick.Mods;
using Microsoft.Win32;

namespace Icepick
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string TitanfallExecutableFilter = "Titanfall 2 (Titanfall2.exe)|Titanfall2.exe";
		private const string TitanfallDefaultInstallDir = @"C:\Program Files (x86)\Origin Games\Titanfall2\";

		List<string> eventHistory;
		Settings settings;

		public MainWindow()
		{
			InitializeComponent();

			eventHistory = new List<string>();
			settings = new Settings();

			btnDisableCrashReports.DataContext = settings;
			btnEnableDeveloperMode.DataContext = settings;

			CrashReporting.CrashReporter.StartWatching();
			CrashReporting.CrashReporter.OnCrashDumpProcessed += CrashReporter_OnDumpProcessed;

			Api.ApiQueue.OnApiRequestIssued += ApiQueue_OnApiRequestIssued;
			Api.ApiQueue.OnApiRequestResult += ApiQueue_OnApiRequestResult;

			SDKInjector.OnLaunchingProcess += SDKInjector_OnLaunchingProcess;
			SDKInjector.OnInjectingIntoProcess += SDKInjector_OnInjectingIntoProcess;
			SDKInjector.OnInjectionComplete += SDKInjector_OnInjectionComplete;
			SDKInjector.OnInjectionException += SDKInjector_OnInjectionException;

			ModDatabase.OnStartedLoadingMods += ModDatabase_OnStartedLoadingMods;
			ModDatabase.OnFinishedLoadingMods += ModDatabase_OnFinishedLoadingMods;
			ModDatabase.OnModLoaded += ModDatabase_OnModLoaded;
			ModDatabase.OnFinishedImportingMod += ModDatabase_OnFinishedImportingMod;
			ModDatabase.LoadAll();
		}

		private void AddEvent( string eventDescription, bool ignoreStatus = false )
		{
			eventHistory.Add( $"{DateTime.Now.ToShortTimeString()} : {eventDescription}" );
			if ( !ignoreStatus )
			{
				lblStatusText.Text = eventDescription;
			}
		}

        private void CrashReporter_OnDumpProcessed( bool success, string name )
        {
            if (success)
            {
                AddEvent($"Crash report {name} submitted successfully", true);
            }
            else
            {
                AddEvent($"Failed to upload crash report {name}", true);
            }
        }

		private void ApiQueue_OnApiRequestIssued( string apiPath )
		{
			AddEvent( $"Issued api request to {apiPath}", true );
		}

		private void ApiQueue_OnApiRequestResult( string apiPath, bool success, Api.ApiResult result )
		{
			if( success )
			{
				AddEvent( $"Api request to {apiPath} succeeded! {result.rawData}", true );

				if( apiPath == Api.ApiRoutes.GetApiRoute( "IcepickInfo" ) )
				{
					string latestIcepickId = null;
					if( result.data.TryGetValue( "_id", out latestIcepickId ) )
					{
						if( latestIcepickId != Icepick.Version.Current )
						{
							OnIcepickUpdateAvailable();
						}
					}
				}
			}
			else
			{
				if ( result != null )
				{
					AddEvent( $"Api request to {apiPath} failed with message: {result.message}", true );
				}
				else
				{
					AddEvent( $"Api request to {apiPath} failed with no return result.", true );
				}
				
			}
		}

		private void SDKInjector_OnLaunchingProcess( string message )
		{
			AddEvent( "Launching Titanfall 2 and waiting for injection..." );
		}

		private void SDKInjector_OnInjectingIntoProcess( string message )
		{
			AddEvent( "Injecting into Titanfall 2 process..." );
		}

		private void SDKInjector_OnInjectionComplete( string message )
		{
			AddEvent( "Succesfully injected into Titanfall 2!" );
		}

		private void SDKInjector_OnInjectionException( string message )
		{
			AddEvent( "An exception occurred while injecting! " + message );
		}

		private void ModDatabase_OnStartedLoadingMods()
		{
			AddEvent( "Loading mods..." );
		}

		private void ModDatabase_OnFinishedLoadingMods()
		{
			AddEvent( "Finished loading mods!" );
			CheckForAllUpdates();
		}

		private void ModDatabase_OnModLoaded( TitanfallMod mod )
		{
			ModsPanel.Children.Add( new Controls.ModItem( mod ) );
		}

		private void ModDatabase_OnFinishedImportingMod( bool success, ModDatabase.ModImportType importType, string message )
		{
			if( success )
			{
				AddEvent( $"Successfully imported mod! {message}" );

				switch ( importType )
				{
					case ModDatabase.ModImportType.Mod:
						ReloadModsList();
						break;
				}
			}
			else
			{
				AddEvent( $"Failed to import mod, {message}" );
			}
		}

		private void OpenTitanfallMods_Click( object sender, RoutedEventArgs e)
		{
			Process.Start( Api.ApiRoutes.GetSite() );
		}
		
		private void ViewEventLog_Click( object sender, RoutedEventArgs e )
		{
			bool makeVisible = !EventLogButton.IsChecked;
			ModsViewer.Visibility = makeVisible ? Visibility.Hidden : Visibility.Visible;
			EventLogViewer.Visibility = makeVisible ? Visibility.Visible : Visibility.Hidden;

			if ( makeVisible )
			{
				EventLog.Children.Clear();
				foreach ( string historicEvent in eventHistory )
				{
					TextBlock block = new TextBlock();
					block.Text = historicEvent;
					EventLog.Children.Add( block );
				}
			}

			EventLogButton.IsChecked = EventLogViewer.Visibility == Visibility.Visible;
		}

		private void OpenModsFolder_Click( object sender, RoutedEventArgs e )
		{
			ModDatabase.ShowModsFolder();
		}

		private void OpenSavesFolder_Click( object sender, RoutedEventArgs e )
		{
			ModDatabase.ShowSavesFolder();
		}

		private void ReloadMods_Click( object sender, RoutedEventArgs e )
		{
			ReloadModsList();
		}

		private void About_Click( object sender, RoutedEventArgs e )
		{
			Controls.AboutIcepick about = new Controls.AboutIcepick();
			about.Show();
		}

		private void CheckForUpdates_Click( object sender, RoutedEventArgs e )
		{
			CheckForAllUpdates();
		}

		private void SelectGameLocation_Click( object sender, RoutedEventArgs e )
		{
			ShowSelectGameLocation();
		}

		private void CleanupRegistry_Click( object sender, RoutedEventArgs e )
		{
			Api.IcepickRegistry.ClearRegistry();
		}

		private void LaunchGame_Click( object sender, RoutedEventArgs e )
		{
			string gamePath = Api.IcepickRegistry.AttemptReadRespawnRegistryPath() ?? Api.IcepickRegistry.ReadGameInstallPath();
			if ( string.IsNullOrEmpty( gamePath ) )
			{
				ShowSelectGameLocation();
				gamePath = Api.IcepickRegistry.ReadGameInstallPath();
			}
			if ( string.IsNullOrEmpty( gamePath ) )
			{
				MessageBox.Show( "You must specify the path to your Titanfall 2 installation before you can use the Icepick.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation );
				return;
			}

			SDKInjector.LaunchAndInject( gamePath );
		}

		private void Icepick_Drop( object sender, DragEventArgs e )
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				string[] files = (string[]) e.Data.GetData( DataFormats.FileDrop );
				foreach( string file in files )
				{
					AddEvent( $"Attempting to import potential mod: {file}" );
					ModDatabase.AttemptImportMod( file );
				}
			}
		}

		private void ReloadModsList()
		{
			ModsPanel.Children.Clear();
			ModDatabase.ClearDatabase();
			ModDatabase.LoadAll();
		}

		private void CheckForAllUpdates()
		{
			foreach( TitanfallMod mod in ModDatabase.LoadedMods )
			{
				mod.CheckForUpdates();
			}
		}

		private void ShowSelectGameLocation()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.CheckPathExists = true;
			openFileDialog.Filter = TitanfallExecutableFilter;
			openFileDialog.InitialDirectory = TitanfallDefaultInstallDir;
			if ( openFileDialog.ShowDialog() == true )
			{
				if( !string.IsNullOrWhiteSpace( openFileDialog.FileName ) )
				{
					Api.IcepickRegistry.WriteGameInstallPath( openFileDialog.FileName );
				}
			}
		}

		private void OnIcepickUpdateAvailable()
		{
			// Show update on the Icepick framework mod
			foreach ( TitanfallMod mod in ModDatabase.LoadedMods )
			{
				if( mod.IsIcepickFramework )
				{
					mod.IcepickUpdateAvailable();
				}
			}

			// Show a popup too to nudge people to download it more
			MessageBoxResult result = MessageBox.Show( "An update to the Icepick is available! It is highly recommend you download this update as soon as possible.\n\nDo you wish to go to the download page now?", "Icepick Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information );
			if( result == MessageBoxResult.Yes )
			{
				foreach ( TitanfallMod mod in ModDatabase.LoadedMods )
				{
					if ( mod.IsIcepickFramework )
					{
						mod.OpenDownloadPage();
					}
				}
			}
		}

	}
}
