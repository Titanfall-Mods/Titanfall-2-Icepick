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
		public struct File
		{
			public string ComparisonString;
			public string ReplacedCodeFile;
			public long AddressOffset;

			public bool WriteIntoMemory( ModBase ParentMod )
			{
				Debug.WriteLine( $"Writing {this} to memory..." );
				try
				{
					long Address = Modder.MemoryModder.Instance.FindAddress( Encoding.ASCII.GetBytes( ComparisonString ) );
					Debug.WriteLine( $"{this} found address: {Address}" );

					if ( Address > 0 )
					{
						Address += AddressOffset;
						string ReplacementPath = $"{ParentMod.Path}{System.IO.Path.DirectorySeparatorChar}{ReplacedCodeFile}";
						byte[] FileBytes = System.IO.File.ReadAllBytes( ReplacementPath );
						Modder.MemoryModder.Instance.WriteMemory( Address, FileBytes );
						return true;
					}
				}
				catch ( Exception )
				{
					return false;
				}
				return false;
			}

			public override string ToString()
			{
				return $"[ModFile {ReplacedCodeFile} (@{AddressOffset})]";
			}
		}

		public string Path;
		protected string Name;
		protected string Description;
		protected float Version;
		protected List<string> Authors = new List<string>();
		protected List<string> Contacts = new List<string>();
		protected List<File> Files = new List<File>();

		public ModBase()
		{
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
