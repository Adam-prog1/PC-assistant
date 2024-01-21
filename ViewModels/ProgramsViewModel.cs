//using System;
//using System.Management;
//using System.Windows;
//using System.Collections.ObjectModel;
//using System.Windows.Input;

//namespace PC_assistant.ViewModels
//{
//    public class ProgramsViewModel : ViewModelBase
//    {
//        private ObservableCollection<string> installedPrograms;

//        public ObservableCollection<string> InstalledPrograms
//        {
//            get { return installedPrograms; }
//            set
//            {
//                if (installedPrograms != value)
//                {
//                    installedPrograms = value;
//                    OnPropertyChanged(nameof(InstalledPrograms));
//                }
//            }
//        }

//        public ICommand GetInstalledProgramsCommand { get; }

//        public ProgramsViewModel()
//        {
//            InstalledPrograms = new ObservableCollection<string>();

//            GetInstalledProgramsCommand = new ViewModelCommand(GetInstalledPrograms);
//        }

//        // Логика получения установленных программ
//        public void GetInstalledPrograms(object parameter)
//        {
//            InstalledPrograms.Clear();

//            try
//            {
//                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
//                ManagementObjectCollection collection = searcher.Get();

//                foreach (ManagementObject obj in collection)
//                {
//                    string programName = obj["Name"] as string;
//                    if (!string.IsNullOrEmpty(programName))
//                    {
//                        InstalledPrograms.Add(programName);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//    }
//}


using System;
using System.Collections.ObjectModel;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
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

        public ProgramsViewModel()
        {
            InstalledPrograms = new ObservableCollection<string>();

            GetInstalledProgramsCommand = new ViewModelCommand(async (_) => await GetInstalledProgramsAsync());
        }

        // Асинхронная логика получения установленных программ
        public async Task GetInstalledProgramsAsync()
        {
            InstalledPrograms.Clear();

            try
            {
                await Task.Run(() =>
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
                    ManagementObjectCollection collection = searcher.Get();

                    foreach (ManagementObject obj in collection)
                    {
                        string programName = obj["Name"] as string;
                        if (!string.IsNullOrEmpty(programName))
                        {
                            // Используем Dispatcher.Invoke для обновления коллекции из основного потока
                            Application.Current.Dispatcher.Invoke(() => InstalledPrograms.Add(programName));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
