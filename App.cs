using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MagicSwitcher
{
    public static class App
    {
        public static InputHook keyboardHook = null;

        [STAThread]
        public static void Start()
        {
            string guid = Marshal.GetTypeLibGuidForAssembly(Assembly.GetExecutingAssembly()).ToString();
            using (Mutex mutexObj = new Mutex(true, guid, out bool createdNewMutex))
            {
                if (!createdNewMutex)
                {
                    MessageBox.Show($"{ProgramTitle()} is already started");
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += ApplicationThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

                NotifyIcon notifyIcon = new NotifyIcon
                {
                    Icon = Properties.Resources.MagicSwitcher,
                    ContextMenuStrip = CreateContextMenu(),
                    Text = ProgramTitle(),
                    Visible = true
                };
                notifyIcon.DoubleClick += ShowSettings;
                // KeyboardHook keyboardHook = InitKeyboardHook();
                InitKeyboardHook();
                Application.Run();
                keyboardHook = null;
            }
        }

        /// <summary>
        /// Initialises keyboard shortcuts.
        /// </summary>
        /// <returns>Returns the keyboard hook handler.</returns>
        // private static KeyboardHook InitKeyboardHook()
        public static void InitKeyboardHook()
        {
            keyboardHook = new InputHook();
            AddShortCut(Settings.ShortcutEnglish, "English");
            AddShortCut(Settings.ShortcutRussian, "Russian");
            AddShortCut(Settings.ShortcutUkrainian, "Ukrainian");
            AddShortCut(Settings.ShortcutPolish, "Polish");
            AddShortCut(Settings.ShortcutNextLng, "Next");
        }

        private static void AddShortCut(string shortcut, string shortcutAction)
        {
            if (shortcut != "" && !string.Equals(shortcut, "Off", StringComparison.OrdinalIgnoreCase))
            {
                if (shortcut.ToUpper() == "LALT")
                {
                    shortcut = "LMenu";
                }
                else if (shortcut.ToUpper() == "RALT")
                {
                    shortcut = "RMenu";
                }

                if (Enum.TryParse(shortcut, true, out Keys key))
                {
                    InputSequence kbdShortcut = new InputSequence(shortcutAction);
                    kbdShortcut.Add(new InputEvent() { Key = key, EventType = InputEventType.KeyDown });
                    kbdShortcut.Add(new InputEvent() { Key = key, EventType = InputEventType.KeyUp });
                    kbdShortcut.Triggered += new InputSequence.KbdShortcutHandler(ShortcutHandler);
                    keyboardHook.AddSequence(kbdShortcut);
                    // System.Diagnostics.Debug.WriteLine(lShiftShortcut);
                }
                else
                {
                    throw new Exception($"Incorrect key name: \"{shortcut}\"");
                }

            }
        }

        /// <summary>
        /// A handler for the keyboars shortcuts.
        /// </summary>
        /// <param name="name"></param>
        private static void ShortcutHandler(string name)
        {
            switch (name)
            {
                case "English":
                    System.Diagnostics.Debug.WriteLine("English");
                    Layouts.ActivateLayout("en-US");
                    break;
                case "Russian":
                    System.Diagnostics.Debug.WriteLine("Russian");
                    Layouts.ActivateLayout("ru-Ru");
                    break;
                case "Ukrainian":
                    System.Diagnostics.Debug.WriteLine("Ukrainian");
                    Layouts.ActivateLayout("uk-UA");
                    break;
                case "Polish":
                    System.Diagnostics.Debug.WriteLine("Polish");
                    Layouts.ActivateLayout("pl-PL");
                    break;
                case "Next":
                    System.Diagnostics.Debug.WriteLine("Next language");
                    Layouts.ActivateNextLayout();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Creates the contex menu for the tray icon.
        /// </summary>
        /// <returns></returns>
        private static ContextMenuStrip CreateContextMenu()
        {
            var menu = new ContextMenuStrip();

            //var menuItemSettings = new ToolStripMenuItem("Settings...");
            //menuItemSettings.Click += new EventHandler(ShowSettings);
            //menu.Items.Add(menuItemSettings);

            var menuItemAbout = new ToolStripMenuItem("About");
            menuItemAbout.Click += new EventHandler(ShowAboutDialog);
            menu.Items.Add(menuItemAbout);

            var menuItemExit = new ToolStripMenuItem("Exit");
            menuItemExit.Click += new EventHandler(CloseApp);
            menu.Items.Add(menuItemExit);

            return menu;
        }

        /// <summary>
        /// A handler for the "About" button.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event argument</param>
        private static void ShowAboutDialog(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version.ToString();
            var description = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0] as AssemblyDescriptionAttribute;
            var copyright = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0] as AssemblyCopyrightAttribute;
            MessageBox.Show(description.Description + Environment.NewLine + Environment.NewLine +
                copyright.Copyright + Environment.NewLine,
                $"About {ProgramTitle()} {version}");
        }

        /// <summary>
        /// Returns the title of the program.
        /// </summary>
        /// <returns></returns>
        public static string ProgramTitle()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyTitleAttribute assemblyTitle =
                assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0] as AssemblyTitleAttribute;
            return assemblyTitle.Title;
        }

        /// <summary>
        /// A handler for the "Settings..." menu item.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event argument</param>
        private static void ShowSettings(object sender, EventArgs e)
        {
            new SettingsForm().Show();
        }

        /// <summary>
        /// A handler for the "Exit" menu item.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event argument</param>
        private static void CloseApp(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Global exceptions in Non User Interfarce(other thread) antipicated error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowExceptionDetails((Exception)e.ExceptionObject);
        }

        /// <summary>
        /// Global exceptions in User Interfarce antipicated error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ShowExceptionDetails(e.Exception);
        }

        /// <summary>
        /// Shows error message dialog.
        /// </summary>
        /// <param name="Ex">Exception information</param>
        private static void ShowExceptionDetails(Exception Ex)
        {
            var message = String.Format("{0}\r\n\r\n{1}", Ex.Message, Ex.StackTrace);
            MessageBox.Show(message, $"Unexpected error in {ProgramTitle()}");
        }
    }
}
