using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net.Http;

namespace Icepick.CrashReporting
{
	public static class CrashReporter
	{
		private const string SentryEndpoint = "https://sentry.io/api/1243227/minidump/?sentry_key=279bea5da66a42f1b653968d55d59db5";

		public delegate void DumpProcessedDelegate(bool success, string name);
		public static event DumpProcessedDelegate OnCrashDumpProcessed;

		private static FileSystemWatcher _watcher;

		public static void StartWatching()
		{
			string crashDumpsPath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "data", "crash_dumps" );
			if( !Directory.Exists( crashDumpsPath ) )
			{
				Directory.CreateDirectory( crashDumpsPath );
			}

			_watcher = new FileSystemWatcher();
			_watcher.Path = crashDumpsPath;
			_watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
			_watcher.Filter = "*.dmp";
			_watcher.Created += new FileSystemEventHandler(OnDumpCreated);
			_watcher.EnableRaisingEvents = true;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private class MEMORYSTATUSEX
		{
			public uint dwLength;
			public uint dwMemoryLoad;
			public ulong ullTotalPhys;
			public ulong ullAvailPhys;
			public ulong ullTotalPageFile;
			public ulong ullAvailPageFile;
			public ulong ullTotalVirtual;
			public ulong ullAvailVirtual;
			public ulong ullAvailExtendedVirtual;
			public MEMORYSTATUSEX()
			{
				this.dwLength = (uint)Marshal.SizeOf(this);
			}
		}

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

		private static bool SendCrashDump(string path, string name)
		{
			// Get machine UUID or generate one if not valid
			string uuidString = Api.IcepickRegistry.ReadMachineUUID();
			Guid uuid;
			if (!Guid.TryParse(uuidString, out uuid))
			{
				uuid = Guid.NewGuid();
				uuidString = uuid.ToString();
				Api.IcepickRegistry.WriteMachineUUID(uuidString);
			}

			// Attempt to get version from TTF2SDK.dll
			string versionString = "unknown";
			try
			{
				FileVersionInfo sdkVersionInfo = FileVersionInfo.GetVersionInfo(Mods.SDKInjector.SDKDllName);
				if (sdkVersionInfo.Comments != "")
				{
					versionString = sdkVersionInfo.Comments;
				}
			}
			catch (Exception) { }

			// Get memory information
			ulong totalMemory = 0;
			ulong freeMemory = 0;
			MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
			if (GlobalMemoryStatusEx(memStatus))
			{
				totalMemory = memStatus.ullTotalPhys;
				freeMemory = memStatus.ullAvailPhys;
			}

			// Get disk space information
			string rootPath = Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory);
			ulong freeStorage = 0;
			ulong storageSize = 0;
			ulong dummy;
			GetDiskFreeSpaceEx(rootPath, out freeStorage, out storageSize, out dummy);

			// Get a handle on the file
			FileStream stream = null;
			for (int i = 0; i < 10; i++)
			{
				try
				{
					stream = new FileStream(path, FileMode.Open, FileAccess.Read);
					break;
				}
				catch (IOException)
				{
					System.Threading.Thread.Sleep(500);
				}
			}

			if (stream == null)
			{
				return false;
			}

			// Send request to sentry
			HttpContent memorySizeParam = new StringContent(totalMemory.ToString());
			HttpContent freeMemoryParam = new StringContent(freeMemory.ToString());
			HttpContent storageSizeParam = new StringContent(storageSize.ToString());
			HttpContent freeStorageParam = new StringContent(freeStorage.ToString());
			HttpContent sdkNameParam = new StringContent("Icepick");
			HttpContent sdkVersionParam = new StringContent(Version.Current);
			HttpContent releaseParam = new StringContent(versionString);
			HttpContent userIdParam = new StringContent(uuidString);
			HttpContent fileStreamContent = new StreamContent(stream);

			using (var client = new HttpClient())
			{
				using (var formData = new MultipartFormDataContent())
				{
					formData.Add(memorySizeParam, "sentry[contexts][device][memory_size]");
					formData.Add(freeMemoryParam, "sentry[contexts][device][free_memory]");
					formData.Add(storageSizeParam, "sentry[contexts][device][storage_size]");
					formData.Add(freeStorageParam, "sentry[contexts][device][free_storage]");
					formData.Add(sdkNameParam, "sentry[sdk][name]");
					formData.Add(sdkVersionParam, "sentry[sdk][version]");
					formData.Add(releaseParam, "sentry[release]");
					formData.Add(userIdParam, "sentry[user][id]");
					formData.Add(fileStreamContent, "upload_file_minidump", name);

					var task = client.PostAsync(SentryEndpoint, formData);
					task.Wait();
					return task.Result.StatusCode == System.Net.HttpStatusCode.OK;
				}
			}
		}

		private static void OnDumpCreated(object source, FileSystemEventArgs e)
		{
			if (Api.IcepickRegistry.ReadDisableCrashReports())
			{
				return;
			}

			bool crashDumpSent = false;
			try
			{
				crashDumpSent = SendCrashDump(e.FullPath, e.Name);
			}
			catch (Exception)
			{
				crashDumpSent = false;
			}

			OnCrashDumpProcessed?.Invoke(crashDumpSent, e.Name);
		}
	}
}
