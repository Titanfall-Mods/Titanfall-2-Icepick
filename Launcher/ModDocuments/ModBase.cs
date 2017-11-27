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
					// Get the bytes we wish to write
					string ReplacementPath = $"{ParentMod.Path}{System.IO.Path.DirectorySeparatorChar}{ReplacedCodeFile}";
					byte[] FileBytes = System.IO.File.ReadAllBytes( ReplacementPath );

					// Get the new memory address to write to
					IntPtr NewWriteAddress = Modder.MemoryModder.Instance.AllocateMemory( FileBytes );
					Debug.WriteLine( $"{this} got write address: {NewWriteAddress.ToInt64().ToString("X")}" );
					if ( NewWriteAddress.ToInt64() > 0 )
					{
						int BytesWritten = Modder.MemoryModder.Instance.WriteMemory( NewWriteAddress.ToInt64(), FileBytes );
						Debug.WriteLine( $"BytesWritten to new address: {BytesWritten}" );
					}

					// Find the address of the existing memory
					long ExistingAddress = Modder.MemoryModder.Instance.FindAddress( Encoding.ASCII.GetBytes( ComparisonString ) );
					Debug.WriteLine( $"ExistingAddress: {ExistingAddress.ToString( "X" )}" );
					if ( ExistingAddress > 0 )
					{
						// Convert the address of the existing memory into a byte array
						byte[] HexBytes = LongToByteArray( ExistingAddress, true );
						Debug.WriteLine( "Hex Bytes:" );
						foreach ( byte b in HexBytes )
						{
							Debug.Write( $"{b.ToString( "X" )}, " );
						}
						Debug.WriteLine( "" );

						// Search for the address of the pointer to the existing memory
						long PointerAddress = Modder.MemoryModder.Instance.FindAddress( HexBytes );
						Debug.WriteLine( $"PointerAddress: {PointerAddress.ToString( "X" )}" );
						if( PointerAddress > 0 )
						{
							// Write the new memory pointer
							long WriteAddress = NewWriteAddress.ToInt64();
							byte[] WriteBytes = LongToByteArray( WriteAddress, true );
							Modder.MemoryModder.Instance.WriteMemory( PointerAddress, WriteBytes );
							return true;
						}
					}

				}
				catch ( Exception )
				{
					return false;
				}
				return false;
			}

			public byte[] LongToByteArray( long Input, bool IsLittleEndian = false )
			{
				string HexString = Input.ToString( "X" );
				if( HexString.Length % 2 != 0)
				{
					HexString = "0" + HexString;
				}

				int NumBytes = HexString.Length;
				byte[] Bytes = new byte[ NumBytes / 2 ];
				for ( int i = 0; i < NumBytes; i += 2 )
				{
					byte NewByte = Convert.ToByte( HexString.Substring( i, 2 ), 16 );
					Bytes[ i / 2 ] = NewByte;
				}

				if ( IsLittleEndian )
				{
					Bytes = Bytes.Reverse().ToArray();
				}

				return Bytes;
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
