using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Icepick.Mods
{
	public static class ModDatabase
	{
		public const string ModsDirectory = "data/mods";
		private const string ArchiveExtension = ".zip";

		public delegate void ModDatabaseDelegate();
		public delegate void TitanfallModDelegate( TitanfallMod Mod );
		public delegate void ImportModDelegate( bool success, string message );

		public static event ModDatabaseDelegate OnStartedLoadingMods;
		public static event TitanfallModDelegate OnModLoaded;
		public static event ModDatabaseDelegate OnFinishedLoadingMods;
		public static event ImportModDelegate OnFinishedImportingMod;

		public static List<TitanfallMod> LoadedMods = new List<TitanfallMod>();

		public static void ShowModsFolder()
		{
			string path = System.IO.Path.Combine( Environment.CurrentDirectory, ModDatabase.ModsDirectory );
			System.Diagnostics.Process.Start( path );
		}

		public static void ClearDatabase()
		{
			LoadedMods.Clear();
		}

		public static void LoadAll()
		{
			if ( OnStartedLoadingMods != null )
			{
				OnStartedLoadingMods();
			}

			if ( Directory.Exists( ModsDirectory ) )
			{
				foreach ( var ModPath in Directory.GetDirectories( ModsDirectory ) )
				{
					TitanfallMod newMod = new TitanfallMod( ModPath );
					string modDirectory = System.IO.Path.GetFileName( newMod.Directory );
					if ( !modDirectory.StartsWith( "." ) )
					{
						LoadedMods.Add( newMod );
						if ( OnModLoaded != null )
						{
							OnModLoaded( newMod );
						}
					}
				}
			}

			if ( OnFinishedLoadingMods != null )
			{
				OnFinishedLoadingMods();
			}
		}

		public static void AttemptImportMod( string path )
		{
			if ( Path.GetExtension( path ) == ArchiveExtension )
			{
				string modsFullDirectory = Path.Combine( Environment.CurrentDirectory, ModsDirectory );
				Console.WriteLine( modsFullDirectory );

				// Check if a mod already exists
				string modFolderName = Path.GetFileNameWithoutExtension( path );
				string destinationFolder = Path.Combine( modsFullDirectory, modFolderName );
				if( Directory.Exists( destinationFolder ) )
				{
					MessageBoxResult overwriteResult = MessageBox.Show( $"'{modFolderName}' already exists in your mods folder.\n\nDo you wish to overwrite it?", "Overwrite Mod?", MessageBoxButton.YesNo );
					if( overwriteResult == MessageBoxResult.Yes )
					{
						Directory.Delete( destinationFolder, true );
					}
					else
					{
						if ( OnFinishedImportingMod != null )
						{
							OnFinishedImportingMod( false, $"A mod already exists in folder '{modFolderName}'!" );
						}
						return;
					}
				}

				try
				{
					ZipFile.ExtractToDirectory( path, destinationFolder );
				}
				catch( Exception e )
				{
					if ( OnFinishedImportingMod != null )
					{
						OnFinishedImportingMod( false, $"An exception occurred while importing mod '{modFolderName}', {e.Message}" );
					}
					return;
				}

				if ( OnFinishedImportingMod != null )
				{
					OnFinishedImportingMod( true, $"{modFolderName} imported successfully!" );
				}
			}
		}

		public static string PackageMod( string path )
		{
			string exportPath = Path.Combine( Environment.CurrentDirectory, ModsDirectory, Path.GetFileName( path ) ) + ArchiveExtension;
			string modDirectory = Path.Combine( Environment.CurrentDirectory, path );
			string errorMessage = null;

			try
			{
				ZipFile.CreateFromDirectory( modDirectory, exportPath );
			}
			catch( Exception e )
			{
				errorMessage = e.Message;
			}

			return errorMessage;
		}

	}
}
