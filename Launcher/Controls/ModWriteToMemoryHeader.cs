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
	public partial class ModWriteToMemoryHeader : UserControl
	{
		public ModWriteToMemoryHeader()
		{
			InitializeComponent();
		}

		public new string Text
		{
			set
			{
				label.Text = value;
			}
		}

	}
}
