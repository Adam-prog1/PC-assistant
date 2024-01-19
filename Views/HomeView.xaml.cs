using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Management;
using System.Windows;
using System.Windows.Controls;


namespace PC_assistant.Views
{
    /// <summary>
    /// Логика взаимодействия для HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();

            // Заполняем список характеристик при инициализации окна
            //Loaded += MainWindow_Loaded;
        }

        //// Код для вывода характеристик
        //private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    // Получение и отображение характеристик компьютера
        //    string processorInfo = GetProcessorInfo();
        //    string ramInfo = GetRAMInfo();
        //    string biosYearInfo = GetBiosYearInfo();
        //    string osInfo = GetOSInfo();

        //    // Форматирование строки и установка текста с символами новой строки
        //    SystemInfo.Text = $"Процессор: {processorInfo}\nОперативная память: {ramInfo}\nГод выпуска BIOS: {biosYearInfo}\nТекущая ОС: {osInfo}";
        //}

        //// Код для вывода характеристик процессора
        //private string GetProcessorInfo()
        //{
        //    try
        //    {
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
        //        foreach (ManagementObject obj in searcher.Get())
        //        {
        //            return obj["Name"].ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Ошибка при получении информации о процессоре: " + ex.ToString();
        //    }

        //    return "Информация о процессоре недоступна.";
        //}

        //// Код для вывода характеристик ОЗУ
        //private string GetRAMInfo()
        //{
        //    try
        //    {
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
        //        foreach (ManagementObject obj in searcher.Get())
        //        {
        //            ulong ramSize = Convert.ToUInt64(obj["TotalPhysicalMemory"]);
        //            return $"{ramSize / (1024 * 1024)} MB";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Ошибка при получении информации об оперативной памяти: " + ex.ToString();
        //    }

        //    return "Информация об оперативной памяти недоступна.";
        //}

        //// Код для вывода характеристик BIOS
        //private string GetBiosYearInfo()
        //{
        //    try
        //    {
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
        //        foreach (ManagementObject obj in searcher.Get())
        //        {
        //            return obj["ReleaseDate"].ToString().Substring(0, 4);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Ошибка при получении информации о годе выпуска BIOS: " + ex.ToString();
        //    }

        //    return "Информация о годе выпуска BIOS недоступна.";
        //}

        //// Код для вывода версии Windows
        //private string GetOSInfo()
        //{
        //    string osVersion = Environment.OSVersion.Version.ToString();
        //    string osDescription = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
        //    string osBuild = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", "").ToString();

        //    return $"{osDescription} {osVersion} build {osBuild}";
        //}
        

        // Код Ключ активации Windows
        private void ActivationKeyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    string subKeyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                    using (RegistryKey subKey = key.OpenSubKey(subKeyPath))
                    {
                        if (subKey != null)
                        {
                            if (subKey.GetValue("DigitalProductId") is byte[] digitalProductIdBytes)
                            {
                                string digitalProductId = BitConverter.ToString(digitalProductIdBytes).Replace("-", "");

                                string win10ProductName = "Версия Windows 10: " + subKey.GetValue("ProductName") + Environment.NewLine;
                                string win10ProductID = "ID продукта: " + subKey.GetValue("ProductID") + Environment.NewLine;
                                string win10ProductKey = ConvertToKey(digitalProductId);
                                string productKeyLabel = "Ключ Windows 10: " + win10ProductKey;

                                string result = win10ProductName + win10ProductID + productKeyLabel;
                                MessageBox.Show(result, "Ключ активации", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Ключ DigitalProductId не найден в реестре.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Не удалось открыть ключ {subKeyPath} в реестре.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Код для конвертации ключа активации
        private string ConvertToKey(string digitalProductId)
        {
            const int keyOffset = 52;
            bool isWin10 = (Convert.ToByte(digitalProductId.Substring(66, 2), 16) & 6) != 0;
            digitalProductId = digitalProductId.Substring(keyOffset);

            char[] chars = "BCDFGHJKMPQRTVWXY2346789".ToCharArray();
            string winKeyOutput = "";

            for (int j = 24; j >= 0; j--)
            {
                int cur = 0;
                for (int y = 14; y >= 0; y--)
                {
                    cur = cur * 256 + Convert.ToByte(digitalProductId.Substring(y + keyOffset, 1), 16);
                    digitalProductId = digitalProductId.Remove(y + keyOffset, 1).Insert(y + keyOffset, ((cur / 24) & 255).ToString("X"));
                    cur %= 24;
                }
                winKeyOutput = chars[cur] + winKeyOutput;
            }

            if (isWin10)
            {
                _ = winKeyOutput.Substring(1, winKeyOutput.LastIndexOf(winKeyOutput[0]) - 1);
                string insert = "N";
                winKeyOutput = winKeyOutput.Insert(2, insert);
                if (winKeyOutput.LastIndexOf(winKeyOutput[0]) == 0) winKeyOutput = insert + winKeyOutput;
            }

            string a = winKeyOutput.Substring(0, 5);
            string b = winKeyOutput.Substring(5, 5);
            string c = winKeyOutput.Substring(10, 5);
            string d = winKeyOutput.Substring(15, 5);
            string e = winKeyOutput.Substring(20, 5);

            return $"{a}-{b}-{c}-{d}-{e}";
        }

        // Код для Расширенных сведений о системе
        private void AdvancedSystemInfoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("msinfo32");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Код для Создания точки восстановления
        private void CreateRestorePointButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создание точки восстановления с помощтю PowerShell
                //Process.Start("powershell.exe", "-NoExit -Command checkpoint-computer -description 'Restore Point created by PC Assistant'");

                // Открыть окно создание точки восстановления (Защита системы) 
                Process.Start("control", "sysdm.cpl,,4");

                // Сразу открывает Восстановление системы
                //Process.Start("rstrui.exe");
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Ошибка при создании точки восстановления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show($"Ошибка при открытии окна Защита системы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
