﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PC_assistant.ViewModels;

namespace PC_assistant.Views
{
    /// <summary>
    /// Логика взаимодействия для ProgramsView.xaml
    /// </summary>
    public partial class ProgramsView : UserControl
    {
        private ProgramsViewModel viewModel;

        // Добавим свойства
        public ICommand GetInstalledProgramsCommand { get; set; }
        public IEnumerable InstalledPrograms { get; set; }

        public ProgramsView()
        {
            InitializeComponent();
            viewModel = new ProgramsViewModel();
            DataContext = viewModel;

            // Присвоим свойства из viewModel
            GetInstalledProgramsCommand = viewModel.GetInstalledProgramsCommand;
            InstalledPrograms = viewModel.InstalledPrograms;
        }

        // С асинхроном
        private async void GetInstalledPrograms_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.GetInstalledProgramsAsync();
        }

        ////Без асинхрона
        //private void GetInstalledPrograms_Click(object sender, RoutedEventArgs e)
        //{
        //    viewModel.GetInstalledPrograms(null);
        //}

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
                Process.Start("ms-settings:startupapps");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
