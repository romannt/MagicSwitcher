using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MagicSwitcher
{
    /// <summary>
    /// Provides methods to change the input language in the foreground window.
    /// </summary>
    public static class Layouts
    {
        /// <summary>
        /// Activates the keyboard layout with the specified code in the foreground window.
        /// </summary>
        /// <param name="cultureName">Culture code (en-US, ru-Ru, uk-UA)</param>
        public static void ActivateLayout(string cultureName)
        {
            int keyboardLayoutId = new CultureInfo(cultureName).KeyboardLayoutId;
            string InputLocaleId = keyboardLayoutId.ToString("X8");
            WinAPI.PostMessage(GetActiveWindow(), WinAPI.WM_INPUTLANGCHANGEREQUEST,
                new IntPtr(WinAPI.INPUTLANGCHANGE_SYSCHARSET), WinAPI.LoadKeyboardLayout(InputLocaleId, WinAPI.KLF_ACTIVATE));
        }

        /// <summary>
        /// Activates the next keyboard layout in the foreground window.
        /// </summary>
        public static void ActivateNextLayout()
        {
            WinAPI.PostMessage(GetActiveWindow(), WinAPI.WM_INPUTLANGCHANGEREQUEST,
                new IntPtr(WinAPI.INPUTLANGCHANGE_FORWARD), IntPtr.Zero);
        }

        /// <summary>
        /// Retrieves a handle to the focused or foreground window.
        /// </summary>
        /// <returns>The return value is a handle to the focused or foreground window.</returns>
        public static IntPtr GetActiveWindow()
        {
            var guiThread = new WinAPI.GUITHREADINFO();
            guiThread.cbSize = Marshal.SizeOf(guiThread);
            WinAPI.GetGUIThreadInfo(WinAPI.GetWindowThreadProcessId(WinAPI.GetForegroundWindow(), IntPtr.Zero), ref guiThread);
            IntPtr activeWindowHandle = guiThread.hwndFocus;
            if (activeWindowHandle == IntPtr.Zero)
            {
                activeWindowHandle = WinAPI.GetForegroundWindow();
            }
            return activeWindowHandle;
        }
    }
}
