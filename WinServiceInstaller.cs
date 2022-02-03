using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace MagicSwitcher
{
    // Атрибут [RunInstaller(true)] указывает на то, что класс WinServiceInstaller должен вызываться при установке сборки, то есть службы.
    [RunInstaller(true)]
    public partial class WinServiceInstaller : System.Configuration.Install.Installer
    {
        // Класс System.Configuration.Install.Installer определяет ряд методов: Install() (установка),
        // Commit() (завершает транзакцию установки), Rollback() (восстанавливает состояние компьютера до установки)
        // и Uninstall() (удаление). При необходимости мы можем их переопределить. Но в нашем случае мы будем
        // использовать только конструктор.
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;
        public WinServiceInstaller()
        {
            InitializeComponent();
            // Класс ServiceProcessInstaller управляет настройкой значений для всех запускаемых служб внутри одного процесса
            // (метод Main класса Program может одновременно запускать несколько служб). Класс ServiceInstaller предназначен
            // для настройки значений для каждой из запускаемых служб. То есть если у нас запускается три службы, то для каждой
            // службы создается свой объект ServiceInstaller. В нашем случае запускается только одна служба, поэтому объекты
            // обоих классов у нас будут только в одном экземпляре.
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();
            // Настройки службы:
            // тип учетной записи = учетная запись предоставляет широкие привилегии на локальном компьютере и соответствует
            // компьютеру в сети
            processInstaller.Account = ServiceAccount.LocalSystem;
            // тип запуска = автоматически
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            // имя службы
            serviceInstaller.ServiceName = Program.ServiceName();
            // добавляем оба объекта установщиков в коллекцию Installers
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
