using System;
using System.Runtime.InteropServices;

namespace MagicSwitcher
{
    /// <summary>
    /// This class contains Windows API functions and constants
    /// </summary>
    public static class WinAPI
    {
        #region Keyboard hook constants, types and functions
        // Hook type
        public const int WH_KEYBOARD_LL = 13;
        public const int WH_MOUSE_LL = 14;
        // Keyboard wParam
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;
        // Keyboard Flags
        public const int LLKHF_EXTENDED = 0x01;
        public const int LLKHF_INJECTED = 0x10;
        public const int LLKHF_ALTDOWN = 0x20;
        public const int LLKHF_UP = 0x80;
        // Mouse wParam
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_MOUSEHWHEEL = 0x020E;
        public const int WM_NCMOUSEMOVE = 0x00A0;
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int WM_NCLBUTTONUP = 0x00A2;
        public const int WM_NCLBUTTONDBLCLK = 0x00A3;
        public const int WM_NCRBUTTONDOWN = 0x00A4;
        public const int WM_NCRBUTTONUP = 0x00A5;
        public const int WM_NCRBUTTONDBLCLK = 0x00A6;
        public const int WM_NCMBUTTONDOWN = 0x00A7;
        public const int WM_NCMBUTTONUP = 0x00A8;
        public const int WM_NCMBUTTONDBLCLK = 0x00A9;

        public enum MouseWParam
        {
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDBLCLK = 0x0209,

            WM_MOUSEWHEEL = 0x020A,
            WM_MOUSEHWHEEL = 0x020E,

            WM_NCMOUSEMOVE = 0x00A0,
            WM_NCLBUTTONDOWN = 0x00A1,
            WM_NCLBUTTONUP = 0x00A2,
            WM_NCLBUTTONDBLCLK = 0x00A3,
            WM_NCRBUTTONDOWN = 0x00A4,
            WM_NCRBUTTONUP = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6,
            WM_NCMBUTTONDOWN = 0x00A7,
            WM_NCMBUTTONUP = 0x00A8,
            WM_NCMBUTTONDBLCLK = 0x00A9
        }

        /// <summary>
        /// Defines the x- and y- coordinates of a point
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// Contains information about a low-level keyboard input event
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;         // A virtual-key code. The code must be a value in the range 1 to 254
            public int scanCode;       // A hardware scan code for the key
            public int flags;          // The extended-key flag, event-injected flags, context code, and transition-state flag
            public int time;           // The time stamp for this message
            public IntPtr dwExtraInfo; // Additional information associated with the message
        }

        /// <summary>
        /// Contains information about a low-level mouse input event
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;           // The x- and y-coordinates of the cursor, in per-monitor-aware screen coordinates
            public int mouseData;      // Specifies which X button was pressed or released or the wheel delta
            public int flags;          // The event-injected flags
            public int time;           // The time stamp for this message
            public IntPtr dwExtraInfo; // Additional information associated with the message
        }

        /// <summary>
        /// Defines the callback type for the keyboard hook
        /// </summary>
        /// <param name="nCode">Hook code that the hook procedure uses to determine the action to perform.</param>
        /// <param name="wparam">Additional message-specific information.</param>
        /// <param name="lparam">Additional message-specific information.</param>
        /// <returns></returns>
        public delegate int KeyboardHookProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);

        /// <summary>
        /// Defines the callback type for the mouse hook
        /// </summary>
        /// <param name="nCode">Hook code that the hook procedure uses to determine the action to perform.</param>
        /// <param name="wparam">Additional message-specific information.</param>
        /// <param name="lparam">Additional message-specific information.</param>
        /// <returns></returns>
        public delegate int MouseHookProc(int nCode, int wParam, ref MSLLHOOKSTRUCT lParam);

        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain. 
        /// </summary>
        /// <param name="idHook">The type of hook procedure to be installed.</param>
        /// <param name="lpfn">A pointer to the hook procedure.</param>
        /// <param name="hMod">A handle to the DLL containing the hook procedure pointed to by the lpfn parameter.</param>
        /// <param name="dwThreadId">The identifier of the thread with which the hook procedure is to be associated.</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, MouseHookProc lpfn, IntPtr hMod, uint dwThreadId);

        /// <summary>
        /// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="hhk">A handle to the hook to be removed.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        /// Passes the hook information to the next hook procedure in the current hook chain.
        /// </summary>
        /// <param name="hhk">This parameter is ignored.</param>
        /// <param name="nCode">The hook code passed to the current hook procedure.</param>
        /// <param name="wparam">Additional message-specific information.</param>
        /// <param name="lparam">Additional message-specific information.</param>
        /// <returns>
        /// This value is returned by the next hook procedure in the chain.
        /// The current hook procedure must also return this value. The meaning of the return value depends on the hook type.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, int wParam, ref MSLLHOOKSTRUCT lParam);

        /// <summary>
        /// Loads the specified module into the address space of the calling process.
        /// </summary>
        /// <param name="lpFileName">The name of the module.</param>
        /// <returns>If the function succeeds, the return value is a handle to the module.</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpFileName);
        #endregion

        #region Keyboard layout constants, types and functions
        public const uint KLF_ACTIVATE = 1;
        public const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        public const uint INPUTLANGCHANGE_SYSCHARSET = 0x0001;
        public const uint INPUTLANGCHANGE_FORWARD = 0x0002;
        public const uint INPUTLANGCHANGE_BACKWARD = 0x0004;

        /// <summary>
        /// Defines the coordinates of the upper-left and lower-right corners of a rectangle.
        /// </summary>
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        /// <summary>
        /// Contains information about a GUI thread.
        /// </summary>
        public struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rectCaret;
        }

        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which the user is currently working).
        /// </summary>
        /// <returns>The return value is a handle to the foreground window.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Loads a new input locale identifier (formerly called the keyboard layout) into the system.
        /// </summary>
        /// <param name="pwszKLID">The name of the input locale identifier to load.</param>
        /// <param name="Flags">Specifies how the input locale identifier is to be loaded.</param>
        /// <returns>
        /// If the function succeeds, the return value is the input locale identifier corresponding
        /// to the name specified in pwszKLID. If no matching locale is available, the return value is the
        /// default language of the system.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

        /// <summary>
        /// Places (posts) a message in the message queue associated with the thread that created the specified
        /// window and returns without waiting for the thread to process the message. 
        /// </summary>
        /// <param name="hWnd">A handle to the window whose window procedure is to receive the message.</param>
        /// <param name="msg">The message to be posted.</param>
        /// <param name="wparam">Additional message-specific information.</param>
        /// <param name="lparam">Additional message-specific information.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wparam, IntPtr lparam);

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified window and, optionally, the identifier
        /// of the process that created the window. 
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="proccess">A pointer to a variable that receives the process identifier.</param>
        /// <returns>The return value is the identifier of the thread that created the window.</returns>
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr proccess);

        /// <summary>
        /// Retrieves information about the active window or a specified GUI thread. 
        /// </summary>
        /// <param name="idThread">The identifier for the thread for which information is to be retrieved.</param>
        /// <param name="lpgui">A pointer to a GUITHREADINFO structure that receives information describing the thread.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);
        #endregion

        #region Console
        // http://msdn.microsoft.com/en-us/library/ms683150(VS.85).aspx
        /// <summary>
        /// Detaches the calling process from its console.
        /// </summary>
        /// <returns>nonzero if the function succeeds; otherwise, zero.</returns>
        /// <remarks>
        /// If the calling process is not already attached to a console,
        /// the error code returned is ERROR_INVALID_PARAMETER (87).
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int FreeConsole();
        #endregion
    }
}