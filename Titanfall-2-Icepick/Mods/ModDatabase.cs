using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace Icepick.Mods
{
	public static class ModDatabase
	{
		public enum ModImportType
		{
			Invalid,
			Mod,
			Save
		}

		public const string ModsDirectory = @"data\mods";
		public const string SavesDirectory = @"data\saves";
		public const string DisabledFileName = "disabled";
		private const string ArchiveExtension = ".zip";

		public delegate void ModDatabaseDelegate();
		public delegate void TitanfallModDelegate( TitanfallMod Mod );
		public delegate void ImportModDelegate( bool success, ModImportType importType, string message );

		public static event ModDatabaseDelegate OnStartedLoadingMods;
		public static event TitanfallModDelegate OnModLoaded;
		public static event ModDatabaseDelegate OnFinishedLoadingMods;
		public static event ImportModDelegate OnFinishedImportingMod;

		public static List<TitanfallMod> LoadedMods = new List<TitanfallMod>();

		public static void ShowFolder(string directory)
		{
			string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);
			if (!Directory.Exists(path))
			{
				MessageBox.Show($"The directory '{path}' is missing!", "Missing Directory", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			System.Diagnostics.Process.Start(path);
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

			string modsFullDirectory = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, ModsDirectory );

			if ( Directory.Exists( modsFullDirectory ) )
			{
				foreach ( var modPath in Directory.GetDirectories( modsFullDirectory ) )
				{
					TitanfallMod newMod = new TitanfallMod( modPath );
					string modDirectory = Path.GetFileName( newMod.Directory );
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
				string modsFullDirectory = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, ModsDirectory );
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
							OnFinishedImportingMod( false, ModImportType.Invalid, $"A mod already exists in folder '{modFolderName}'!" );
						}
						return;
					}
				}

				try
				{
					bool foundModDefinition = false;
					bool foundSaveFile = false;

					ZipArchive zip = ZipFile.OpenRead( path );
					foreach( var entry in zip.Entries )
					{
						string[] parts = entry.Name.Split( '.' );
						if ( parts.Length > 2 && entry.Name.EndsWith( ".txt" ) )
						{
							foundSaveFile = true;
						}

						if( entry.Name == TitanfallMod.ModDocumentFile )
						{
							foundModDefinition = true;
						}

					}

					if ( foundModDefinition )
					{
						// Extract mod to the mods folder
						ZipFile.ExtractToDirectory( path, destinationFolder );

						if ( OnFinishedImportingMod != null )
						{
							OnFinishedImportingMod( true, ModImportType.Mod, $"{modFolderName} imported successfully!" );
						}
					}
					else if( foundSaveFile )
					{
						// Extract saves to the saves folder
						destinationFolder = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, SavesDirectory );
						ZipFile.ExtractToDirectory( path, destinationFolder );

						if ( OnFinishedImportingMod != null )
						{
							OnFinishedImportingMod( true, ModImportType.Save, $"{modFolderName} imported to saves successfully!" );
						}
					}
					else
					{
						throw new Exception( "Mod was not a valid mod, nor a save file." );
					}
				}
				catch( Exception e )
				{
					if ( OnFinishedImportingMod != null )
					{
						OnFinishedImportingMod( false, ModImportType.Invalid, $"An exception occurred while importing mod '{modFolderName}', {e.Message}" );
					}
					return;
				}
			}
		}

		public static string PackageMod( string path )
		{
			string exportPath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, ModsDirectory, Path.GetFileName( path ) ) + ArchiveExtension;
			string modDirectory = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, path );
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

		public static bool ToggleMod(string path)
		{
			string disabledFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ModsDirectory, Path.GetFileName(path), DisabledFileName);

			if (File.Exists(disabledFilePath))
			{
				File.Delete(disabledFilePath);
				return true;
			}

			File.Create(disabledFilePath).Close();
			return false;
		}

	}
}
