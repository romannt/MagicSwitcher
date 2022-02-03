using System;
using System.Collections;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace MagicSwitcher
{
    class Program
    {
        static void Main(string[] args)
        {
            // Если запущено как консольное приложение
            if (Environment.UserInteractive)
            {
                if (!ParseArgs(args))
                {
                    WinAPI.FreeConsole();
                    App.Start();
                }
            }
            else
            {
                // Запуск службы Windows
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { new WinService() };
                ServiceBase.Run(ServicesToRun);
            }
        }

        /// <summary>
        /// Обрабатывает параметры командной строки.
        /// </summary>
        /// <param name="args">Массив параметров командной строки</param>
        /// <returns>Возвращает true, если была выполнена команда, переданная в парамере командной строки.</returns>
        static bool ParseArgs(string[] args)
        {
            var commandFound = false;
            foreach (var command in args)
            {
                switch (command.ToUpper())
                {
                    case "/?":
                    case "/HELP":
                        // Console Output from a Winforms Application
                        // http://benincampus.blogspot.com/2011/03/re-console-output-from-winforms.html
                        // WinAPI.AttachConsole(WinAPI.ATTACH_PARENT_PROCESS);
                        Console.WriteLine($"{ProgramTitle()} {ProgramVersion()}" + Environment.NewLine);
                        Console.WriteLine(GetProgramInfo());
                        commandFound = true;
                        break;
                    case "/INSTALL":
                        Console.WriteLine("Install MagicSwitcher as a Windows Service");
                        InstallService();
                        StartService();
                        commandFound = true;
                        break;
                    case "/UNINSTALL":
                        Console.WriteLine("Removing MagicSwitcher from Windows Services");
                        StopService();
                        UninstallService();
                        commandFound = true;
                        break;
                }
                if (commandFound)
                    break;
            }
            return commandFound;
        }
        /// <summary>
        /// Возвращает информацию о программе для вывода в окне About
        /// </summary>
        /// <returns>Возвращает информацию о программе</returns>
        static string GetProgramInfo()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var description = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0] as AssemblyDescriptionAttribute;
            var copyright = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0] as AssemblyCopyrightAttribute;
            var result = description.Description + Environment.NewLine +
                copyright.Copyright + Environment.NewLine +
                "Parameters: " + Environment.NewLine +
                " /? or /help: Show the help" + Environment.NewLine +
                " /install: install the program as a Windows Service" + Environment.NewLine +
                " /uninstall: remove Magic Switcher from Windows Services" + Environment.NewLine;
            return result;
        }

        /// <summary>
        /// Returns the title of the program.
        /// </summary>
        /// <returns>Returns the title of the program.</returns>
        public static string ProgramTitle()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyTitleAttribute assemblyTitle =
                assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0] as AssemblyTitleAttribute;
            return assemblyTitle.Title;
        }

        /// <summary>
        /// Returns the version number of the program.
        /// </summary>
        /// <returns>Returns the version number of the program.</returns>
        public static string ProgramVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version.ToString();
            return version;
        }

        //----------------------------------------------------------------------
        // Код для установки и удаления службы - временно в Program.cs
        // Нужно будет вынести в отдельный класс
        //----------------------------------------------------------------------

        /// <summary>
        /// Возвращает имя службы Windows
        /// </summary>
        /// <returns>Возвращает имя службы Windows</returns>
        public static string ServiceName()
        {
            return ProgramTitle() + " Service";
        }

        private static bool IsInstalled()
        {
            using (ServiceController controller = new ServiceController(ServiceName()))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        /*
        private static bool IsRunning()
        {
            using (ServiceController controller = new ServiceController(ServiceName()))
            {
                if (!IsInstalled())
                    return false;
                return (controller.Status == ServiceControllerStatus.Running);
            }
        }
        */

        private static AssemblyInstaller GetInstaller()
        {
            AssemblyInstaller installer = new AssemblyInstaller(typeof(WinService).Assembly, null)
            {
                UseNewContext = true
            };
            return installer;
        }

        private static void InstallService()
        {
            if (IsInstalled())
                return;
            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(state);
                        }
                        catch { }
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private static void UninstallService()
        {
            if (!IsInstalled())
                return;
            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Uninstall(state);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private static void StartService()
        {
            if (!IsInstalled())
                return;
            using (ServiceController controller = new ServiceController(ServiceName()))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        private static void StopService()
        {
            if (!IsInstalled())
                return;
            using (ServiceController controller = new ServiceController(ServiceName()))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
