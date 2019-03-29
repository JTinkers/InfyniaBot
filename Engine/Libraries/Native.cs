using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Libraries
{
	public static class Native
	{
		[DllImport("kernel32")]
		public static extern bool AllocConsole();

		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

		[DllImport("user32.dll")]
		public static extern short GetAsyncKeyState(int key);

		public const int WM_KEYDOWN = 0x100;
		public const int WM_KEYUP = 0x0101;
	}
}
