using HelloCode.Commands;
using System.Diagnostics;

namespace HelloCode.ViewModels
{
    public class StartViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;

        public RelayCommand LoginCommand { get; }
        public RelayCommand RegisterCommand { get; }
        public RelayCommand TelegramCommand { get; }

        public StartViewModel(MainViewModel main)
        {
            _main = main;

            LoginCommand = new RelayCommand(_ => _main.GoToLoginCommand.Execute(null));
            RegisterCommand = new RelayCommand(_ => _main.GoToRegisterCommand.Execute(null));

            TelegramCommand = new RelayCommand(_ =>
            {
                string url = "https://t.me/your_channel";

                Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });
            });
        }
    }
}
