#include "stdafx.h"
#include "Windows.h"

HOOKPROC pHookCatcher;

HMODULE hLib;
HHOOK hHook = NULL;

extern "C" __declspec(dllexport) void SetHook(void)
{
	if(hHook != 0) return;
	
	hLib=LoadLibrary("Hooker");
	pHookCatcher=(HOOKPROC)GetProcAddress(hLib,"CallWndProc");
	hHook=SetWindowsHookEx(WH_SHELL, pHookCatcher, hLib, 0);
	if (hHook == NULL)
	{
		throw;
	}
}

extern "C" __declspec(dllexport) void UnHook(void)
{
	UnhookWindowsHookEx(hHook);
	hHook = 0;
}

extern "C" __declspec(dllexport) int CallWndProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	if(nCode < 0)
		return CallNextHookEx(0, nCode, wParam, lParam);

	if (nCode == HSHELL_LANGUAGE)
	{
		HWND window = FindWindow(0, "Alex Shestakov's Keyboard Layout Monitor");
		PostMessage(window, WM_USER+7, 0, lParam );
	}

	return CallNextHookEx(0,nCode,wParam, lParam);
}