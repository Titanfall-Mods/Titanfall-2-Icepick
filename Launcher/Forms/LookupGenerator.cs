using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher.Forms
{
	public partial class LookupGenerator : Form
	{
		public LookupGenerator()
		{
			InitializeComponent();
			Application.EnableVisualStyles();
		}

		private static Dictionary<char, string> LookupGeneratorReplacements = new Dictionary<char, string>()
		{
			{ '\t', @"\t" },
			{ '\n', @"\r\n" },
		};

		private void textBoxInput_TextChanged( object sender, EventArgs e )
		{
			UpdateOutputText();
		}

		private void textBoxOutput_TextChanged( object sender, EventArgs e )
		{
			UpdateOutputText();
		}

		private void btnClear_Click( object sender, EventArgs e )
		{
			textBoxOutput.Text = string.Empty;
			textBoxInput.Text = string.Empty;
		}

		private void UpdateOutputText()
		{
			string Output = textBoxInput.Text;
			foreach ( var CharacterPair in LookupGeneratorReplacements )
			{
				Output = Output.Replace( CharacterPair.Key.ToString(), CharacterPair.Value );
			}
			textBoxOutput.Text = Output;
		}
	}
}
