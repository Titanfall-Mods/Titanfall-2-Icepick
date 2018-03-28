using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Syringe;

namespace Launcher.Modder
{
	public class SDKInterface
	{

		[StructLayout( LayoutKind.Sequential, Pack = 8 )]
		struct OverwrittenFile
		{
			[CustomMarshalAs( CustomUnmanagedType.LPStr )] public string Chunk;
			[CustomMarshalAs( CustomUnmanagedType.LPStr )] public string Contents;
			public int Length;

			public OverwrittenFile( string InChunk )
			{
				Chunk = InChunk;
				Contents = string.Empty;
				Length = 0;
			}

			public void SetContents( string InContents )
			{
				Contents = InContents;
				Length = Contents.Length;
			}
		};

		public static void AddClientFile( string ChunkName, string FileContents )
		{
			OverwrittenFile File = new OverwrittenFile( ChunkName );
			File.SetContents( FileContents );
			SpyglassLauncher.SyringeInstance.CallExport( SpyglassLauncher.DLL_NAME, "AddClientFile", File );
		}

		public static void AddServerFile( string ChunkName, string FileContents )
		{
			OverwrittenFile File = new OverwrittenFile( ChunkName );
			File.SetContents( FileContents );
			SpyglassLauncher.SyringeInstance.CallExport( SpyglassLauncher.DLL_NAME, "AddServerFile", File );
		}

	}
}
