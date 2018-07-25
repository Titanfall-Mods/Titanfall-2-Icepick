using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Icepick.Mods
{
	public class TitanfallMod
	{
		public const string ModDocumentFile = "mod.json";
		const string ModImage = "mod.png";
		const string ReleaseFile = "release.txt";
		const string IcepickApiId = "icepick";

		public TitanfallModDefinition Definition;
		public string CurrentReleaseId;
		public string ImagePath;
		public string Directory;
		public bool RequiresUpdate { get; private set; }

		public delegate void ModStateUpdatedDelegate();
		public event ModStateUpdatedDelegate OnStatusUpdated;

		public TitanfallMod( string Directory )
		{
			string modDocumentPath = Path.Combine( Directory, ModDocumentFile );
			if ( File.Exists( modDocumentPath ) )
			{
				StreamReader reader = new StreamReader( modDocumentPath );
				string Contents = reader.ReadToEnd();
				reader.Close();
				reader.Dispose();

				Definition = JsonConvert.DeserializeObject<TitanfallModDefinition>( Contents );
			}
			this.Directory = Directory;

			// Load custom image for this mod if it exists
			string modImagePath = Path.Combine( Directory, ModImage );
			if ( File.Exists( modImagePath ) )
			{
				ImagePath = modImagePath;
			}

			// Get the release id of the mod so we can check it for updates
			string modReleasePath = Path.Combine( Directory, ReleaseFile );
			if ( File.Exists( modReleasePath ) )
			{
				StreamReader reader = new StreamReader( modReleasePath );
				CurrentReleaseId = reader.ReadToEnd();
				reader.Close();
				reader.Dispose();
			}

			if ( OnStatusUpdated != null )
			{
				OnStatusUpdated();
			}
		}

		public bool IsIcepickFramework
		{
			get
			{
				return Definition != null && !string.IsNullOrWhiteSpace( Definition.ApiId ) && Definition.ApiId == IcepickApiId;
			}
		}

		public async void CheckForUpdates()
		{
			if ( Definition != null && !string.IsNullOrWhiteSpace( Definition.ApiId ) )
			{
				if ( IsIcepickFramework )
				{
					Api.ApiQueue.ApiRequest( "IcepickInfo" );
				}
				else
				{ 
					Task<Api.ApiResult> requestTask = Api.ApiQueue.ApiRequest( "LatestModRelease", Definition.ApiId );
					await requestTask;

					Api.ApiResult results = requestTask.Result;
					if ( results != null && results.data != null )
					{
						string latestReleaseId = null;
						results.data.TryGetValue( "_id", out latestReleaseId );
						RequiresUpdate = latestReleaseId != null && latestReleaseId != CurrentReleaseId;
						if ( OnStatusUpdated != null )
						{
							OnStatusUpdated();
						}
					}
				}
			}
		}

		public void OpenDownloadPage()
		{
			if( IsIcepickFramework )
			{
				Process.Start( Api.ApiRoutes.GetRoute( "DownloadIcepick" ) );
			}
			else if ( Definition != null && !string.IsNullOrWhiteSpace( Definition.ApiId ) )
			{
				Process.Start( Api.ApiRoutes.GetRoute( "ViewMod", Definition.ApiId ) );
			}
		}

		public List<string> GetWarnings()
		{
			List<string> warnings = new List<string>();

			if ( Definition != null )
			{
				if ( string.IsNullOrWhiteSpace( Definition.ApiId ) )
				{
					warnings.Add( "This mod is missing an ApiId and will not check for updates." );
				}
				if ( string.IsNullOrWhiteSpace( Definition.Description ) )
				{
					warnings.Add( "This mod has no description." );
				}
				if ( Definition.Authors.Count < 1 )
				{
					warnings.Add( "This mod has no contact information." );
				}
				if ( Definition.Contacts.Count < 1 )
				{
					warnings.Add( "This mod has no contact information." );
				}
			}

			return warnings;
		}

		public List<string> GetErrors()
		{
			List<string> errors = new List<string>();

			if ( Definition == null )
			{
				errors.Add( "This mod has a missing, malformed, or corrupt definition file." );
			}
			else
			{
				if( string.IsNullOrWhiteSpace( Definition.Name ) )
				{
					errors.Add( "This mod has no name." );
				}
			}
			return errors;
		}

		public void IcepickUpdateAvailable()
		{
			RequiresUpdate = true;
			if ( OnStatusUpdated != null )
			{
				OnStatusUpdated();
			}
		}

	}
}
