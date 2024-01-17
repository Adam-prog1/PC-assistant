using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PC_assistant.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        //Fields
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;

        //Properties
        public ViewModelBase CurrentChildView
        {
            get
            {
                return _currentChildView;
            }

            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }

        public string Caption
        {
            get
            {
                return _caption;
            }

            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public IconChar Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        //--> Commands
        public ICommand ShowHomeViewCommand { get; }
        public ICommand ShowOptimizationViewCommand { get; }
        public ICommand ShowDiskCleanupViewCommand { get; }

        public MainViewModel()
        {

            //Initialize commands
            ShowHomeViewCommand = new ViewModelCommand(ExecuteShowHomeViewCommand);
            ShowOptimizationViewCommand = new ViewModelCommand(ExecuteShowOptimizationViewCommand);
            ShowDiskCleanupViewCommand = new ViewModelCommand(ExecuteShowDiskCleanupViewCommand);

            //Default view
            ExecuteShowDiskCleanupViewCommand(null);
        }

        private void ExecuteShowHomeViewCommand(object obj)
        {
            CurrentChildView = new HomeViewModel();
            Caption = "Home";
            Icon = IconChar.Home;
        }

        private void ExecuteShowOptimizationViewCommand(object obj)
        {
            CurrentChildView = new OptimizationViewModel();
            Caption = "Optimization";
            Icon = IconChar.UserGroup;
        }

        private void ExecuteShowDiskCleanupViewCommand(object obj)
        {
            CurrentChildView = new DiskCleanupViewModel();
            Caption = "Очистка диска";
            Icon = IconChar.Home;
        }
    }
}
