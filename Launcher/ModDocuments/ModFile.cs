using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Launcher.ModDocuments
{
	public class ModFile
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

		public string id;
		public string ComparisonString;
		public string ReplacedCodeFile;
		public long AddressOffset;
		public string ChunkName;
		public string Context;

		public float Progress;
		public bool? Success;
		public WriteState State;
		public string LastError = string.Empty;

		private string Contents;
		private ModBase ParentMod;

		protected const string PROCESSED_FILES_DIR = ".processed";
		protected const string PREPROCESSOR_REGEX = @"//.*#([a-zA-Z]+) (.*)"; // Preprocessor format: // #include test.nut
		protected const int PREPROCESSOR_CAPTURES = 2;

		// Preprocessor handlers return true if they are a valid handler, and should require processing the file again after they've been run
		protected delegate bool ModFilePreprocessorHandler( ModFile TargetFile, int LineIdx, string Data );
		protected Dictionary<string, ModFilePreprocessorHandler> ModFilePreprocessors = new Dictionary<string, ModFilePreprocessorHandler>()
		{
			{ "include", IncludePreprocessor },
			{ "includefolder", IncludeFolderPreprocessor },
			{ "if", NullPreprocessor },
		};

		public void ReadFileContents()
		{
			string ReplacementPath = $"{ParentMod.Path}{Path.DirectorySeparatorChar}{ReplacedCodeFile}";
			Contents = File.ReadAllText( ReplacementPath );
		}

		public void SendToSDK( ModBase ParentMod )
		{
			this.ParentMod = ParentMod;
			ReadAndProcessContents();

			if ( Contents.Length > 0 )
			{
				if ( Context == "sv" )
				{
					Modder.SDKInterface.AddServerFile( this.ChunkName, Contents );
				}
				else
				{
					Modder.SDKInterface.AddClientFile( this.ChunkName, Contents );
				}
			}
			else
			{
				string ReplacementPath = $"{ParentMod.Path}{Path.DirectorySeparatorChar}{ReplacedCodeFile}";
				Console.WriteLine( $"Error: Failed to write file {ReplacementPath} to SDK!" );
			}
		}

		private void ReadAndProcessContents()
		{
			// @todo: Read base file, append files, process for #includes

			// Get the file contents we wish to write
			string ReplacementPath = $"{ParentMod.Path}{Path.DirectorySeparatorChar}{ReplacedCodeFile}";
			Contents = File.ReadAllText( ReplacementPath );

			// Append files
			foreach ( ModBase.AppendedFile AppendFile in ParentMod.AppendedFiles )
			{
				if ( AppendFile.AppendTo == this.id )
				{
					string AppendString = File.ReadAllText( $"{ParentMod.Path}{Path.DirectorySeparatorChar}{AppendFile.ScriptPath}" );
					Contents += AppendString;
				}
			}

			// Process each line looking for custom preprocessors
			bool PreprocessingFinished = false;
			int i = 0;
			while ( !PreprocessingFinished )
			{
				PreprocessingFinished = PreprocessContents();
				i++;
				if ( i > 16 ) break;
			}

		}

		private bool PreprocessContents()
		{
			using ( StringReader Reader = new StringReader( Contents ) )
			{
				int LineIdx = 0;
				string Line;
				while ( ( Line = Reader.ReadLine() ) != null )
				{
					Line = Line.Trim();
					Match PreprocessorMatch = Regex.Match( Line, PREPROCESSOR_REGEX );
					if ( PreprocessorMatch.Success )
					{
						if ( PreprocessorMatch.Groups.Count > PREPROCESSOR_CAPTURES )
						{
							Group ProcessorNameGroup = PreprocessorMatch.Groups[ 1 ];
							Group ProcessorDataGroup = PreprocessorMatch.Groups[ 2 ];

							if ( ModFilePreprocessors.ContainsKey( ProcessorNameGroup.Value ) )
							{
								bool IsValidPreprocessor = ModFilePreprocessors[ ProcessorNameGroup.Value ]( this, LineIdx, ProcessorDataGroup.Value );
								if ( IsValidPreprocessor )
								{
									// Found a preprocessor, so iterate over file contents again to support nested ones
									return false;
								}
							}
							else
							{
								Debug.WriteLine( "[Warning] Found a custom preprocessor that is not handled! " + Line );
							}
						}
					}

					++LineIdx;
				}
			}

			return true;
		}

		private void SaveProcessedFile()
		{
			if ( SpyglassLauncher.DeveloperMode )
			{
				string ProcessedFilesDirectory = $"{ParentMod.Path}{Path.DirectorySeparatorChar}{PROCESSED_FILES_DIR}{Path.DirectorySeparatorChar}{Path.GetDirectoryName( ReplacedCodeFile )}";
				if ( !Directory.Exists( ProcessedFilesDirectory ) )
				{
					Directory.CreateDirectory( ProcessedFilesDirectory );
				}

				string ProcessedFile = $"{ParentMod.Path}{Path.DirectorySeparatorChar}{PROCESSED_FILES_DIR}{Path.DirectorySeparatorChar}{ReplacedCodeFile}"; ;
				File.WriteAllText( ProcessedFile, Contents );
			}
		}

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
			this.ParentMod = ParentMod;

// 			try
			{
				// Read the files we need to 
				ReadAndProcessContents();

				// Write processed file out if in developer mode
				SaveProcessedFile();

				// Convert contents to bytes for writing to memory
				byte[] ContentBytes = Encoding.ASCII.GetBytes( Contents );

				// Find the address of the existing memory
				UpdateStatus( WriteState.FindingExistingScript );
				long ExistingAddress = Modder.MemoryModder.Instance.FindAddress( Encoding.ASCII.GetBytes( ComparisonString ) );
				Debug.WriteLine( $"{this} ExistingAddress: {ExistingAddress.ToString( "X" )}" );
				if ( ExistingAddress <= 0 )
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
				Debug.WriteLine( $"{this} PointerAddress: {PointerAddress.ToString( "X" )}" );
				if ( PointerAddress <= 0 )
				{
					throw new Exception( "Could not find pointer to existing script." );
				}

				// Get the new memory address to write to
				UpdateStatus( WriteState.FindingNewMemory );
				IntPtr NewWriteAddressPtr = Modder.MemoryModder.Instance.AllocateMemory( ContentBytes );
				long NewWriteAddress = NewWriteAddressPtr.ToInt64();
				Debug.WriteLine( $"{this} got write address: {NewWriteAddress.ToString( "X" )}" );
				if ( NewWriteAddress <= 0 )
				{
					throw new Exception( "Could not find memory to write new script to." );
				}

				// Write the script to the new memory
				UpdateStatus( WriteState.WritingToNewMemory );
				BytesWritten = Modder.MemoryModder.Instance.WriteMemory( NewWriteAddress, ContentBytes );
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
				if ( BytesWritten <= 0 )
				{
					throw new Exception( $"Could not overwrite memory pointer at {PointerAddress.ToString( "X" )}." );
				}

				// Success
				return true;

			}
// 			catch ( Exception e )
// 			{
// 				LastError = e.Message;
// 				return false;
// 			}
		}

		public byte[] LongToByteArray( long Input, bool IsLittleEndian = false )
		{
			string HexString = Input.ToString( "X" );
			if ( HexString.Length % 2 != 0 )
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
			Progress = ( (int) NewState ) / (float) ( Enum.GetValues( typeof( WriteState ) ).Length - 1 );
			State = NewState;
		}

		public override string ToString()
		{
			return $"[ModFile {ReplacedCodeFile} (@{AddressOffset})]";
		}

		private static void ReplaceContentsLineWithFileContents( ModFile TargetFile, int LineIdx, string FilePath, bool InsertInsteadOfReplace = false )
		{
			if ( !File.Exists( FilePath ) )
			{
				Debug.WriteLine( $"[Error] Could not include file as it does not exist: {FilePath}!" );
				return;
			}

			// Get contents of the included file
			string IncludeContents = File.ReadAllText( FilePath );

			// Get lines for the original file
			string[] ContentLines = TargetFile.Contents.Split( new string[] { Environment.NewLine }, int.MaxValue, StringSplitOptions.None );

			// Replace the preprocessor
			if ( InsertInsteadOfReplace )
			{
				var ContentLinesList = ContentLines.ToList();
				ContentLinesList.Insert( LineIdx + 1, IncludeContents );
				ContentLines = ContentLinesList.ToArray();
			}
			else
			{
				ContentLines[ LineIdx ] = IncludeContents;
			}

			// Recompile content lines
			StringBuilder Builder = new StringBuilder();
			foreach ( string Line in ContentLines )
			{
				Builder.Append( Line );
				Builder.Append( Environment.NewLine );
			}
			TargetFile.Contents = Builder.ToString();
		}

		private static bool IncludePreprocessor( ModFile TargetFile, int LineIdx, string Data )
		{
			Debug.WriteLine( $"Processing #include on {TargetFile} at line {LineIdx + 1}: {Data}" );

			string FilePath = $"{TargetFile.ParentMod.Path}{Path.DirectorySeparatorChar}{Data}";
			ReplaceContentsLineWithFileContents( TargetFile, LineIdx, FilePath );

			return true;
		}

		private static bool IncludeFolderPreprocessor( ModFile TargetFile, int LineIdx, string Data )
		{
			Debug.WriteLine( $"Processing #includefolder on {TargetFile} at line {LineIdx + 1}: {Data}" );

			// Split and parse the input data to allow for wildcard includes
			string FolderPath = $"{TargetFile.ParentMod.Path}{Path.DirectorySeparatorChar}";
			string FilePattern = "*";
			if ( Data.Contains( '/' ) && !Data.EndsWith( "/" ) )
			{
				string[] Folders = Data.Split( '/' );
				for ( int i = 0; i < Folders.Length - 1; ++i )
				{
					FolderPath += Folders[ i ] + Path.DirectorySeparatorChar;
				}
				FilePattern = Folders[ Folders.Length - 1 ];
			}
			else
			{
				FolderPath += Data;
			}

			// Add each file in the folder to the file
			string[] Files = Directory.GetFiles( FolderPath, FilePattern );
			for ( int i = Files.Length - 1; i >= 0; --i )
			{
				ReplaceContentsLineWithFileContents( TargetFile, LineIdx, Files[ i ], i > 0 );
			}

			return true;
		}

		private static bool NullPreprocessor( ModFile TargetFile, int LineIdx, string Data )
		{
			// Preprocessor doesn't do anything, only to stop warnings
			return false;
		}
	}

}
