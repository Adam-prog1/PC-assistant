using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Management;
using System.Windows;


namespace PC_assistant.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public HomeViewModel()
        {

            // Заполняем список характеристик при инициализации окна
            Loaded += MainWindow_Loaded;
        }


        // Код для вывода характеристик
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Получение и отображение характеристик компьютера
            string processorInfo = GetProcessorInfo();
            string ramInfo = GetRAMInfo();
            string biosYearInfo = GetBiosYearInfo();
            string osInfo = GetOSInfo();

            // Форматирование строки и установка текста с символами новой строки
            SystemInfo.Text = $"Процессор: {processorInfo}\nОперативная память: {ramInfo}\nГод выпуска BIOS: {biosYearInfo}\nТекущая ОС: {osInfo}";
        }

        // Код для вывода характеристик процессора
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

        // Код для вывода характеристик ОЗУ
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

        // Код для вывода характеристик BIOS
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

        // Код для вывода версии Windows
        private string GetOSInfo()
        {
            string osVersion = Environment.OSVersion.Version.ToString();
            string osDescription = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
            string osBuild = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", "").ToString();

            return $"{osDescription} {osVersion} build {osBuild}";
        }







    }
}
