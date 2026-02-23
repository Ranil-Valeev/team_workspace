using System;
using System.Collections.Generic;
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
using Сourse_management.Pages;

namespace Сourse_management.User_Pages
{
    /// <summary>
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
            UserContentFrame.Navigate(new UserWelcomePage());
        }
        //private void Logout_Click(object sender, RoutedEventArgs e)
        //{
        //    CurrentUser.Logout();
        //    NavigationService?.Navigate(new LoginPage());
        //}
        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            UserContentFrame.Navigate(new UserProfilePage());
        }

        private void Articles_Click(object sender, RoutedEventArgs e)
        {
            UserContentFrame.Navigate(new ArticlesPage());
        }

        private void Example2_Click(object sender, RoutedEventArgs e)
        {
            // UserContentFrame.Navigate(new Example2Page());
            MessageBox.Show("Страница в разработке", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Example3_Click(object sender, RoutedEventArgs e)
        {
            // UserContentFrame.Navigate(new Example3Page());
            MessageBox.Show("Страница в разработке", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Example4_Click(object sender, RoutedEventArgs e)
        {
            // UserContentFrame.Navigate(new Example4Page());
            MessageBox.Show("Страница в разработке", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Example5_Click(object sender, RoutedEventArgs e)
        {
            // UserContentFrame.Navigate(new Example5Page());
            MessageBox.Show("Страница в разработке", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?",
                                        "Выход",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                CurrentUser.Logout();
                NavigationService?.Navigate(new LoginPage());
            }
        }

        // Управление окном
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
                Application.Current.Shutdown();
       
        }
    }
}
