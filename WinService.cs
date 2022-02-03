using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace MagicSwitcher
{
    public partial class WinService : ServiceBase
    {
        public WinService()
        {
            InitializeComponent();
            CanStop = true; // службу можно остановить
            CanPauseAndContinue = false; // службу нельзя приостановить и затем продолжить (можно реализовать позже)
            AutoLog = false; // записывать в журнал Windows событий команд запуска, останова, паузы и возобновления
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                // Запуск службы Windows
                App.InitKeyboardHook();
                WriteToEventLog(Program.ProgramTitle() + " service has been started", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                WriteToEventLog(ex.Message, EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            // Остановка службы Windows
            // ...
            WriteToEventLog(Program.ProgramTitle() + " service has been stopped", EventLogEntryType.Information);
        }

        /// <summary>
        /// Выводит сообщение в лог событий Windows.
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="type">Тип сообщения</param>
        private void WriteToEventLog(string message, EventLogEntryType type)
        {
            EventLog.WriteEntry(message, type);
        }
    }
}
