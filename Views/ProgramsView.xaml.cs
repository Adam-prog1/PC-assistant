using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using PC_assistant.ViewModels;

namespace PC_assistant.Views
{
    /// <summary>
    /// Логика взаимодействия для ProgramsView.xaml
    /// </summary>
    public partial class ProgramsView : UserControl
    {
        private ProgramsViewModel viewModel;

        public ProgramsView()
        {
            InitializeComponent();
            viewModel = new ProgramsViewModel();
            DataContext = viewModel;
        }

        private void GetInstalledPrograms_Click(object sender, RoutedEventArgs e)
        {
            viewModel.GetInstalledPrograms(null);
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
    }
}
