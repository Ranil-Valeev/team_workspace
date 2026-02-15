using HelloCode.Commands;
using Сourse_management.ViewModels;

namespace HelloCode.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        // Команды навигации
        public RelayCommand GoToStartCommand { get; }
        public RelayCommand GoToLoginCommand { get; }
        public RelayCommand GoToRegisterCommand { get; }

        public MainViewModel()
        {
            GoToStartCommand = new RelayCommand(_ => CurrentViewModel = new StartViewModel(this));
            GoToLoginCommand = new RelayCommand(_ => CurrentViewModel = new LoginViewModel(this));
            GoToRegisterCommand = new RelayCommand(_ => CurrentViewModel = new RegisterViewModel(this));

            // Стартовый экран
            CurrentViewModel = new StartViewModel(this);
        }
    }
}
