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
using Launcher.Utils;

namespace Launcher
{

	public partial class SpyglassLauncher : Form
	{
		private const float PROCESS_TIMEOUT = 30;
		private const string DEFAULT_GAME_PATH = "C:/Program Files (x86)/Origin Games/Titanfall2/Titanfall2.exe";
		private const string ORIGIN_PROCESS_NAME = "Origin";
		private const string DLL_NAME = "TitanfallSDK.dll";
		private const string DLL_FUNC_CONSOLE = "InitializeSDKConsole";
		private const string DLL_FUNC_INIT = "InitializeSDK";

		private Dictionary<int, ModJson> DisplayedMods = new Dictionary<int, ModJson>();
		private Modder.ConsoleFileWatcher ConsoleWatcher;
		private Thread InjectionThread;
		private string CurrentGamePath;
		private Injector SyringeInstance;
		private Process TTF2Process;

		public static bool DeveloperMode = true;

		public SpyglassLauncher()
		{
			InitializeComponent();

			// Redirect outputs to the in-app console
			Console.SetOut( new Modder.MultiConsoleWriter( new Modder.ConsoleControlWriter( richTextBoxConsole ), Console.Out ) );
			var ConsoleDebugListener = new Modder.ConsoleControlTraceListener( richTextBoxConsole );
			Debug.Listeners.Add( ConsoleDebugListener );

			txtGamePath.Text = DEFAULT_GAME_PATH;

			foreach ( var ModPath in Directory.GetDirectories( "Mods" ) )
			{
				if ( ModJson.ShouldLoad( ModPath ) )
				{
					int Index = listMods.Items.Add( ModPath );
					ModJson NewMod = new ModJson();
					NewMod.Load( ModPath );
					DisplayedMods.Add( Index, NewMod );
				}
			}

			// Update developer mode checkbox
			developerToolStripMenuItem.Checked = SpyglassLauncher.DeveloperMode;

			// Watch the log file path so that we can update them in the console
			ConsoleWatcher = new Modder.ConsoleFileWatcher( txtGamePath.Text );
		}

		// Injection
		private void CreateInjectionThread()
		{
 			InjectionThread = new Thread(new ThreadStart(LaunchAndInject));
			InjectionThread.IsBackground = true;
		}

		private void LaunchAndInject()
		{
			PerformLaunch(true);
		}

		private void PerformLaunch(bool performInjection = true)
		{
 			Process.Start(new ProcessStartInfo( txtGamePath.Text ) );
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
			if (InjectionThread?.ThreadState == System.Threading.ThreadState.Stopped)
			{
				CreateInjectionThread();
			}

			DateTime start = DateTime.Now;
			CurrentGamePath = txtGamePath.Text;
			string processName = Path.GetFileNameWithoutExtension(CurrentGamePath);

			btnLaunchGame.Enabled = false;
			btnLaunchGame.Text = "Waiting for process...";

			// Wait 30 seconds for Origin to launch the game
			while ((DateTime.Now - start).Seconds < PROCESS_TIMEOUT)
			{
				Process[] processes = Process.GetProcessesByName(processName);
				Process ttf2Process = processes.Length > 0 ? processes[0] : null;
				if (ttf2Process == null)
				{
					continue;
				}

				try
				{
					Process ParentProcess = ttf2Process.GetParentProcess();
					if ( ParentProcess != null && ParentProcess.ProcessName == ORIGIN_PROCESS_NAME )
					{
						Inject( ttf2Process );
					}
				}
				catch (Win32Exception e)
				{
					MessageBox.Show("Failed to inject SDK into Titanfall 2. Error Message = " + e.Message + ", Error Code = " + e.NativeErrorCode, "Failed to launch", MessageBoxButtons.OK, MessageBoxIcon.Error);
					ttf2Process?.Kill();
					return;
				}
				catch (Exception e)
				{
					MessageBox.Show("Failed to inject SDK into Titanfall 2. Error Message = " + e.Message, "Failed to launch", MessageBoxButtons.OK, MessageBoxIcon.Error);
					ttf2Process?.Kill();
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
			syringe.SetDLLSearchPath( Directory.GetCurrentDirectory() );
			syringe.InjectLibrary( DLL_NAME );
			syringe.CallExport( DLL_NAME, DLL_FUNC_CONSOLE );
			syringe.CallExport( DLL_NAME, DLL_FUNC_INIT );

			TTF2Process = targetProcess;
			SyringeInstance = syringe;
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

		private void developerMenuItem_Click( object sender, EventArgs e )
		{
			developerToolStripMenuItem.Checked = !developerToolStripMenuItem.Checked;
			SpyglassLauncher.DeveloperMode = developerToolStripMenuItem.Checked;
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

		private async void btnWriteMods_Click( object sender, EventArgs e )
		{
			Forms.WriteToMemoryProgress WriteToMemory = new Forms.WriteToMemoryProgress();
			WriteToMemory.Show();

			foreach ( var DisplayedMod in DisplayedMods )
			{
				if ( listMods.GetItemChecked( DisplayedMod.Key ) )
				{
					Debug.WriteLine( "Attempting to inject " + DisplayedMod.Value.ToString() );
					WriteToMemory.AddModProgress( DisplayedMod.Value );
					await Task.Factory.StartNew( () => DisplayedMod.Value.WriteToMemory() );
				}
			}

		}

		private async void InjectMods()
		{
			foreach( var DisplayedMod in DisplayedMods )
			{
				if( listMods.GetItemChecked( DisplayedMod.Key ) )
				{
					Debug.WriteLine( "Attempting to inject " + DisplayedMod.Value.ToString() );
					await Task.Factory.StartNew( () => DisplayedMod.Value.WriteToMemory() );
				}
			}
		}

		private void lookupGeneratorToolStripMenuItem_Click( object sender, EventArgs e )
		{
			Forms.LookupGenerator LookupGenerator = new Forms.LookupGenerator();
			LookupGenerator.Show();
		}

		private void lookupScannerToolStripMenuItem_Click( object sender, EventArgs e )
		{
			Forms.LookupScanner LookupScanner = new Forms.LookupScanner();
			LookupScanner.Show();
		}

		private void SpyglassLauncher_Load( object sender, EventArgs e )
		{
			foreach ( TabPage Page in tabControl.TabPages )
			{
				Page.Show();
			}
		}

		private void txtGamePath_TextChanged( object sender, EventArgs e )
		{
			ConsoleWatcher?.UpdateWatchPath( txtGamePath.Text );
		}

		private void spawnListGeneratorToolStripMenuItem_Click( object sender, EventArgs e )
		{
			Forms.SpawnlistGenerator Generator = new Forms.SpawnlistGenerator();
			Generator.Show();
		}

		// @todo: remove this
		private void sigScanTestToolStripMenuItem_Click( object sender, EventArgs e )
		{
			SyringeInstance.CallExport( DLL_NAME, "TestSigscan" );
		}
	}

}
