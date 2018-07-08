using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Icepick.Extensions
{
	public static class ProcessExtensions
	{
		[StructLayout( LayoutKind.Sequential )]
		public struct ParentProcessData
		{
			internal IntPtr ExitStatus;
			internal IntPtr PebBaseAddress;
			internal IntPtr AffinityMask;
			internal IntPtr BasePriority;
			internal UIntPtr UniqueProcessId;
			internal IntPtr InheritedFromUniqueProcessId;

			[DllImport( "ntdll.dll" )]
			private static extern int NtQueryInformationProcess( IntPtr processHandle, int processInformationClass, ref ParentProcessData processInformation, int processInformationLength, out int returnLength );

			public static Process GetParentProcessOf( Process ChildProcess )
			{
				return GetParentProcess( ChildProcess.Handle );
			}

			public static Process GetParentProcess( IntPtr Handle )
			{
				ParentProcessData ParentData = new ParentProcessData();
				int ReturnLength;
				int Status = NtQueryInformationProcess( Handle, 0, ref ParentData, Marshal.SizeOf( ParentData ), out ReturnLength );
				if ( Status != 0 )
				{
					throw new Win32Exception( Status );
				}

				try
				{
					return Process.GetProcessById( ParentData.InheritedFromUniqueProcessId.ToInt32() );
				}
				catch ( ArgumentException )
				{
					return null;
				}
			}
		}

		public static Process GetParentProcess( this Process ChildProcess )
		{
			return ParentProcessData.GetParentProcessOf( ChildProcess );
		}

	}
}
