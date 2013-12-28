using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace HookerWatcher
{
	class Program
	{
		static void Main()
		{
			try
			{
				SetHook();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
			while (true)
			{
				Thread.Sleep(10 * 1000);
				if (IntPtr.Zero == FindWindow(IntPtr.Zero, "Alex Shestakov's Keyboard Layout Monitor"))
				{
					try
					{
						UnHook();
					}
					catch
					{
					}
					break;
				}
			}
		}

		[DllImport("Hooker")]
		private static extern void SetHook();

		[DllImport("Hooker")]
		private static extern void UnHook();

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr FindWindow(IntPtr lpClassName, string lpWindowName);
	}
}
