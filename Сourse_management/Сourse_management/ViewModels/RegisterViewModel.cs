using HelloCode.Commands;

namespace HelloCode.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;

        public RelayCommand BackCommand { get; }
        public RelayCommand RegisterCommand { get; }

        private string _login;
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public RegisterViewModel(MainViewModel main)
        {
            _main = main;

            BackCommand = new RelayCommand(_ => _main.GoToStartCommand.Execute(null));

            RegisterCommand = new RelayCommand(_ =>
            {
                // TODO: логика регистрации
            });
        }
    }
}
