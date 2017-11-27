using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syringe;
using Syringe.Win32;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

// #define SQUIRREL_TESTING

namespace Launcher
{

	public partial class SpyglassLauncher : Form
	{

		private Thread injectionThread;
		private string gamePath;

		private const float PROCESS_TIMEOUT = 30;
#if SQUIRREL_TESTING
		private const string DEFAULT_GAME_PATH = "F:/Projects/Git Repos/SquirrelTest/x64/Debug/SquirrelTest.exe"; // "C:/Program Files (x86)/Origin Games/Titanfall2/Titanfall2.exe";
		private const string ORIGIN_LAUNCH_TTF_COMMAND = @"F:\Projects\Git Repos\SquirrelTest\x64\Debug\SquirrelTest.exe"; //"origin://LaunchGame/Origin.OFR.50.0001464";
#else
		private const string DEFAULT_GAME_PATH = "C:/Program Files (x86)/Origin Games/Titanfall2/Titanfall2.exe";
		private const string ORIGIN_LAUNCH_TTF_COMMAND = "origin://LaunchGame/Origin.OFR.50.0001464";
#endif
		private const string DLL_NAME = "TitanfallSDK.dll";
		private const string DLL_FUNC_CONSOLE = "InitializeSDKConsole";
		private const string DLL_FUNC_INIT = "InitializeSDK";

		private Dictionary<int, ModJson> DisplayedMods = new Dictionary<int, ModJson>();

		public SpyglassLauncher()
		{
			InitializeComponent();

			txtGamePath.Text = DEFAULT_GAME_PATH;

			foreach ( var ModPath in Directory.GetDirectories( "Mods" ) )
			{
				int Index = listMods.Items.Add( ModPath );
				ModJson NewMod = new ModJson();
				NewMod.Load( ModPath );
				DisplayedMods.Add( Index, NewMod );
			}
		}
		
		// Injection
		private void CreateInjectionThread()
		{
 			injectionThread = new Thread(new ThreadStart(LaunchAndInject));
			injectionThread.IsBackground = true;
		}

		private void LaunchAndInject()
		{
			PerformLaunch(true);
		}

		private void PerformLaunch(bool performInjection = true)
		{
 			Process.Start(new ProcessStartInfo(ORIGIN_LAUNCH_TTF_COMMAND));
			if (performInjection)
			{
				btnLaunchGame.Enabled = false;
				btnLaunchGame.Text = "Waiting for injection...";
				PerformInjection();
			}
		}

		private void PerformInjection()
		{
			// Create the injection thread if it doesn't exist
			if (injectionThread?.ThreadState == System.Threading.ThreadState.Stopped)
			{
				CreateInjectionThread();
			}

			DateTime start = DateTime.Now;
			gamePath = txtGamePath.Text;
			string processName = Path.GetFileNameWithoutExtension(gamePath);

			btnLaunchGame.Enabled = false;
			btnLaunchGame.Text = "Waiting for process...";

			// Wait 30 seconds for Origin to launch the game
			while ((DateTime.Now - start).Seconds < PROCESS_TIMEOUT)
			{
				Process[] procsses = Process.GetProcessesByName(processName);
				Process ttf2Process = procsses.Length > 0 ? procsses[0] : null;
				if (ttf2Process == null)
				{
					continue;
				}

				try
				{
					Inject(ttf2Process);
				}
				catch (Win32Exception e)
				{
					MessageBox.Show("Failed to inject SDK into Titanfall 2. Error Message = " + e.Message + ", Error Code = " + e.NativeErrorCode, "Failed to launch", MessageBoxButtons.OK, MessageBoxIcon.Error);
					ttf2Process.Kill();
					return;
				}
				catch (Exception e)
				{
					MessageBox.Show("Failed to inject SDK into Titanfall 2. Error Message = " + e.Message, "Failed to launch", MessageBoxButtons.OK, MessageBoxIcon.Error);
					ttf2Process.Kill();
					return;
				}
				finally
				{
					ResetLaunchButton();
				}
				return;
			}

			MessageBox.Show($"Failed to find game process after {PROCESS_TIMEOUT} seconds.", "Failed to launch", MessageBoxButtons.OK, MessageBoxIcon.Error);
			ResetLaunchButton();
		}

		private void Inject(Process targetProcess)
		{
			Injector syringe = new Injector(targetProcess);
			syringe.SetDLLSearchPath(Directory.GetCurrentDirectory());
			syringe.InjectLibrary(DLL_NAME);
			syringe.CallExport(DLL_NAME, DLL_FUNC_CONSOLE);
			syringe.CallExport(DLL_NAME, DLL_FUNC_INIT);
		}

		// Actions
		private void btnBrowseGamePath_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = txtGamePath.Text;
			openFileDialog.Filter = "Titanfall 2 (*.exe)|*.exe";
			DialogResult result = openFileDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				txtGamePath.Text = openFileDialog.FileName;
			}
		}

		private void optionMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;
			item.Checked = !item.Checked;
		}

		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void btnLaunchGame_Click(object sender, EventArgs e)
		{
			LaunchAndInject();
		}

		private void ResetLaunchButton()
		{
			btnLaunchGame.Invoke(new MethodInvoker(delegate{
					btnLaunchGame.Text = "Launch Game";
					btnLaunchGame.Enabled = true;
			}));
		}

		private void btnWriteMods_Click( object sender, EventArgs e )
		{
			InjectMods();
		}

		private async void InjectMods()
		{
			foreach( var DisplayedMod in DisplayedMods )
			{
				if( listMods.GetItemChecked( DisplayedMod.Key ) )
				{
					Debug.WriteLine( "Attempting to inject " + DisplayedMod.Value.ToString() );

					Debug.WriteLine( $"modder: {Modder.MemoryModder.Instance}" );
					DisplayedMod.Value.WriteToMemory();
				}
			}
		}
	}

}
