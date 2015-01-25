using System.Runtime.InteropServices;

namespace KeyboardLayoutMonitor
{
	public static class KeyboardLayoutSwitchHooker
	{
		// ReSharper disable InconsistentNaming
		public const int WM_USER = 0x0400;
		public const int WM_LANGUAGE_CHANGED = WM_USER + 7;
		// ReSharper restore InconsistentNaming

		[DllImport("Hooker")]
		public static extern void SetHook();

		[DllImport("Hooker")]
		public static extern void UnHook();
	}
}