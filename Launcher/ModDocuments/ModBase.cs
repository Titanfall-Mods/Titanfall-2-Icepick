using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Launcher.Modder;

namespace Launcher.ModDocuments
{
	public abstract class ModBase
	{
		public string ModPath;
		public bool Active;
		public List<ModFile> Files = new List<ModFile>();

		protected string Name;
		protected string Description;
		protected float Version;
		protected List<string> Authors = new List<string>();
		protected List<string> Contacts = new List<string>();

		protected List<FileSystemWatcher> FileWatchers = new List<FileSystemWatcher>();
		protected Dictionary<string, DateTime> FileLastChanged = new Dictionary<string, DateTime>();

		public ModBase()
		{
		}

		public static bool ShouldLoad( string ModPath )
		{
			return true;
		}

		public virtual void Load( string InPath )
		{
			ModPath = InPath;
		}

		public void WriteToMemory()
		{
			Debug.WriteLine( $"Writing {Name} files to memory..." );
			Parallel.ForEach( Files, ( F ) =>
			{
				bool Success = F.WriteIntoMemory( this );
				if ( !Success )
				{
					Debug.WriteLine( $"Failed to write mod file into memory: {F}" );
				}
			} );
		}

		public override string ToString()
		{
			return $"[Mod {ModPath}]";
		}

		public void Compile()
		{
			foreach ( ModFile File in Files )
			{
				File.CompileFile();
			}
		}

		public void WatchFiles()
		{
			CleanupFileWatchers();
			foreach( ModFile File in Files )
			{
				AddFileWatcher( File.ReplacementFile );
				foreach( string AppendFile in File.AppendedFiles )
				{
					AddFileWatcher( AppendFile );
				}
			}
		}

		protected void AddFileWatcher( string ModRelativeFilePath )
		{
			string Directory = Path.GetDirectoryName( ModRelativeFilePath );
			string FileName = Path.GetFileName( ModRelativeFilePath );
			string ModRelativeDirectory = $"{ModPath}{Path.DirectorySeparatorChar}{Directory}";
			Console.WriteLine( $"{ModPath} watching file: {ModRelativeFilePath}" );

			FileSystemWatcher Watcher = new FileSystemWatcher();
			Watcher.Path = ModRelativeDirectory;
			Watcher.NotifyFilter = NotifyFilters.LastWrite;
			Watcher.Filter = FileName;
			Watcher.EnableRaisingEvents = true;
			Watcher.Changed += OnModFileChanged;
			FileWatchers.Add( Watcher );
		}

		public void CleanupFileWatchers()
		{
			foreach( var Watcher in FileWatchers )
			{
				Watcher.EnableRaisingEvents = false;
				Watcher.Dispose();
			}
			FileWatchers.Clear();
		}

		private void OnModFileChanged( object sender, FileSystemEventArgs e )
		{
			// Hack: Only recompile every X seconds due to file watcher always raising the changed event twice for every save
			bool bRecompile = true;
			if( FileLastChanged.ContainsKey( e.FullPath ) )
			{
				TimeSpan TimeSinceLastUpdated = DateTime.UtcNow - FileLastChanged[ e.FullPath ];
				bRecompile = TimeSinceLastUpdated.TotalMilliseconds > 250;
			}

			if ( bRecompile )
			{
				Console.WriteLine( $"{e.FullPath} updated, recompiling mods..." );
				ModsCompiler.CompileAllMods();
				FileLastChanged[ e.FullPath ] = DateTime.UtcNow;
			}
		}

	}
}
