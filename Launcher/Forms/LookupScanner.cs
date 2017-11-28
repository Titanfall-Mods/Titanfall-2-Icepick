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
	public partial class LookupScanner : Form
	{
		public LookupScanner()
		{
			InitializeComponent();
		}

		private void btnSearch_Click( object sender, EventArgs e )
		{
			SearchForMemory();
		}

		private async void SearchForMemory()
		{
			textStatus.Text = "Searching...";
			btnSearch.Enabled = false;
			textLookup.ReadOnly = true;

			long ExistingAddress = await Task.Factory.StartNew( () => Modder.MemoryModder.Instance.FindAddress( Encoding.ASCII.GetBytes( textLookup.Text ) ) );
			if ( ExistingAddress > 0 )
			{
				textStatus.Text = $"Address: {ExistingAddress.ToString( "X" )}";
			}
			else
			{
				textStatus.Text = "Address not found.";
			}
			btnSearch.Enabled = true;
			textLookup.ReadOnly = false;
		}

	}
}
