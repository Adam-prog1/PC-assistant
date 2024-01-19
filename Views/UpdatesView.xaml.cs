using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

using System.Windows.Controls;

namespace PC_assistant.Views
{
    /// <summary>
    /// Логика взаимодействия для UpdatesView.xaml
    /// </summary>
    public partial class UpdatesView : UserControl
    {
        public UpdatesView()
        {
            InitializeComponent();
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
    }
}
