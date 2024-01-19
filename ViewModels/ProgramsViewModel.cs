using System;
using System.Diagnostics;
using System.Management;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PC_assistant.ViewModels
{
    public class ProgramsViewModel : ViewModelBase
    {
        private ObservableCollection<string> installedPrograms;

        public ObservableCollection<string> InstalledPrograms
        {
            get { return installedPrograms; }
            set
            {
                if (installedPrograms != value)
                {
                    installedPrograms = value;
                    OnPropertyChanged(nameof(InstalledPrograms));
                }
            }
        }

        public ICommand GetInstalledProgramsCommand { get; }
        public ICommand UninstallProgramCommand { get; }
        public ICommand UpdateProgramsListCommand { get; }
        public ICommand OpenStartupAppsSettingsCommand { get; }

        public ProgramsViewModel()
        {
            InstalledPrograms = new ObservableCollection<string>();

            GetInstalledProgramsCommand = new ViewModelCommand(GetInstalledPrograms);
            UninstallProgramCommand = new ViewModelCommand(UninstallProgram);
            UpdateProgramsListCommand = new ViewModelCommand(UpdateProgramsList);
            OpenStartupAppsSettingsCommand = new ViewModelCommand(OpenStartupAppsSettings);
        }

        public void GetInstalledPrograms(object parameter)
        {
            // Логика получения установленных программ
            InstalledPrograms.Clear();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject obj in collection)
                {
                    string programName = obj["Name"] as string;
                    if (!string.IsNullOrEmpty(programName))
                    {
                        InstalledPrograms.Add(programName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UninstallProgram(object parameter)
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

        public void UpdateProgramsList(object parameter)
        {
            GetInstalledPrograms(null); // null передается вместо параметра
        }

        public void OpenStartupAppsSettings(object parameter)
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
    }
}
