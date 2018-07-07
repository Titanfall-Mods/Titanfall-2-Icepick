using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Icepick.Mods;

namespace Icepick
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<string> eventHistory;

		public MainWindow()
		{
			InitializeComponent();

			eventHistory = new List<string>();

			Api.ApiQueue.OnApiRequestIssued += ApiQueue_OnApiRequestIssued;
			Api.ApiQueue.OnApiRequestResult += ApiQueue_OnApiRequestResult;

			ModDatabase.OnStartedLoadingMods += ModDatabase_OnStartedLoadingMods;
			ModDatabase.OnFinishedLoadingMods += ModDatabase_OnFinishedLoadingMods;
			ModDatabase.OnModLoaded += ModDatabase_OnModLoaded;
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

		private void ApiQueue_OnApiRequestIssued( string apiPath )
		{
			AddEvent( $"Issued api request to {apiPath}", true );
		}

		private void ApiQueue_OnApiRequestResult( string apiPath, bool success, Api.ApiResult result )
		{
			if( success )
			{
				AddEvent( $"Api request to {apiPath} succeeded! {result.rawData}", true );
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
			string path = System.IO.Path.Combine( Environment.CurrentDirectory, ModDatabase.MODS_DIRECTORY );
			Process.Start( path );
		}

		private void ReloadMods_Click( object sender, RoutedEventArgs e )
		{
			ModsPanel.Children.Clear();
			ModDatabase.ClearDatabase();
			ModDatabase.LoadAll();
		}

		private void CheckForUpdates_Click( object sender, RoutedEventArgs e )
		{
			CheckForAllUpdates();
		}

		private void CheckForAllUpdates()
		{
			Api.ApiQueue.ApiRequest( "IcepickInfo" );
			foreach( TitanfallMod mod in ModDatabase.LoadedMods )
			{
				mod.CheckForUpdates();
			}
		}

	}
}
