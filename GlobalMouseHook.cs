using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace MagicSwitcher
{
    class GlobalMouseHook
    {
        // Only one instace of the global Mouse hook is possible, so the singleton pattern is used
        public static GlobalMouseHook Instance { get; private set; }
        // This reference to the delegate object is necessary to avoid destroying the delegate object by the Garbage Collector
        private static WinAPI.MouseHookProc callbackDelegate;

        static GlobalMouseHook()
        {
            Instance = new GlobalMouseHook();
            if (Instance != null)
                Instance.Hook();
        }

        ~GlobalMouseHook()
        {
            if (Instance != null)
                Instance.UnHook();
        }

        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseDblClick;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseWheel;

        /// <summary>
        /// Handle to the hook, need this to unhook and call the next hook
        /// </summary>
        private IntPtr hookHandle = IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the globalMouseHook class and installs the Mouse hook.
        /// </summary>
        private void Hook()
        {
            Debug.Assert(callbackDelegate == null, "Can't hook more than once");
            // if (callbackDelegate != null)
            //    throw new InvalidOperationException("Can't hook more than once");
            Debug.WriteLine("Hook");
            IntPtr hInstance = WinAPI.LoadLibrary("User32");
            callbackDelegate = new WinAPI.MouseHookProc(MouseHookProc);
            hookHandle = WinAPI.SetWindowsHookEx(WinAPI.WH_MOUSE_LL, callbackDelegate, hInstance, 0);
            if (hookHandle == IntPtr.Zero) throw new Win32Exception();
        }

        // Releases unmanaged resources and performs other cleanup operations before the
        // globalMouseHook is reclaimed by garbage collection and uninstalls the Mouse hook.
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
        /// The callback for the Mouse hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        private int MouseHookProc(int code, int wParam, ref WinAPI.MSLLHOOKSTRUCT lParam)
        {
            if (code >= 0)
            {
                MouseEventHandler mouseEvent = null;
                var button = MouseButtons.None;
                int delta = 0;

                switch (wParam)
                {
                    case WinAPI.WM_LBUTTONDOWN:
                    case WinAPI.WM_NCLBUTTONDOWN:
                        button = MouseButtons.Left;
                        mouseEvent = MouseDown;
                        break;

                    case WinAPI.WM_RBUTTONDOWN:
                    case WinAPI.WM_NCRBUTTONDOWN:
                        button = MouseButtons.Right;
                        mouseEvent = MouseDown;
                        break;

                    case WinAPI.WM_MBUTTONDOWN:
                    case WinAPI.WM_NCMBUTTONDOWN:
                        button = MouseButtons.Middle;
                        mouseEvent = MouseDown;
                        break;

                    case WinAPI.WM_LBUTTONUP:
                    case WinAPI.WM_NCLBUTTONUP:
                        button = MouseButtons.Left;
                        mouseEvent = MouseUp;
                        break;

                    case WinAPI.WM_RBUTTONUP:
                    case WinAPI.WM_NCRBUTTONUP:
                        button = MouseButtons.Right;
                        mouseEvent = MouseUp;
                        break;

                    case WinAPI.WM_MBUTTONUP:
                    case WinAPI.WM_NCMBUTTONUP:
                        button = MouseButtons.Middle;
                        mouseEvent = MouseUp;
                        break;

                    case WinAPI.WM_LBUTTONDBLCLK:
                    case WinAPI.WM_NCLBUTTONDBLCLK:
                        button = MouseButtons.Left;
                        mouseEvent = MouseDblClick;
                        break;

                    case WinAPI.WM_RBUTTONDBLCLK:
                    case WinAPI.WM_NCRBUTTONDBLCLK:
                        button = MouseButtons.Right;
                        mouseEvent = MouseDblClick;
                        break;

                    case WinAPI.WM_MBUTTONDBLCLK:
                    case WinAPI.WM_NCMBUTTONDBLCLK:
                        button = MouseButtons.Middle;
                        mouseEvent = MouseDblClick;
                        break;

                    case WinAPI.WM_MOUSEMOVE:
                    case WinAPI.WM_NCMOUSEMOVE:
                        mouseEvent = MouseMove;
                        break;

                    case WinAPI.WM_MOUSEHWHEEL:
                    case WinAPI.WM_MOUSEWHEEL:
                        delta = lParam.mouseData >> 16;
                        mouseEvent = MouseWheel;
                        break;
                }

                if (mouseEvent != null)
                {
                    var args = new MouseEventArgs(button, 1, lParam.pt.x, lParam.pt.y, delta);
                    mouseEvent(this, args);
                }
            }
            return WinAPI.CallNextHookEx(hookHandle, code, wParam, ref lParam);
        }
    }
}
