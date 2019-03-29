using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Engine.Libraries.GUI
{
	public class TextWrittenEventArgs : EventArgs
	{
		public string Text;
	}

	public class ConsoleRedirector : TextWriter
	{
		public event EventHandler<TextWrittenEventArgs> TextWritten;

		public override void Write(string value)
		{
			base.Write(value);

			TextWritten(null, new TextWrittenEventArgs() { Text = value });
		}

		public override void WriteLine(string value)
		{
			base.WriteLine(value);

			TextWritten(null, new TextWrittenEventArgs() { Text = value + "\n" });
		}

		public override Encoding Encoding => Encoding.UTF8;
	}
}
