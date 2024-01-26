using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using IWshRuntimeLibrary;
using System.Windows.Controls;


namespace PC_assistant.Views
{
    /// <summary>
    /// Логика взаимодействия для OptimizationView.xaml
    /// </summary>
    public partial class OptimizationView : UserControl
    {
        public OptimizationView()
        {
            InitializeComponent();
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
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.Save();
        }

        // Код для настройки даты и времени
        private void AddSecondsToClock_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("control", "timedate.cpl");
        }

        // Код для открытия параметров значков рабочего стола
        private void DesktopIconSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("control", "desk.cpl,,0");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
