// MainWindow.xaml.cs
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Windows;
using IWshRuntimeLibrary;
using System.Linq;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace PC_assistant
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Заполняем список характеристик при инициализации окна
            Loaded += MainWindow_Loaded;

            // Заполняем список кеш-файлов и временных файлов при инициализации окна
            PopulateDiskCleanupList();
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

        // Код для Создания точки восстановления с помощью powershell
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

        // Код для добавления корзины на рабочей стол
        private void AddBinToDesktopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string binShortcutPath = Path.Combine(desktopPath, "Корзина.lnk");

                if (!System.IO.File.Exists(binShortcutPath))
                {
                    CreateShortcut(binShortcutPath, "::{645FF040-5081-101B-9F08-00AA002F954E}");
                    MessageBox.Show("Корзина добавлена на рабочий стол.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Корзина уже присутствует на рабочем столе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Код для добавления Этот компьютер на рабочий стол
        private void AddComputerToDesktopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string computerShortcutPath = Path.Combine(desktopPath, "Этот компьютер.lnk");

                if (!System.IO.File.Exists(computerShortcutPath))
                {
                    CreateShortcut(computerShortcutPath, Environment.GetFolderPath(Environment.SpecialFolder.MyComputer));
                    MessageBox.Show("Этот компьютер добавлен на рабочий стол.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Этот компьютер уже присутствует на рабочем столе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Создание ярлыка для "Мой компьютер" и "Корзина"
        private void CreateShortcut(string shortcutPath, string targetPath)
        {
            // Создание ярлыка
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.Save();
        }

        // Код для добавления секунд в часы Windows
        private void AddSecondsToClock_Click(object sender, RoutedEventArgs e)
        {
            // Ваш код для добавления секунд в часы Windows
            Process.Start("control", "timedate.cpl");
            MessageBox.Show("Добавлены секунды в часы Windows. Пожалуйста, настройте отображение времени в окне \"Дата и время\".");
        }

        // Код для открытия параметров значков рабочего стола
        private void DesktopIconSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Запускаем команду для открытия параметров значков рабочего стола
                Process.Start("control", "desk.cpl,,0");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Кнопка "О программе"
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        // Код для отключения обновлений
        private void DisableWindowsUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Ваш код для отключения обновлений
            ExecuteCommand("reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\" /v AUOptions /t REG_DWORD /d 1 /f");
            MessageBox.Show("Windows Update отключено. Перезапустите компьютер для применения изменений.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Код для вкючения обновлений
        private void EnableWindowsUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Ваш код для включения обновлений
            ExecuteCommand("reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\" /v AUOptions /f");
            MessageBox.Show("Windows Update включено. Перезапустите компьютер для применения изменений.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteCommand(string command)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Verb = "runas"  // Запустить с правами администратора
                };

                Process process = new Process
                {
                    StartInfo = processStartInfo
                };

                process.Start();

                StreamWriter sw = process.StandardInput;
                StreamReader sr = process.StandardOutput;

                sw.WriteLine(command);
                sw.WriteLine("exit");

                string output = sr.ReadToEnd();

                process.WaitForExit();
                process.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Код для отображения установленных программ
        private void GetInstalledPrograms_Click(object sender, RoutedEventArgs e)
        {
            listBoxInstalledPrograms.Items.Clear();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject obj in collection)
                {
                    string programName = obj["Name"] as string;
                    if (!string.IsNullOrEmpty(programName))
                    {
                        listBoxInstalledPrograms.Items.Add(programName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Код для открытия окна "Программы и компоненты"
        private void UninstallProgram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("appwiz.cpl");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Код для обновления списка установленных программ
        private void UpdateProgram_Click(object sender, RoutedEventArgs e)
        {
            listBoxInstalledPrograms.Items.Clear();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject obj in collection)
                {
                    string programName = obj["Name"] as string;
                    if (!string.IsNullOrEmpty(programName))
                    {
                        listBoxInstalledPrograms.Items.Add(programName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Код для Автозапуска программ
        private void OpenStartupAppsSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("ms-settings:");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Очистка диска
        private void PopulateDiskCleanupList()
        {
            // Указываем путь к временным файлам и кешу на диске C:
            string tempFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");
            string windowsTempFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");

            // Получаем список файлов
            var tempFiles = Directory.GetFiles(tempFolderPath).Where(System.IO.File.Exists).Select(filePath => new { Path = filePath, Size = new FileInfo(filePath).Length });
            var windowsTempFiles = Directory.GetFiles(windowsTempFolderPath).Where(System.IO.File.Exists).Select(filePath => new { Path = filePath, Size = new FileInfo(filePath).Length });

            // Объединяем списки
            var allFiles = tempFiles.Concat(windowsTempFiles);

            // Заполняем ListBox
            foreach (var file in allFiles)
            {
                // Отображаем размер файла в Мб
                string fileSizeMb = $" ({file.Size / (1024 * 1024)} Mb)";

                // Создаем новый CheckBox с текстом, содержащим путь файла и его размер
                CheckBox checkBox = new CheckBox
                {
                    Content = file.Path + fileSizeMb,
                    Tag = file.Path, // Сохраняем путь к файлу в свойстве Tag для дальнейшего использования
                };

                // Добавляем обработчик события для CheckBox
                checkBox.Checked += CheckBox_Checked;
                checkBox.Unchecked += CheckBox_Unchecked;

                // Добавляем CheckBox в ListBox
                DiskCleanupListBox.Items.Add(checkBox);
            }
        }

        // Обновляет метку с общим размером выбранных файлов
        private void UpdateTotalSizeLabel()
        {
            long totalSize = 0;

            foreach (var item in DiskCleanupListBox.Items)
            {
                if (item is CheckBox checkBox && checkBox.IsChecked == true)
                {
                    string path = (string)checkBox.Tag;
                    long fileSize = new FileInfo(path).Length;
                    totalSize += fileSize;
                }
            }

            // Отображаем размер в мегабайтах
            TotalSizeLabel.Content = $"Общий размер выбранных файлов: {totalSize / (1024 * 1024)} Mb";
        }

        // Обработчик события для установки галочки
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Обновляем метку с общим размером
            UpdateTotalSizeLabel();
        }

        // Обработчик события для снятия галочки
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Обновляем метку с общим размером
            UpdateTotalSizeLabel();
        }

        // Метод для получения размера всех файлов
        private long GetTotalFileSize()
        {
            // Указываем путь к временным файлам и кешу на диске C:
            string tempFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");
            string windowsTempFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");

            // Получаем список файлов
            var tempFiles = Directory.GetFiles(tempFolderPath).Select(file => new FileInfo(file).Length);
            var windowsTempFiles = Directory.GetFiles(windowsTempFolderPath).Select(file => new FileInfo(file).Length);

            // Суммируем размеры всех файлов
            long totalSize = tempFiles.Concat(windowsTempFiles).Sum();

            return totalSize;
        }

        // Обработчик события для кнопки "Удалить выбранные"
        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var selectedItem in DiskCleanupListBox.SelectedItems)
                {
                    // Указываем путь к файлу
                    string filePath = Path.Combine(Path.GetTempPath(), selectedItem.ToString());

                    // Проверяем, существует ли файл перед удалением
                    if (System.IO.File.Exists(filePath))
                    {
                        // Удаляем файл
                        System.IO.File.Delete(filePath);
                    }
                }

                // Очищаем ListBox после удаления файлов
                DiskCleanupListBox.Items.Clear();

                // Заполняем ListBox заново после удаления файлов
                PopulateDiskCleanupList();

                MessageBox.Show("Выбранные файлы успешно удалены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик события для кнопки "Выбрать все"
        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in DiskCleanupListBox.Items)
            {
                if (item is CheckBox checkBox)
                {
                    checkBox.IsChecked = true;
                }
            }

            // Обновляем метку с общим размером
            UpdateTotalSizeLabel();
        }

        // Обработчик события для кнопки "Снять выбор"
        private void DeselectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in DiskCleanupListBox.Items)
            {
                if (item is CheckBox checkBox)
                {
                    checkBox.IsChecked = false;
                }
            }

            // Обновляем метку с общим размером
            UpdateTotalSizeLabel();
        }

        // Обработчик события для кнопки "Обновить"
        private void UpdateDiskCleanupListButton_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем ListBox
            DiskCleanupListBox.Items.Clear();

            // Заполняем ListBox заново
            PopulateDiskCleanupList();

            // Получаем и отображаем размер всей занимаемой памяти
            long totalSize = GetTotalFileSize();
            TotalSizeLabel.Content = $"Общий размер: {totalSize / (1024 * 1024)} Mb";
        }

        // Код для Оптимизации дисков
        private void OptimizeDrives_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string systemPath = Environment.Is64BitProcess ? Environment.SystemDirectory : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "sysnative");
                Process.Start(systemPath + @"\dfrgui.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при запуске оптимизации дисков: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Код для очистки диска
        private void RunDiskCleanupButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("cleanmgr.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при запуске Очистки диска: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}


