using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher.Modder
{
	public class ConsoleControlWriter : TextWriter
	{
		private Control TextBox;

		public ConsoleControlWriter( Control TextBox )
		{
			this.TextBox = TextBox;
		}

		public override void Write( char value )
		{
			TextBox.Text += value;
		}

		public override void Write( string value )
		{
			TextBox.Text += value;
		}

		public override Encoding Encoding
		{
			get
			{
				return Encoding.ASCII;
			}
		}
	}

	public class ConsoleControlTraceListener : TraceListener
	{
		private RichTextBox m_TextBox;

		public ConsoleControlTraceListener( RichTextBox Control )
		{
			m_TextBox = Control;
		}

		protected void WriteOnThread(string Text)
		{
			m_TextBox.SelectionStart = m_TextBox.TextLength;
			m_TextBox.SelectionLength = 0;
			m_TextBox.SelectionColor = System.Drawing.Color.Gray;
			m_TextBox.AppendText( $"{DateTime.Now.ToString( "HH:mm:ss" )} | {Text}" );
		}

		public override void Write( string Text )
		{
			if ( m_TextBox.IsHandleCreated )
			{
				m_TextBox.Parent.Invoke( new MethodInvoker( () => { WriteOnThread( Text ); } ) );
			}
		}

		public override void WriteLine( string Text )
		{
			Write( Text + "\r\n" );
		}
	}

}
