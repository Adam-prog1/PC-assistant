using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using PC_assistant.ViewModels;

namespace PC_assistant.Views
{
    /// <summary>
    /// Логика взаимодействия для DiskCleanupView.xaml
    /// </summary>
    public partial class DiskCleanupView : UserControl
    {
        public DiskCleanupView()
        {
            InitializeComponent();

            // Заполняем список временных файлов при инициализации окна
            PopulateDiskCleanupList();
        }

        // Очистка диска
        private void PopulateDiskCleanupList()
        {
            // Указываем путь к временным файлам на диске C:
            string tempFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");
            string windowsTempFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");

            // Получаем список файлов
            var tempFiles = Directory.GetFiles(tempFolderPath).Where(File.Exists).Select(filePath => new { Path = filePath, Size = new FileInfo(filePath).Length });
            var windowsTempFiles = Directory.GetFiles(windowsTempFolderPath).Where(File.Exists).Select(filePath => new { Path = filePath, Size = new FileInfo(filePath).Length });

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
            // Указываем путь к временным файлам на диске C:
            string tempFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");
            string windowsTempFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");

            // Получаем список файлов
            var tempFiles = Directory.GetFiles(tempFolderPath).Select(file => new FileInfo(file).Length);
            var windowsTempFiles = Directory.GetFiles(windowsTempFolderPath).Select(file => new FileInfo(file).Length);

            // Суммируем размеры всех файлов
            long totalSize = tempFiles.Concat(windowsTempFiles).Sum();

            return totalSize;
        }

        // Обработчик события для кнопки "Удалить выбранные" _1
        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in DiskCleanupListBox.Items)
                {
                    if (item is CheckBox checkBox && checkBox.IsChecked == true)
                    {
                        // Получаем путь к файлу из Tag
                        string filePath = (string)checkBox.Tag;

                        DeleteFileSafely(filePath);
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

        // Обработчик события для пропуска вайлов используемых в других процессах _1
        private void DeleteFileSafely(string filePath)
        {
            try
            {
                // Попытка удаления файла
                File.Delete(filePath);
            }
            catch (IOException)
            {
                // Обработка ошибок IOException (файл заблокирован другим процессом)
                Trace.WriteLine($"Файл {filePath} заблокирован другим процессом. Пропущен.");
            }
            catch (Exception ex)
            {
                // Обработка других ошибок удаления файла
                Trace.WriteLine($"Ошибка при удалении файла {filePath}: {ex.Message}");
            }
        }

        // Обработчик события для кнопки "Удалить выбранные" с пропуском файлов используемых в других процессах
        //private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        foreach (var item in DiskCleanupListBox.Items)
        //        {
        //            if (item is CheckBox checkBox && checkBox.IsChecked == true)
        //            {
        //                // Получаем путь к файлу из Tag
        //                string filePath = (string)checkBox.Tag;

        //                try
        //                {
        //                    // Попытка удаления файла
        //                    File.Delete(filePath);
        //                }
        //                catch (IOException ex)
        //                {
        //                    // Обработка ошибок IOException (файл заблокирован другим процессом)
        //                    Console.WriteLine($"Файл {filePath} заблокирован другим процессом. Пропущен.");
        //                }
        //                catch (Exception ex)
        //                {
        //                    // Обработка других ошибок удаления файла
        //                    MessageBox.Show($"Ошибка при удалении файла {filePath}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //                }
        //            }
        //        }

        //        // Очищаем ListBox после удаления файлов
        //        DiskCleanupListBox.Items.Clear();

        //        // Заполняем ListBox заново после удаления файлов
        //        PopulateDiskCleanupList();

        //        MessageBox.Show("Выбранные файлы успешно удалены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        // Обработчик события для кнопки "Удалить выбранные"
        //private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        foreach (var item in DiskCleanupListBox.Items)
        //        {
        //            if (item is CheckBox checkBox && checkBox.IsChecked == true)
        //            {
        //                // Получаем путь к файлу из Tag
        //                string filePath = (string)checkBox.Tag;

        //                try
        //                {
        //                    // Попытка удаления файла
        //                    File.Delete(filePath);
        //                }
        //                catch (Exception ex)
        //                {
        //                    // Обработка ошибок удаления файла
        //                    MessageBox.Show($"Ошибка при удалении файла {filePath}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //                }
        //            }
        //        }

        //        // Очищаем ListBox после удаления файлов
        //        DiskCleanupListBox.Items.Clear();

        //        // Заполняем ListBox заново после удаления файлов
        //        PopulateDiskCleanupList();

        //        MessageBox.Show("Выбранные файлы успешно удалены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}



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
