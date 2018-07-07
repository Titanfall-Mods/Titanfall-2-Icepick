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
		const string MOD_DOCUMENT = "mod.json";
		const string MOD_IMAGE = "mod.png";
		const string RELEASE_FILE = "release.txt";

		public TitanfallModDefinition Definition;
		public string CurrentReleaseId;
		public string ImagePath;
		public string Directory;
		public bool RequiresUpdate { get; private set; }

		public delegate void ModStateUpdatedDelegate();
		public event ModStateUpdatedDelegate OnStatusUpdated;

		public TitanfallMod( string Directory )
		{
			string modDocumentPath = Path.Combine( Directory, MOD_DOCUMENT );
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
			string modImagePath = Path.Combine( Directory, MOD_IMAGE );
			if ( File.Exists( modImagePath ) )
			{
				ImagePath = modImagePath;
			}

			// Get the release id of the mod so we can check it for updates
			string modReleasePath = Path.Combine( Directory, RELEASE_FILE );
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

		public async void CheckForUpdates()
		{
			if ( Definition != null && !string.IsNullOrWhiteSpace( Definition.ApiId ) )
			{
				Task<Api.ApiResult> requestTask = Api.ApiQueue.ApiRequest( "LatestModRelease", Definition.ApiId );
				await requestTask;

				Api.ApiResult results = requestTask.Result;
				if( results != null && results.data != null )
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

		public void OpenDownloadPage()
		{
			if ( Definition != null && !string.IsNullOrWhiteSpace( Definition.ApiId ) )
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

	}
}
