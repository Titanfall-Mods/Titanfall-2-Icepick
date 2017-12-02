using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher.Controls
{
	class InfoProgressBar : ProgressBar
	{
		public string InfoText { get; set; }
		public bool DisplayError { get; set; }

		public InfoProgressBar()
		{
			SetStyle( ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true );
			SetStyle( ControlStyles.OptimizedDoubleBuffer, true );
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			Rectangle rect = ClientRectangle;
			Graphics g = e.Graphics;

			ProgressBarRenderer.DrawHorizontalBar( g, rect );
			rect.Inflate( -3, -3 );
			if ( Value > 0 )
			{
				// As we doing this ourselves we need to draw the chunks on the progress bar
				Rectangle clip = new Rectangle( rect.X, rect.Y, (int) Math.Round( ( (float) Value / Maximum ) * rect.Width ), rect.Height );
				e.Graphics.FillRectangle( DisplayError? Brushes.Red : Brushes.LimeGreen, 2, 2, clip.Width + 2, clip.Height + 2 );
			}

			using ( Font f = new Font( FontFamily.GenericSansSerif, 8 ) )
			{

				SizeF len = g.MeasureString( InfoText, f );
				// Calculate the location of the text (the middle of progress bar)
				// Point location = new Point(Convert.ToInt32((rect.Width / 2) - (len.Width / 2)), Convert.ToInt32((rect.Height / 2) - (len.Height / 2)));
				Point location = new Point( Convert.ToInt32( ( Width / 2 ) - len.Width / 2 ), Convert.ToInt32( ( Height / 2 ) - len.Height / 2 ) );
				// The commented-out code will centre the text into the highlighted area only. This will centre the text regardless of the highlighted area.
				// Draw the custom text
				g.DrawString( InfoText, f, Brushes.Black, location );
			}
		}

	}
}
