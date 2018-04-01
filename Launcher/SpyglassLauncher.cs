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
using Launcher.ModDocuments;
using Launcher.Modder;

namespace Launcher
{

	public partial class SpyglassLauncher : Form
	{
		private const float PROCESS_TIMEOUT = 10;
		private const string DEFAULT_GAME_PATH = "C:/Program Files (x86)/Origin Games/Titanfall2/Titanfall2.exe";
		private const string ORIGIN_PROCESS_NAME = "Origin";
		public const string DLL_NAME = "TTF2SDK.dll";
		public const string DLL_FUNC_CONSOLE = "InitializeSDKConsole";
		public const string DLL_FUNC_INIT = "InitializeSDK";
		public const string MODS_DIRECTORY = "Mods";

		private Modder.ConsoleFileWatcher ConsoleWatcher;
		private Thread InjectionThread;
		private string CurrentGamePath;
		private Process TTF2Process;

		private List<FileSystemWatcher> ModJsonWatchers = new List<FileSystemWatcher>();

		public static Injector SyringeInstance;
		public static bool DeveloperMode = true;
		public static List<ModBase> DisplayedMods = new List<ModBase>();

		public SpyglassLauncher()
		{
			InitializeComponent();

			// Redirect outputs to the in-app console
// 			Console.SetOut( new Modder.MultiConsoleWriter( new Modder.ConsoleControlWriter( richTextBoxConsole ), Console.Out ) );
// 			var ConsoleDebugListener = new Modder.ConsoleControlTraceListener( richTextBoxConsole );
// 			Debug.Listeners.Add( ConsoleDebugListener );

			txtGamePath.Text = DEFAULT_GAME_PATH;

			foreach ( var ModPath in Directory.GetDirectories( MODS_DIRECTORY ) )
			{
				// Load the json mod
				LoadModFromDirectory( ModPath );

				// Watch the json for changes, watch outside of the mod so we can reload it immediately if there were json errors
				FileSystemWatcher JsonWatcher = new FileSystemWatcher();
				JsonWatcher.Path = ModPath;
				JsonWatcher.NotifyFilter = NotifyFilters.LastWrite;
				JsonWatcher.Filter = "*.json";
				JsonWatcher.Changed += JsonModFileUpdated;
				JsonWatcher.EnableRaisingEvents = true;
				ModJsonWatchers.Add( JsonWatcher );
			}

			// Update developer mode checkbox
			developerToolStripMenuItem.Checked = SpyglassLauncher.DeveloperMode;
		}

		protected void LoadModFromDirectory( string Directory, bool UseInvoke = false )
		{
			try
			{
				if ( ModJson.ShouldLoad( Directory ) )
				{
					if ( UseInvoke )
					{
						Invoke( new MethodInvoker( delegate
						{
							LoadModFromDirectory_Internal( Directory );
						} ) );
					}
					else
					{
						LoadModFromDirectory_Internal( Directory );
					}
				}
			}
			catch ( Exception e )
			{
				Console.WriteLine( $"[Error] {e.Message}" );
			}
		}

		protected void LoadModFromDirectory_Internal( string Directory )
		{
			int Index = listMods.Items.Add( Directory );
			ModJson NewMod = new ModJson();
			NewMod.Load( Directory );
			DisplayedMods.Add( NewMod );
		}

		private void JsonModFileUpdated( object sender, FileSystemEventArgs e )
		{
			string ModPath = Path.GetDirectoryName( e.FullPath );
			Console.WriteLine( $"{ModPath} updated, reloading mod..." );
			for( int i = 0; i < DisplayedMods.Count; ++i )
			{
				ModBase Mod = DisplayedMods[i];
				if ( ModPath == Mod.ModPath )
				{
					bool bWasChecked = listMods.GetItemChecked( i );

					// Cleanup previous watchers
					Mod.CleanupFileWatchers();

					// Remove from the mods list
					Invoke( new MethodInvoker( delegate
					{
						listMods.Items.RemoveAt( i );
					} ) );
					DisplayedMods.RemoveAt( i );

					// Reload the mod
					LoadModFromDirectory( ModPath, UseInvoke: true );

					// Recheck the mod entry if it was already checked
					Invoke( new MethodInvoker( delegate
					{
						listMods.SetItemChecked( listMods.Items.Count - 1, bWasChecked );
					} ) );

					break;
				}
			}
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
				return;
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
					Console.WriteLine( "Failed to inject SDK into Titanfall 2. Error Message = " + e.Message + ", Error Code = " + e.NativeErrorCode, "Failed to launch", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				catch (Exception e)
				{
					Console.WriteLine("Failed to inject SDK into Titanfall 2. Error Message = " + e.Message, "Failed to launch", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				finally
				{
					ResetLaunchButton();
				}

				// Try again every second
				Thread.Sleep( 250 );
			}

// 			MessageBox.Show($"Failed to find game process after {PROCESS_TIMEOUT} seconds.", "Failed to launch", MessageBoxButtons.OK, MessageBoxIcon.Error);
			ResetLaunchButton();
		}

		private void Inject(Process targetProcess)
		{
			Injector syringe = new Injector(targetProcess);
			syringe.SetDLLSearchPath( Directory.GetCurrentDirectory() );
			syringe.InjectLibrary( DLL_NAME );

			TTF2Process = targetProcess;
			SyringeInstance = syringe;

			SDKInterface.SetReplacementsPath();
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

		private void btnWriteMods_Click( object sender, EventArgs e )
		{
			ModsCompiler.CompileAllMods();
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

		private void OnModCheckedStateChanged( object sender, ItemCheckEventArgs e )
		{
			ModBase Mod = DisplayedMods[ e.Index ];
			Mod.Active = e.NewValue == CheckState.Checked;
		}
	}

}
