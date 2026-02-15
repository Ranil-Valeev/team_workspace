using HelloCode.Commands;

namespace HelloCode.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;

        public RelayCommand BackCommand { get; }
        public RelayCommand LoginCommand { get; }

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

        public LoginViewModel(MainViewModel main)
        {
            _main = main;

            BackCommand = new RelayCommand(_ => _main.GoToStartCommand.Execute(null));

            LoginCommand = new RelayCommand(_ =>
            {
                // TODO: логика входа
            });
        }
    }
}
