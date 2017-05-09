using Syringe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{

	[StructLayout(LayoutKind.Sequential)]
	struct SettingsStruct
	{
		[MarshalAs(UnmanagedType.I1)]
		public bool EnableConsole;
	}

}
