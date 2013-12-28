using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardLayoutMonitor
{
	public class KeyboardLayoutSwitchHooker
	{
		#region Interop
		// ReSharper disable InconsistentNaming

		public const int WM_USER = 0x0400;
		public const int WM_LANGUAGE_CHANGED = WM_USER + 7;

		[DllImport("Hooker")]
		public static extern void SetHook();

		[DllImport("Hooker")]
		public static extern void UnHook();

		// ReSharper restore InconsistentNaming
		#endregion

		public static void TryStartLayoutMonitor()
		{
			if (Environment.OSVersion.Version.Major < 6)
				return;
		}
	}

	public static class ColorSettingsController
	{
		public static void SetColor(IntPtr layoutHandle, Settings settings)
		{
			if (layoutHandle == currentLanguageHanlder) return;
			currentLanguageHanlder = layoutHandle;

			string languageName = null;
			foreach (InputLanguage language in InstalledInputLanguages)
			{
				if (language.Handle == layoutHandle)
				{
					languageName = language.Culture.ThreeLetterWindowsLanguageName;
					break;
				}
			}
			if (languageName == null) return;


			try
			{
				if (!DwmApi.DwmIsCompositionEnabled())
					return;
				DwmApi.WDM_COLORIZATION_PARAMS colorizationParams = (languageName == settings.DefaultLayoutName)
																		? settings.DefaultLayoutColorScheme
																		: settings.AlternativeLayoutColorScheme;
				DwmApi.DwmSetColorizationParameters(ref colorizationParams, 0);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				throw;
			}

		}

		private static IntPtr currentLanguageHanlder = IntPtr.Zero;
		private static readonly InputLanguageCollection InstalledInputLanguages = InputLanguage.InstalledInputLanguages;
	}
}