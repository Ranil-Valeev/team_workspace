using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Сourse_management.Pages
{
    /// <summary>
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {

        public StartPage()
        {
            InitializeComponent();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.LoginPage());
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.RegisterPage());
        }

        private void Telegram_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://t.me/exicdzn";

            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }
    }
}
