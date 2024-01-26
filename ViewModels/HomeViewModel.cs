using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Management;


namespace PC_assistant.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private string _systemInfo;

        public string SystemInfo
        {
            get { return _systemInfo; }
            set
            {
                if (_systemInfo != value)
                {
                    _systemInfo = value;
                    OnPropertyChanged(nameof(SystemInfo));
                }
            }
        }

        public HomeViewModel()
        {
            // Заполняем список характеристик при инициализации
            PopulateSystemInfo();

            // Добавляем обработчик события PropertyChanged из базового класса
            PropertyChanged += HandlePropertyChanged;
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Обработка изменений свойств
        }

        // Логика получения информации о системе и её вывод
        private void PopulateSystemInfo()
        {
            string processorInfo = GetProcessorInfo();
            string ramInfo = GetRAMInfo();
            string biosYearInfo = GetBiosYearInfo();
            string osInfo = GetOSInfo();

            // Форматирование строки и установка текста
            SystemInfo = $"Процессор: {processorInfo}\nОперативная память: {ramInfo}\nГод выпуска BIOS: {biosYearInfo}\nТекущая ОС: {osInfo}";
        }

        // Логика для получения характеристик процессора
        private string GetProcessorInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["Name"].ToString();
                }
            }
            catch (Exception ex)
            {
                return "Ошибка при получении информации о процессоре: " + ex.ToString();
            }

            return "Информация о процессоре недоступна.";
        }

        // Логика для получения характеристик ОЗУ
        private string GetRAMInfo()
        {          
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    ulong ramSize = Convert.ToUInt64(obj["TotalPhysicalMemory"]);
                    return $"{ramSize / (1024 * 1024)} MB";
                }
            }
            catch (Exception ex)
            {
                return "Ошибка при получении информации об оперативной памяти: " + ex.ToString();
            }

            return "Информация об оперативной памяти недоступна.";
        }

        // Логика для получения характеристик BIOS
        private string GetBiosYearInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["ReleaseDate"].ToString().Substring(0, 4);
                }
            }
            catch (Exception ex)
            {
                return "Ошибка при получении информации о годе выпуска BIOS: " + ex.ToString();
            }

            return "Информация о годе выпуска BIOS недоступна.";
        }

        // Логика для получения информации об ОС
        private string GetOSInfo()
        {
            string osVersion = Environment.OSVersion.Version.ToString();
            string osDescription = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
            string osBuild = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", "").ToString();

            return $"{osDescription} {osVersion} build {osBuild}";
        }

    }
}
