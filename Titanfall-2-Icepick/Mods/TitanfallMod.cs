using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Icepick.Mods
{
	public class TitanfallMod
	{
		public const string ModDocumentFile = "mod.json";
		const string ModImage = "mod.png";
		const string IcepickModName = "Icepick Framework";

		public TitanfallModDefinition Definition;
		public string ImagePath;
		public string Directory;

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
		}

		public bool IsIcepickFramework
		{
			get
			{
				return Definition != null && !string.IsNullOrWhiteSpace( Definition.Name ) && Definition.Name == IcepickModName;
			}
		}

		public List<string> GetWarnings()
		{
			List<string> warnings = new List<string>();

			if ( Definition != null )
			{
				if ( string.IsNullOrWhiteSpace( Definition.Description ) )
				{
					warnings.Add( "This mod has no description." );
				}
				if ( Definition.Authors == null || Definition.Authors.Count < 1 )
				{
					warnings.Add( "This mod has no contact information." );
				}
				if ( Definition.Contacts == null || Definition.Contacts.Count < 1 )
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
