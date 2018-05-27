using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Launcher.Forms
{
	public partial class SpawnlistGenerator : Form
	{
		private const string MODELS_FOLDER_NAME = @"\models";
		private const string OUTPUT_FORMAT = "\nPrecacheList = [\n{0}];\n\n// #include scripts/spawnlists/sh_spawnlist_loader.nut\n";
		private const string MODEL_FORMAT = "\t$\"{0}\"{1}\n";

		public SpawnlistGenerator()
		{
			InitializeComponent();
		}

		private void btnBrowse_Click( object sender, EventArgs e )
		{
			DialogResult Result = folderBrowserDialog.ShowDialog();
			if( Result == DialogResult.OK )
			{
				textBoxLocation.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void btnGenerate_Click( object sender, EventArgs e )
		{
			GenerateSpawnList();
		}

		private void GenerateSpawnList()
		{
			ClearOutput();

			string ModelsLocation = textBoxLocation.Text;
			if ( string.IsNullOrEmpty( ModelsLocation ) )
			{
				AppendOutput( "No models location specified." );
				return;
			}

			// Check if we specified a models folder directly, or a whole vpk export folder
			if ( !ModelsLocation.EndsWith( MODELS_FOLDER_NAME ) )
			{
				foreach ( var SubDir in Directory.GetDirectories( ModelsLocation ) )
				{
					if( SubDir.EndsWith( MODELS_FOLDER_NAME ) )
					{
						ModelsLocation = SubDir;
						break;
					}
				}
			}

			// Find all models
			List<string> ModelsList = new List<string>();
			AddFilesInDirectory( ModelsList, ModelsLocation );

			// Format and output models
			string BasePath = ModelsLocation.Replace( MODELS_FOLDER_NAME, string.Empty );
			string ModelsOutput = "";
			for ( int i = 0; i < ModelsList.Count; ++i )
			{
				ModelsList[ i ] = ModelsList[ i ].Replace( BasePath + @"\", string.Empty );
				ModelsList[ i ] = ModelsList[ i ].Replace( @"\", @"/" );
				ModelsOutput += string.Format( MODEL_FORMAT, ModelsList[ i ], i < ModelsList.Count - 1 ? ", " : "" );
			}

			// Format and output for squirrel
			ModelsOutput = string.Format( OUTPUT_FORMAT, ModelsOutput );
			ModelsOutput = ModelsOutput.Replace( "\n", Environment.NewLine );
			AppendOutput( ModelsOutput );
		}

		private void AddFilesInDirectory( List<string> ModelsList, string Path )
		{
			foreach ( var SubDir in Directory.GetDirectories( Path ) )
			{
				AddFilesInDirectory( ModelsList, SubDir );
			}
			foreach ( var File in Directory.GetFiles( Path ) )
			{
				ModelsList.Add( File );
			}
		}

		private void ClearOutput()
		{
			textBoxOutput.Text = string.Empty;
		}

		private void AppendOutput( string Line )
		{
			textBoxOutput.Text += Line + Environment.NewLine;
		}

	}
}
