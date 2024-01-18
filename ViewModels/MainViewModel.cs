// MainViewModel.cs

using FontAwesome.Sharp;
using System.Windows.Input;

namespace PC_assistant.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly HomeViewModel _homeViewModel = new HomeViewModel();
        private readonly OptimizationViewModel _optimizationViewModel = new OptimizationViewModel();
        private readonly UpdatesViewModel _updatesViewModel = new UpdatesViewModel();
        private readonly ProgramsViewModel _programsViewModel = new ProgramsViewModel();
        private readonly DiskCleanupViewModel _diskCleanupViewModel = new DiskCleanupViewModel();

        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;

        public ViewModelBase CurrentChildView
        {
            get { return _currentChildView; }
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public IconChar Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public ICommand ShowHomeViewCommand { get; }
        public ICommand ShowOptimizationViewCommand { get; }
        public ICommand ShowUpdatesViewCommand { get; }
        public ICommand ShowProgramsViewCommand { get; }
        public ICommand ShowDiskCleanupViewCommand { get; }

        public MainViewModel()
        {
            ShowHomeViewCommand = new ViewModelCommand(ExecuteShowHomeViewCommand);
            ShowOptimizationViewCommand = new ViewModelCommand(ExecuteShowOptimizationViewCommand);
            ShowUpdatesViewCommand = new ViewModelCommand(ExecuteShowUpdatesViewCommand);
            ShowProgramsViewCommand = new ViewModelCommand(ExecuteShowProgramsViewCommand);
            ShowDiskCleanupViewCommand = new ViewModelCommand(ExecuteShowDiskCleanupViewCommand);

            // Default view
            ExecuteShowHomeViewCommand(null);
        }

        private void ExecuteShowHomeViewCommand(object obj)
        {
            CurrentChildView = _homeViewModel;
            Caption = "Home";
            Icon = IconChar.Home;
        }

        private void ExecuteShowOptimizationViewCommand(object obj)
        {
            CurrentChildView = _optimizationViewModel;
            Caption = "Оптимизация";
            Icon = IconChar.Desktop;
        }

        private void ExecuteShowUpdatesViewCommand(object obj)
        {
            CurrentChildView = _updatesViewModel;
            Caption = "Обновления";
            Icon = IconChar.CloudArrowUp;
        }

        private void ExecuteShowProgramsViewCommand(object obj)
        {
            CurrentChildView = _programsViewModel;
            Caption = "Программы";
            Icon = IconChar.Briefcase;
        }

        private void ExecuteShowDiskCleanupViewCommand(object obj)
        {
            CurrentChildView = _diskCleanupViewModel;
            Caption = "Очистка диска";
            Icon = IconChar.TrashCan;
        }
    }
}


//// MainViewModel.cs

//using FontAwesome.Sharp;
//using System.Collections.Generic;
//using System.Windows.Input;

//namespace PC_assistant.ViewModels
//{
//    public class MainViewModel : ViewModelBase
//    {
//        private readonly Dictionary<string, ViewModelBase> _viewModelCache = new Dictionary<string, ViewModelBase>();

//        private ViewModelBase _currentChildView;
//        private string _caption;
//        private IconChar _icon;

//        private readonly HomeViewModel _homeViewModel = new HomeViewModel();
//        private readonly OptimizationViewModel _optimizationViewModel = new OptimizationViewModel();
//        private readonly UpdatesViewModel _updatesViewModel = new UpdatesViewModel();
//        private readonly ProgramsViewModel _programsViewModel = new ProgramsViewModel();
//        private readonly DiskCleanupViewModel _diskCleanupViewModel = new DiskCleanupViewModel();

//        public ViewModelBase CurrentChildView
//        {
//            get { return _currentChildView; }
//            set
//            {
//                _currentChildView = value;
//                OnPropertyChanged(nameof(CurrentChildView));
//            }
//        }

//        public string Caption
//        {
//            get { return _caption; }
//            set
//            {
//                _caption = value;
//                OnPropertyChanged(nameof(Caption));
//            }
//        }

//        public IconChar Icon
//        {
//            get { return _icon; }
//            set
//            {
//                _icon = value;
//                OnPropertyChanged(nameof(Icon));
//            }
//        }

//        public ICommand ShowHomeViewCommand { get; }
//        public ICommand ShowOptimizationViewCommand { get; }
//        public ICommand ShowUpdatesViewCommand { get; }
//        public ICommand ShowProgramsViewCommand { get; }
//        public ICommand ShowDiskCleanupViewCommand { get; }

//        public MainViewModel()
//        {
//            ShowHomeViewCommand = new ViewModelCommand(ExecuteShowHomeViewCommand);
//            ShowOptimizationViewCommand = new ViewModelCommand(ExecuteShowOptimizationViewCommand);
//            ShowUpdatesViewCommand = new ViewModelCommand(ExecuteShowUpdatesViewCommand);
//            ShowProgramsViewCommand = new ViewModelCommand(ExecuteShowProgramsViewCommand);
//            ShowDiskCleanupViewCommand = new ViewModelCommand(ExecuteShowDiskCleanupViewCommand);

//            // Default view
//            ExecuteShowHomeViewCommand(null);
//        }

//        private void ExecuteShowHomeViewCommand(object obj)
//        {
//            SetCurrentChildView(_homeViewModel, "HomeView");
//            Caption = "Home";
//            Icon = IconChar.Home;
//        }

//        private void ExecuteShowOptimizationViewCommand(object obj)
//        {
//            SetCurrentChildView(_optimizationViewModel, "OptimizationView");
//            Caption = "Оптимизация";
//            Icon = IconChar.Desktop;
//        }

//        private void ExecuteShowUpdatesViewCommand(object obj)
//        {
//            SetCurrentChildView(_updatesViewModel, "UpdatesView");
//            Caption = "Обновления";
//            Icon = IconChar.CloudArrowUp;
//        }

//        private void ExecuteShowProgramsViewCommand(object obj)
//        {
//            SetCurrentChildView(_programsViewModel, "ProgramsView");
//            Caption = "Программы";
//            Icon = IconChar.Briefcase;
//        }

//        private void ExecuteShowDiskCleanupViewCommand(object obj)
//        {
//            SetCurrentChildView(_diskCleanupViewModel, "DiskCleanupView");
//            Caption = "Очистка диска";
//            Icon = IconChar.TrashCan;
//        }

//        private void SetCurrentChildView(ViewModelBase viewModel, string key)
//        {
//            if (!_viewModelCache.TryGetValue(key, out var cachedViewModel))
//            {
//                _viewModelCache[key] = viewModel;
//                cachedViewModel = viewModel;
//            }

//            CurrentChildView = cachedViewModel;
//        }
//    }
//}
