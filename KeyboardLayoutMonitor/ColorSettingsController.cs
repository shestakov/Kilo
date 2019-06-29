using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardLayoutMonitor
{
	public static class ColorSettingsController
	{
		private static IntPtr currentLanguageHanlder = IntPtr.Zero;
		private static readonly InputLanguageCollection installedInputLanguages = InputLanguage.InstalledInputLanguages;

        private static bool isWin10;
        private static bool findTaskbarHandles = true;
        private static bool runApplyTask = false;
        private static List<IntPtr> taskbarHandles = new List<IntPtr>();
        private static Win10Api.AccentPolicy accentPolicy;
        private static Task applyTask;
        private static void ApplyToAllTaskbars()
        {
            while (runApplyTask)
            {
                if (findTaskbarHandles)
                {
                    taskbarHandles.Clear();

                    var primaryBar = Win10Api.FindWindow("Shell_TrayWnd", null);

                    taskbarHandles.Add(primaryBar);

                    IntPtr secondaryBar = IntPtr.Zero;

                    while (true)
                    {
                        secondaryBar = Win10Api.FindWindowEx(IntPtr.Zero, secondaryBar, "Shell_SecondaryTrayWnd", "");
                        if (secondaryBar == IntPtr.Zero) { break; }
                        else
                        {
                            taskbarHandles.Add(secondaryBar);
                        }
                    }

                    findTaskbarHandles = false;
                }

                foreach (var handle in taskbarHandles)
                {
                    int sizeOfPolicy = Marshal.SizeOf(accentPolicy);
                    IntPtr policyPtr = Marshal.AllocHGlobal(sizeOfPolicy);
                    Marshal.StructureToPtr(accentPolicy, policyPtr, false);

                    Win10Api.WinCompatTrData data = new Win10Api.WinCompatTrData(Win10Api.WindowCompositionAttribute.WCA_ACCENT_POLICY, policyPtr, sizeOfPolicy);

                    Win10Api.SetWindowCompositionAttribute(handle, ref data);

                    Marshal.FreeHGlobal(policyPtr);
                }

                Thread.Sleep(10);
            }
        }

        public static void InitialiseWin10(Settings settings)
        {
            isWin10 = true;
            accentPolicy.AccentState = Win10Api.AccentState.ACCENT_ENABLE_GRADIENT;
            accentPolicy.GradientColor = settings.Win10DefaultLayoutColorScheme;

            runApplyTask = true;
            applyTask = new Task(() => ApplyToAllTaskbars());
            applyTask.Start();
        }

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

			
            try
            {
                if (isWin10)
                {
                    if (languageName == settings.DefaultLayoutName)
                    {
                        accentPolicy.GradientColor = settings.Win10DefaultLayoutColorScheme;
                    }
                    else
                    {
                        accentPolicy.GradientColor = settings.Win10AlternativeLayoutColorScheme;
                    }
                }
                else
                {
                    if (languageName == null) return;

                    if (!DwmApi.DwmIsCompositionEnabled())
                        return;
                    var colorizationParams = (languageName == settings.DefaultLayoutName)
                        ? settings.DefaultLayoutColorScheme
                        : settings.AlternativeLayoutColorScheme;
                    DwmApi.DwmSetColorizationParameters(ref colorizationParams, 0);
                }
				
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				throw;
			}
		}
	}
}