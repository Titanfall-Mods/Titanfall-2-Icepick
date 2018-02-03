using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.ModDocuments
{
	public abstract class ModBase
	{
		public class AppendedFile
		{
			public string AppendTo;
			public string ScriptPath;
		}

		public string Path;
		protected string Name;
		protected string Description;
		protected float Version;
		protected List<string> Authors = new List<string>();
		protected List<string> Contacts = new List<string>();
		public List<ModFile> Files = new List<ModFile>();
		public List<AppendedFile> AppendedFiles = new List<AppendedFile>();

		public ModBase()
		{
		}

		public static bool ShouldLoad( string ModPath )
		{
			return true;
		}

		public virtual void Load( string ModPath )
		{
			Path = ModPath;
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
			return $"[Mod {Path}]";
		}

	}
}
