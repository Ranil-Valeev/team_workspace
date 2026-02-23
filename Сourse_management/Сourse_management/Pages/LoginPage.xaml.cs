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
using Сourse_management.Admin_Pages;
using Сourse_management.model;
using Сourse_management.User_Pages;

namespace Сourse_management.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {

        public LoginPage()
        {
            InitializeComponent();

        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new CoursesEntities())
                {
                    var user = db.Users.FirstOrDefault(u => u.Login == TbLogin.Text && u.Password == PbPassword.Password);

                    if (user == null)
                    {
                        MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    } 

                    if (user.Role_Id == 1)
                    {
                        CurrentUser.User = user;
                        CurrentUser.Student = db.Student.FirstOrDefault(s => s.Student_Id == user.Users_Id);
                        NavigationService?.Navigate(new UserPage());
                    }
                    else if (user.Role_Id == 2)
                    {
                        NavigationService?.Navigate(new AdminPage());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
