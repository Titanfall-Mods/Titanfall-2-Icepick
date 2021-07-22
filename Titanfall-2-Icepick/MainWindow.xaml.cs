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
		private const string SteamAppsDirectory = "steamapps";
		private const string EADesktopDirectory = "EA Games";

		List<string> eventHistory;
		Settings settings;
		Launcher selectedLauncher;

		public MainWindow()
		{
			InitializeComponent();

			eventHistory = new List<string>();
			settings = new Settings();

			btnDisableCrashReports.DataContext = settings;
			btnEnableDeveloperMode.DataContext = settings;

			CrashReporting.CrashReporter.StartWatching();
			CrashReporting.CrashReporter.OnCrashDumpProcessed += CrashReporter_OnDumpProcessed;

			SDKInjector.OnLaunchingProcess += SDKInjector_OnLaunchingProcess;
			SDKInjector.OnInjectingIntoProcess += SDKInjector_OnInjectingIntoProcess;
			SDKInjector.OnInjectionComplete += SDKInjector_OnInjectionComplete;
			SDKInjector.OnInjectionException += SDKInjector_OnInjectionException;

			ModDatabase.OnStartedLoadingMods += ModDatabase_OnStartedLoadingMods;
			ModDatabase.OnFinishedLoadingMods += ModDatabase_OnFinishedLoadingMods;
			ModDatabase.OnModLoaded += ModDatabase_OnModLoaded;
			ModDatabase.OnFinishedImportingMod += ModDatabase_OnFinishedImportingMod;
			ModDatabase.LoadAll();

			UpdateLauncherSelection();
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
			AddEvent( "Successfully injected into Titanfall 2!" );
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

		private void OpenTitanfallModsDiscord_Click( object sender, RoutedEventArgs e )
		{
			Process.Start( Api.ApiRoutes.GetDiscord() );
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
			ModDatabase.ShowFolder(ModDatabase.ModsDirectory);
		}

		private void OpenSavesFolder_Click( object sender, RoutedEventArgs e )
		{
			ModDatabase.ShowFolder(ModDatabase.SavesDirectory);
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

		private void SelectGameLocation_Click( object sender, RoutedEventArgs e )
		{
			ShowSelectGameLocation();
		}

		private void CleanupRegistry_Click( object sender, RoutedEventArgs e )
		{
			Api.IcepickRegistry.ClearRegistry();
		}

		private void Quit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void LaunchGame_Click( object sender, RoutedEventArgs e )
		{
			string gamePath = Api.IcepickRegistry.AttemptReadRespawnRegistryPath() ?? Api.IcepickRegistry.ReadGameInstallPath();
			if ( string.IsNullOrEmpty( gamePath ) )
			{
				ShowSelectGameLocation();
				gamePath = Api.IcepickRegistry.ReadGameInstallPath();
				UpdateLauncherSelection();
			}
			if ( string.IsNullOrEmpty( gamePath ) )
			{
				MessageBox.Show( "You must specify the path to your Titanfall 2 installation before you can use the Icepick.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation );
				return;
			}

			SDKInjector.LaunchAndInject( selectedLauncher, gamePath );
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

		private void UpdateLauncherSelection()
		{
			// Default to Origin since that was the original release
			selectedLauncher = Launcher.Origin;

			// Use Steam launch if the install path contains steamapps
			string gamePath = Api.IcepickRegistry.AttemptReadRespawnRegistryPath() ?? Api.IcepickRegistry.ReadGameInstallPath();
			if ( !string.IsNullOrEmpty( gamePath ) )
			{
				if ( gamePath.Contains( SteamAppsDirectory ) )
				{
					selectedLauncher = Launcher.Steam;
				}
				else if ( gamePath.Contains( EADesktopDirectory ) )
				{
					selectedLauncher = Launcher.EADesktop;
				}
			}

			launcherComboBox.SelectedIndex = (int) selectedLauncher;
		}

		private void SelectedLauncherChanged( object sender, SelectionChangedEventArgs e )
		{
			selectedLauncher = (Launcher) launcherComboBox.SelectedIndex;
		}
	}
}
