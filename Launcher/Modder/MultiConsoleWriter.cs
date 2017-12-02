using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Modder
{
	public class MultiConsoleWriter : TextWriter
	{
		private IEnumerable<TextWriter> Writers;

		public MultiConsoleWriter( IEnumerable<TextWriter> Writers )
		{
			this.Writers = Writers.ToList();
		}
		public MultiConsoleWriter( params TextWriter[] Writers )
		{
			this.Writers = Writers;
		}

		public override void Write( char value )
		{
			foreach ( var Writer in Writers )
			{
				Writer.Write( value );
			}
		}

		public override void Write( string value )
		{
			foreach ( var Writer in Writers )
			{
				Writer.Write( value );
			}
		}

		public override void Flush()
		{
			foreach ( var Writer in Writers )
			{
				Writer.Flush();
			}
		}

		public override void Close()
		{
			foreach ( var Writer in Writers )
			{
				Writer.Close();
			}
		}

		public override Encoding Encoding
		{
			get
			{
				return Encoding.Unicode;
			}
		}
	}
}
