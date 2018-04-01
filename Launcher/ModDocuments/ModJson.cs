using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JsonFx.Json;
using Launcher.ModDocuments;

namespace Launcher
{
	class ModJson : ModDocuments.ModBase
	{
		const string MOD_DOCUMENT_NAME = "mod.json";

		public delegate void DocumentNodeDelegate( ModJson TargetMod, KeyValuePair<string, object> DataPair );
		private Dictionary<string, DocumentNodeDelegate> DocumentNodeActions = new Dictionary<string, DocumentNodeDelegate>()
		{
			{ "Name", WriteAsFieldString },
			{ "Description", WriteAsFieldString },
			{ "Authors", WriteToFieldStringList },
			{ "Contacts", WriteToFieldStringList },
			{ "Files", WriteToFieldFilesList }
		};

		public new static bool ShouldLoad( string ModPath )
		{
			return System.IO.File.Exists( $"{ModPath}{System.IO.Path.DirectorySeparatorChar}{MOD_DOCUMENT_NAME}" );
		}

		public override void Load( string ModPath )
		{
			base.Load( ModPath );

			// Load json mod file
			try
			{
				StreamReader FileReader = new StreamReader( $"{ModPath}{System.IO.Path.DirectorySeparatorChar}{MOD_DOCUMENT_NAME}" );
				string ModDocumentContents = FileReader.ReadToEnd();
				FileReader.Close();

				JsonReader Reader = new JsonReader();
				var JsonData = Reader.Read<Dictionary<string, object>>( ModDocumentContents );
				foreach ( var Pair in JsonData )
				{
					if ( DocumentNodeActions.ContainsKey( Pair.Key ) )
					{
						DocumentNodeActions[ Pair.Key ]( this, Pair );
					}
					else
					{
						Debug.WriteLine( $"Unhandled key in json mod document, {Pair.Key} = {Pair.Value}" );
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine( $"[Warning] Could not load mod: {e.Message}" );
			}

			// Setup file watchers
			try
			{
				WatchFiles();
			}
			catch ( Exception e )
			{
				Console.WriteLine( $"[Warning] Failed to setup watcher on mod '{ModPath}', auto-reload may not work. {e.Message}" );
			}
		}

		public static void WriteAsFieldString( ModJson TargetMod, KeyValuePair<string, object> DataPair )
		{
			FieldInfo Field = typeof( ModJson ).GetField( DataPair.Key, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			Field.SetValue( TargetMod, (string) DataPair.Value );
		}

		public static void WriteToFieldStringList( ModJson TargetMod, KeyValuePair<string, object> DataPair )
		{
			FieldInfo Field = typeof( ModJson ).GetField( DataPair.Key, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			List<string> TargetField = Field.GetValue( TargetMod ) as List<string>;
			var DataList = DataPair.Value as string[];
			foreach ( var Value in DataList )
			{
				TargetField.Add( Value );
			}
		}

		public static void WriteToFieldFilesList( ModJson TargetMod, KeyValuePair<string, object> DataPair )
		{
			FieldInfo Field = typeof( ModJson ).GetField( DataPair.Key, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			List<ModFile> TargetField = Field.GetValue( TargetMod ) as List<ModFile>;

			foreach ( dynamic Data in ( DataPair.Value as System.Dynamic.ExpandoObject[] ) )
			{
				IDictionary<String, object> DataDict = (IDictionary<String, object>) Data;
				ModFile NewFile = new ModFile( TargetMod );
				NewFile.id = GetExpandoProperty( DataDict, "id", string.Empty );
				NewFile.Chunk = GetExpandoProperty( DataDict, "Chunk", string.Empty );
				NewFile.ReplacementFile = GetExpandoProperty( DataDict, "Replacement", string.Empty );
				NewFile.AppendedFiles = GetExpandoProperty( DataDict, "Appends", new string[]{} );
				TargetField.Add( NewFile );
			}
		}

		public static T GetExpandoProperty<T>( IDictionary<String, object> DataSource, string ValueName, T DefaultValue )
		{
			object Temp;
			DataSource.TryGetValue( ValueName, out Temp );
			return (T) ( Temp ?? DefaultValue );
		}

	}
}
