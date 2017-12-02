using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Launcher.Controls;
using Launcher.ModDocuments;

namespace Launcher.Forms
{
	public partial class WriteToMemoryProgress : Form
	{
		Dictionary<ModBase, FileWriteToMemoryProgress> ModProgressBars = new Dictionary<ModBase, FileWriteToMemoryProgress>();

		List<FileWriteToMemoryProgress> ProgressDisplays = new List<FileWriteToMemoryProgress>();

		public WriteToMemoryProgress()
		{
			InitializeComponent();
			updateTimer.Enabled = true;
		}

		public void AddModProgress(ModBase Mod)
		{
			ModWriteToMemoryHeader Header = new ModWriteToMemoryHeader();
			Header.Text = Mod.Path;
			flowLayoutPanel.Controls.Add( Header );

			foreach ( ModBase.File F in Mod.Files)
			{
				FileWriteToMemoryProgress Progress = new FileWriteToMemoryProgress();
				Progress.File = F;
				Progress.ModName = F.ToString();
				Progress.Progress = 10;

				flowLayoutPanel.Controls.Add( Progress );
				ProgressDisplays.Add( Progress );
			}

			ResizeProgressControls();
		}

		protected void ResizeProgressControls()
		{
			foreach ( var Progress in ProgressDisplays )
			{
				var Size = Progress.Size;
				Size.Width = flowLayoutPanel.Size.Width - 6 - ( flowLayoutPanel.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0 );
				Progress.Size = Size;
			}
		}

		private void WriteToMemoryProgress_Resize( object sender, EventArgs e )
		{
			ResizeProgressControls();
		}

		private void updateTimer_Tick( object sender, EventArgs e )
		{
			foreach ( var ProgressDisplay in ProgressDisplays )
			{
				ProgressDisplay.Progress = (int) Math.Floor( ProgressDisplay.File.Progress * 100.0f );
				ProgressDisplay.Success = ProgressDisplay.File.Success;
				if ( ProgressDisplay.File.Success != null && ProgressDisplay.File.Success == false )
				{
					ProgressDisplay.InfoText = ProgressDisplay.File.LastError;
				}
				else
				{
					ProgressDisplay.InfoText = ProgressDisplay.File.State.ToString();
				}

			}
		}
	}
}
