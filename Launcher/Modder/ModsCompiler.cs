using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Launcher.ModDocuments;

namespace Launcher.Modder
{
	public static class ModsCompiler
	{
		public const string COMPILED_OUTPUT_DIRECTORY = ".processed";

		public static void CleanOutputDirectory()
		{
			if( Directory.Exists( COMPILED_OUTPUT_DIRECTORY ) )
			{
				DeleteDirectory( COMPILED_OUTPUT_DIRECTORY );
			}
		}

		private static void DeleteDirectory( string DirectoryPath )
		{
			string[] Files = Directory.GetFiles( DirectoryPath );
			string[] Dirs = Directory.GetDirectories( DirectoryPath );

			foreach ( string FilePath in Files )
			{
				File.SetAttributes( FilePath, FileAttributes.Normal );
				File.Delete( FilePath );
			}
			foreach ( string Dir in Dirs )
			{
				DeleteDirectory( Dir );
			}
			Directory.Delete( DirectoryPath, false );
		}

		public static void CompileAllMods()
		{
			CleanOutputDirectory();
			if ( !Directory.Exists( COMPILED_OUTPUT_DIRECTORY ) )
			{
				Directory.CreateDirectory( COMPILED_OUTPUT_DIRECTORY );
			}

			foreach( ModBase Mod in SpyglassLauncher.DisplayedMods )
			{
				if ( Mod.Active )
				{
					Console.WriteLine( $"Compiling {Mod.ModPath}" );
					Mod.Compile();
				}
			}

		}

	}
}
