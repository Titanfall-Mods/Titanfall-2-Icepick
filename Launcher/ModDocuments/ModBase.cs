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
		public class File
		{
			public enum WriteState
			{
				Waiting,
				FindingExistingScript,
				FindingPointerToScript,
				FindingNewMemory,
				WritingToNewMemory,
				OverwritingPointer,
				Finished
			}

			public string ComparisonString;
			public string ReplacedCodeFile;
			public long AddressOffset;

			public float Progress;
			public bool? Success;
			public WriteState State;
			public string LastError = string.Empty;

			public bool WriteIntoMemory( ModBase ParentMod )
			{
				Reset();

				bool Result = Internal_WriteIntoMemory( ParentMod );
				Success = Result;
				if ( Result )
				{
					UpdateStatus( WriteState.Finished );
				}

				return Result;
			}

			private void Reset()
			{
				UpdateStatus( WriteState.Waiting );
				Success = null;
			}

			private bool Internal_WriteIntoMemory( ModBase ParentMod )
			{
				Debug.WriteLine( $"Writing {this} to memory..." );
				int BytesWritten;

				try
				{
					// Find the address of the existing memory
					UpdateStatus( WriteState.FindingExistingScript );
					long ExistingAddress = Modder.MemoryModder.Instance.FindAddress( Encoding.ASCII.GetBytes( ComparisonString ) );
					Debug.WriteLine( $"ExistingAddress: {ExistingAddress.ToString( "X" )}" );
					if( ExistingAddress <= 0 )
					{
						throw new Exception( "Could not find address of existing script." );
					}

					// Convert the address of the existing memory into a byte array
					byte[] HexBytes = LongToByteArray( ExistingAddress, true );
					string HexBytesString = "";
					foreach ( byte b in HexBytes )
					{
						HexBytesString += $"{b.ToString( "X" )}, ";
					}
					Debug.WriteLine( $"Hex Bytes: {HexBytesString}" );

					// Search for the address of the pointer to the existing memory
					UpdateStatus( WriteState.FindingPointerToScript );
					long PointerAddress = Modder.MemoryModder.Instance.FindAddress( HexBytes );
					Debug.WriteLine( $"PointerAddress: {PointerAddress.ToString( "X" )}" );
					if( PointerAddress <= 0 )
					{
						throw new Exception( "Could not find pointer to existing script." );
					}

					// Get the bytes we wish to write
					string ReplacementPath = $"{ParentMod.Path}{System.IO.Path.DirectorySeparatorChar}{ReplacedCodeFile}";
					byte[] FileBytes = System.IO.File.ReadAllBytes( ReplacementPath );

					// Get the new memory address to write to
					UpdateStatus( WriteState.FindingNewMemory );
					IntPtr NewWriteAddressPtr = Modder.MemoryModder.Instance.AllocateMemory( FileBytes );
					long NewWriteAddress = NewWriteAddressPtr.ToInt64();
					Debug.WriteLine( $"{this} got write address: {NewWriteAddress.ToString( "X" )}" );
					if ( NewWriteAddress <= 0 )
					{
						throw new Exception( "Could not find memory to write new script to." );
					}

					// Write the script to the new memory
					UpdateStatus( WriteState.WritingToNewMemory );
					BytesWritten = Modder.MemoryModder.Instance.WriteMemory( NewWriteAddress, FileBytes );
					Debug.WriteLine( $"BytesWritten to new address: {BytesWritten}" );
					if ( BytesWritten <= 0 )
					{
						throw new Exception( $"Could not write script to address {NewWriteAddress.ToString( "X" )}." );
					}

					// Write the new memory pointer
					UpdateStatus( WriteState.OverwritingPointer );
					long WriteAddress = NewWriteAddress;
					byte[] WriteBytes = LongToByteArray( WriteAddress, true );
					BytesWritten = Modder.MemoryModder.Instance.WriteMemory( PointerAddress, WriteBytes );
					if( BytesWritten <= 0 )
					{
						throw new Exception( $"Could not overwrite memory pointer at {PointerAddress.ToString( "X" )}." );
					}

					// Success
					return true;

				}
				catch ( Exception e )
				{
					LastError = e.Message;
					return false;
				}
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

			private void UpdateStatus( WriteState NewState )
			{
				Progress = ((int) NewState ) / (float)( Enum.GetValues(typeof(WriteState)).Length - 1 );
				State = NewState;
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
		public List<File> Files = new List<File>();

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
