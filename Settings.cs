using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicSwitcher
{
    public static class Settings
    {
        public static string ShortcutEnglish { get; private set; } = Properties.Settings.Default.ShortcutEnglish;
        public static string ShortcutRussian { get; private set; } = Properties.Settings.Default.ShortcutRussian;
        public static string ShortcutUkrainian { get; private set; } = Properties.Settings.Default.ShortcutUkrainian;
        public static string ShortcutPolish { get; private set; } = Properties.Settings.Default.ShortcutPolish;
        public static string ShortcutNextLng { get; private set; } = Properties.Settings.Default.ShortcutNextLng;
    }
}
