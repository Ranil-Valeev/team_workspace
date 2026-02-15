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
        private Frame _frame;
        public StartPage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new Pages.LoginPage(_frame));
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new Pages.RegisterPage(_frame));
        }

        private void Telegram_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://t.me/your_channel";

            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }
    }
}
