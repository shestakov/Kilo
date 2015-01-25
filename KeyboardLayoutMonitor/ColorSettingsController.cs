using System;
using System.Windows.Forms;

namespace KeyboardLayoutMonitor
{
	public static class ColorSettingsController
	{
		private static IntPtr currentLanguageHanlder = IntPtr.Zero;
		private static readonly InputLanguageCollection installedInputLanguages = InputLanguage.InstalledInputLanguages;

		public static void SetColor(IntPtr layoutHandle, Settings settings)
		{
			if (layoutHandle == currentLanguageHanlder) return;
			currentLanguageHanlder = layoutHandle;

			string languageName = null;
			foreach (InputLanguage language in installedInputLanguages)
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
				var colorizationParams = (languageName == settings.DefaultLayoutName)
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
	}
}