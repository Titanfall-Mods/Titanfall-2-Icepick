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
		struct SDKSettings
		{
			[CustomMarshalAs( CustomUnmanagedType.LPStr )] public string BasePath;
		};

		public static void Initialise()
		{
            SDKSettings Settings = new SDKSettings();
            Settings.BasePath = Application.StartupPath + "\\data\\";
			Console.WriteLine( "Path: " + Settings.BasePath);
			SpyglassLauncher.SyringeInstance?.CallExport( SpyglassLauncher.DLL_NAME, SpyglassLauncher.DLL_FUNC_INIT, Settings );
		}

	}
}
