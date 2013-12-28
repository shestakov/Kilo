using System.Runtime.InteropServices;

namespace KeyboardLayoutMonitor
{
	// ReSharper disable InconsistentNaming
	public static class DwmApi
	{
		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern bool DwmIsCompositionEnabled();

		[DllImport("dwmapi.dll", EntryPoint = "#127", PreserveSig = false)]
		public static extern void DwmGetColorizationParameters(out WDM_COLORIZATION_PARAMS parameters);

		[DllImport("dwmapi.dll", EntryPoint = "#131", PreserveSig = false)]
		public static extern void DwmSetColorizationParameters(ref WDM_COLORIZATION_PARAMS parameters, uint uUnknown);

		[StructLayout(LayoutKind.Sequential)]
		public struct WDM_COLORIZATION_PARAMS
		{
			public uint Color1;
			public uint Color2;
			public uint Intensity;
			public uint Unknown1;
			public uint Unknown2;
			public uint Unknown3;
			public uint Opaque;
		}
	}
	// ReSharper restore InconsistentNaming
}