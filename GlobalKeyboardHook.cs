using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace MagicSwitcher
{
    class GlobalKeyboardHook
    {
        // Only one instace of the global keyboard hook is possible, so the singleton pattern is used
        public static GlobalKeyboardHook Instance { get; private set; }
        // This reference to the delegate object is necessary to avoid destroying the delegate object by the Garbage Collector
        private static WinAPI.KeyboardHookProc callbackDelegate;

        static GlobalKeyboardHook()
        {
            Instance = new GlobalKeyboardHook();
            if (Instance != null)
                Instance.Hook();
        }

        ~GlobalKeyboardHook()
        {
            if (Instance != null)
                Instance.UnHook();
        }

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;

        /// <summary>
        /// Handle to the hook, need this to unhook and call the next hook
        /// </summary>
        private IntPtr hookHandle = IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the globalKeyboardHook class and installs the keyboard hook.
        /// </summary>
        private void Hook()
        {
            Debug.Assert(callbackDelegate == null, "Can't hook more than once");
            // if (callbackDelegate != null)
            //    throw new InvalidOperationException("Can't hook more than once");
            Debug.WriteLine("Hook");
            IntPtr hInstance = WinAPI.LoadLibrary("User32");
            callbackDelegate = new WinAPI.KeyboardHookProc(KeyboardHookProc);
            hookHandle = WinAPI.SetWindowsHookEx(WinAPI.WH_KEYBOARD_LL, callbackDelegate, hInstance, 0);
            if (hookHandle == IntPtr.Zero) throw new Win32Exception();
        }

        // Releases unmanaged resources and performs other cleanup operations before the
        // globalKeyboardHook is reclaimed by garbage collection and uninstalls the keyboard hook.
        private void UnHook()
        {
            if (callbackDelegate != null)
            {
                Debug.WriteLine("UnHook");
                bool ok = WinAPI.UnhookWindowsHookEx(hookHandle);
                if (!ok) throw new Win32Exception();
                callbackDelegate = null;
            }
        }

        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        private int KeyboardHookProc(int code, int wParam, ref WinAPI.KBDLLHOOKSTRUCT lParam)
        {
            if (code >= 0)
            {
                Keys key = (Keys)lParam.vkCode;
                KeyEventArgs keyEventArgs = new KeyEventArgs(key);
                if ((wParam == WinAPI.WM_KEYDOWN || wParam == WinAPI.WM_SYSKEYDOWN) && (KeyDown != null))
                {
                    // System.Diagnostics.Debug.WriteLine($"KeyDown: {key}");
                    WinAPI.KBDLLHOOKSTRUCT a = lParam;
                    KeyDown(this, keyEventArgs);
                }
                else if ((wParam == WinAPI.WM_KEYUP || wParam == WinAPI.WM_SYSKEYUP) && (KeyUp != null))
                {
                    // System.Diagnostics.Debug.WriteLine($"KeyUp: {key}");
                    KeyUp(this, keyEventArgs);
                }
                if (keyEventArgs.Handled)
                    return 1;
            }
            return WinAPI.CallNextHookEx(hookHandle, code, wParam, ref lParam);
        }
    }
}
