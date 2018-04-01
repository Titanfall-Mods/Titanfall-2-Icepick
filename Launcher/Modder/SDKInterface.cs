using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syringe;

namespace Launcher.Modder
{
	public class SDKInterface
	{

		[StructLayout( LayoutKind.Sequential, Pack = 8 )]
		struct SDKInfo
		{
			[CustomMarshalAs( CustomUnmanagedType.LPStr )] public string Path;
		};

		public static void SetReplacementsPath()
		{
			SDKInfo Info;
			Info.Path = Application.StartupPath + "/.processed/";
			Console.WriteLine( "Path: " + Info.Path );
			SpyglassLauncher.SyringeInstance?.CallExport( SpyglassLauncher.DLL_NAME, "SetReplacementsPath", Info );
		}

	}
}
