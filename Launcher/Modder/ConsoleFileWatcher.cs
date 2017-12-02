using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Modder
{
	public class ConsoleFileWatcher
	{
		private FileSystemWatcher Watcher;
		private DateTime? LastUsedWriteTime;

		public ConsoleFileWatcher( string GamePath )
		{
			Watcher = new FileSystemWatcher();
			UpdateWatchPath( GamePath );
			Watcher.NotifyFilter = NotifyFilters.LastWrite;
			Watcher.Filter = "*.txt";
			Watcher.Changed += Watcher_Changed;
			Watcher.EnableRaisingEvents = true;
		}

		public void UpdateWatchPath( string GamePath )
		{
			Watcher.Path = Path.GetDirectoryName( GamePath );
		}

		private void Watcher_Changed( object sender, FileSystemEventArgs e )
		{
			// Titanfall writes twice, so ignore the log file if the write time was too close to the other one
			DateTime LastWrite = File.GetLastWriteTimeUtc( e.FullPath );
			if ( LastUsedWriteTime != null )
			{
				if ( ( LastWrite - LastUsedWriteTime ).Value.TotalMilliseconds < 4 )
				{
					return;
				}
			}

			try
			{
				// Read the file
				StreamReader FileReader = new StreamReader( e.FullPath );
				string Contents = FileReader.ReadToEnd();
				FileReader.Close();

				// If successful then keep the time and write to the console
				LastUsedWriteTime = LastWrite;
				Debug.WriteLine( "[Black]" + Contents );
			}
			catch ( Exception )
			{
				// Could not open file, ignore
			}
		}

	}
}
