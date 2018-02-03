using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher.Controls
{
	public partial class FileWriteToMemoryProgress : UserControl
	{
		public FileWriteToMemoryProgress()
		{
			InitializeComponent();
		}

		public ModDocuments.ModFile File;

		public string ModName
		{
			get
			{
				return nameLabel.Text;
			}
			set
			{
				nameLabel.Text = value;
			}
		}

		public int Progress
		{
			get
			{
				return infoProgressBar.Value;
			}
			set
			{
				infoProgressBar.Value = value;
			}
		}

		public string InfoText
		{
			set
			{
				infoProgressBar.InfoText = value;
			}
		}

		public bool? Success
		{
			set
			{
				infoProgressBar.DisplayError = ( value != null && value == false );
			}
		}

	}
}
