using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Modder
{
	public static class MemoryScanner
	{

		const int PROCESS_QUERY_INFORMATION = 0x0400;
		const int MEM_COMMIT = 0x00001000;
		const int PAGE_READWRITE = 0x04;
		const int PROCESS_WM_READ = 0x0010;

		[DllImport( "kernel32.dll" )]
		public static extern bool ReadProcessMemory( int hProcess, long lpBaseAddress, byte[] lpBuffer, long dwSize, ref int lpNumberOfBytesRead );

		[DllImport( "kernel32.dll" )]
		static extern void GetSystemInfo( out SystemInfo lpSystemInfo );

		[DllImport( "kernel32.dll", SetLastError = true )]
		static extern int VirtualQueryEx( IntPtr hProcess, IntPtr lpAddress, out BasicMemoryInfo lpBuffer, uint dwLength );

		[StructLayout( LayoutKind.Sequential )]
		public struct BasicMemoryInfo
		{
			public IntPtr BaseAddress;
			public IntPtr AllocationBase;
			public uint AllocationProtect;
			public IntPtr RegionSize;
			public uint State;
			public uint Protect;
			public uint Type;
		}

		public struct SystemInfo
		{
			public ushort processorArchitecture;
			ushort reserved;
			public uint pageSize;
			public IntPtr minimumApplicationAddress;
			public IntPtr maximumApplicationAddress;
			public IntPtr activeProcessorMask;
			public uint numberOfProcessors;
			public uint processorType;
			public uint allocationGranularity;
			public ushort processorLevel;
			public ushort processorRevision;
		}

		public static long FindAddressOfData( IntPtr Handle, byte[] Data )
		{
			SystemInfo SysInfo = new SystemInfo();
			GetSystemInfo( out SysInfo );

			long ProcessMinimumAddress = SysInfo.minimumApplicationAddress.ToInt64();
			long ProcessMaximumAddress = SysInfo.maximumApplicationAddress.ToInt64();
			long ProcessCurrentAddress = ProcessMinimumAddress;
			int BytesRead = 0;
			BasicMemoryInfo MemoryInfo = new BasicMemoryInfo();

			while( ProcessCurrentAddress < ProcessMaximumAddress )
			{
				VirtualQueryEx( Handle, (IntPtr) ProcessCurrentAddress, out MemoryInfo, (uint) Marshal.SizeOf( typeof( BasicMemoryInfo ) ) );
				if(MemoryInfo.Protect == PAGE_READWRITE && MemoryInfo.State == MEM_COMMIT)
				{
					long RegionSize = MemoryInfo.RegionSize.ToInt64();
					byte[] Buffer = new byte[ RegionSize ];
					ReadProcessMemory( Handle.ToInt32(), MemoryInfo.BaseAddress.ToInt64(), Buffer, RegionSize, ref BytesRead );

					long Index = BoyerMooreSearch( Buffer, Data );
					if(Index > 0)
					{
						return ProcessCurrentAddress + Index;
					}
				}

				ProcessCurrentAddress += MemoryInfo.RegionSize.ToInt64();
			}

			return -1;
		}

		private static long BoyerMooreSearch( byte[] Haystack, byte[] Needle )
		{
			unchecked
			{
				int[] Lookup = new int[ 256 ];
				for( int i = 0; i < Lookup.Length; ++i )
				{
					Lookup[ i ] = Needle.Length;
				}
				for( int i = 0; i < Needle.Length; ++i )
				{
					Lookup[ Needle[ i ] ] = Needle.Length - i - 1;
				}

				long Index = Needle.Length - 1;
				byte LastByte = Needle.Last();
				while( Index < Haystack.Length )
				{
					byte CheckByte = Haystack[ Index ];
					if( Haystack[Index] == LastByte )
					{
						bool Found = true;
						for( int k = Needle.Length - 2; k >= 0; --k )
						{
							if ( Haystack[ Index - Needle.Length + k + 1] != Needle[k])
							{
								Found = false;
								break;
							}
						}

						if ( Found )
						{
							return Index - Needle.Length + 1;
						}
						else
						{
							++Index;
						}
					}
					else
					{
						Index += Lookup[ CheckByte ];
					}
				}

			}
			return -1;
		}

	}
}
