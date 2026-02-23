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

namespace Сourse_management.User_Pages
{
    /// <summary>
    /// Логика взаимодействия для UserProfilePage.xaml
    /// </summary>
    public partial class UserProfilePage : Page
    {
        public UserProfilePage()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (CurrentUser.User != null)
            {
                WelcomeTextBlock.Text = $"Добро пожаловать, {CurrentUser.User.Login}!";
                EmailTextBlock.Text = CurrentUser.User.Email ?? "Не указан";
                LoginTextBlock.Text = CurrentUser.User.Login ?? "Не указан";
            }

            if (CurrentUser.Student != null)
            {
                FullNameTextBlock.Text = $"{CurrentUser.Student.Student_SurName} {CurrentUser.Student.Student_Name} {CurrentUser.Student.Student_LastName}";
                PhoneTextBlock.Text = CurrentUser.Student.Student_Number ?? "Не указан";
                GenderTextBlock.Text = CurrentUser.Student.Student_Gender ?? "Не указан";

                BirthDateTextBlock.Text = ((DateTime)CurrentUser.Student.Student_Age).ToString("dd.MM.yyyy");
            }
        }
    }
}
