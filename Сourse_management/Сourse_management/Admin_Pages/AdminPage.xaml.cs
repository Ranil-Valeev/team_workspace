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

namespace Сourse_management.Admin_Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            MainFrame2.Navigate(new WelcomePage());
        }
        private void Courses_Click(object sender, RoutedEventArgs e)
        {
            MainFrame2.Navigate(new Add_Course());
        }

        private void Lessons_Click(object sender, RoutedEventArgs e)
        {
            MainFrame2.Navigate(new Add_Lesson());
        }

        private void Tasks_Click(object sender, RoutedEventArgs e)
        {
            MainFrame2.Navigate(new Add_Task());
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginPage());
        }
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
