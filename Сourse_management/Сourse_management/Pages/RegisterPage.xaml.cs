using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
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
using Сourse_management.model;

namespace Сourse_management.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        private readonly Regex passwordRegex = new Regex(@"^(?=.*[a-zа-я])(?=.*[A-ZА-Я])(?=.*\d)(?=.*[@$!%*?&/.,#]).{8,}$");

        public RegisterPage()
        {
            InitializeComponent();
            PasswordBox1.PasswordChanged += PasswordBox_PasswordChanged;
            PasswordBox2.PasswordChanged += PasswordBox_PasswordChanged;

        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password1 = PasswordBox1.Password;
            string password2 = PasswordBox2.Password;

            // Проверка первого пароля на соответствие требованиям
            if (!string.IsNullOrEmpty(password1) && !passwordRegex.IsMatch(password1))
            {
                PasswordErrorLabel.Content = "Пароль должен содержать минимум 8 символов, заглавные и строчные буквы, цифры и спецсимволы (@$!%*?&/.,#)";
                PasswordErrorLabel.Visibility = Visibility.Visible;
                return;
            }

            // Если оба поля заполнены
            if (!string.IsNullOrEmpty(password1) && !string.IsNullOrEmpty(password2))
            {
                // Проверяем совпадение
                if (password1 != password2)
                {
                    PasswordErrorLabel.Content = "Пароли не совпадают!";
                    PasswordErrorLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    PasswordErrorLabel.Content = "";
                    PasswordErrorLabel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                // Если одно из полей пустое, скрываем ошибку
                PasswordErrorLabel.Visibility = Visibility.Collapsed;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем значения из полей
                string surname = TbSurname.Text.Trim();
                string name = TbName.Text.Trim();
                string lastName = TbLastName.Text.Trim();
                DateTime? birthDate = BirthDatePicker.SelectedDate;
                string gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                string email = TbEmail.Text.Trim();
                string phone = TbNumber.Text.Trim();
                string login = TbLogin.Text.Trim();
                string password1 = PasswordBox1.Password;
                string password2 = PasswordBox2.Password;

                // Проверка на пустые поля
                if (string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(name) ||
                    string.IsNullOrEmpty(lastName) || birthDate == null ||
                    string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(login) ||
                    string.IsNullOrEmpty(password1) || string.IsNullOrEmpty(password2))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка пароля на соответствие требованиям
                if (!passwordRegex.IsMatch(password1))
                {
                    PasswordErrorLabel.Content = "Пароль должен содержать минимум 8 символов, заглавные и строчные буквы, цифры и спецсимволы (@$!%*?&/.,#)";
                    PasswordErrorLabel.Visibility = Visibility.Visible;
                    MessageBox.Show("Пароль не соответствует требованиям безопасности!\n\n" +
                                   "Требования:\n" +
                                   "- Минимум 8 символов\n" +
                                   "- Заглавные буквы (A-Z или А-Я)\n" +
                                   "- Строчные буквы (a-z или а-я)\n" +
                                   "- Цифры (0-9)\n" +
                                   "- Спецсимволы (@$!%*?&/.,#)",
                                   "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка на совпадение паролей
                if (password1 != password2)
                {
                    PasswordErrorLabel.Content = "Пароли не совпадают!";
                    PasswordErrorLabel.Visibility = Visibility.Visible;
                    MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка возраста (должен быть старше 10 лет)
                int age = DateTime.Now.Year - birthDate.Value.Year;
                if (age < 10 || age > 120)
                {
                    MessageBox.Show("Проверьте правильность даты рождения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                using (var context = new CoursesEntities())
                {
                    // Проверяем, существует ли уже пользователь с таким логином
                    var existingUser = context.Users.FirstOrDefault(u => u.Login == login);
                    if (existingUser != null)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Проверяем, существует ли пользователь с таким email
                    var existingEmail = context.Users.FirstOrDefault(u => u.Email == email);
                    if (existingEmail != null)
                    {
                        MessageBox.Show("Пользователь с таким email уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // ШАГ 1: Создаем нового пользователя (Users)
                    var newUser = new Users
                    {
                        Role_Id = 1, // 1 - User, 2 - Admin
                        Login = login,
                        Password = password1,
                        Email = email
                    };

                    // Добавляем пользователя в контекст
                    context.Users.Add(newUser);

                    // СОХРАНЯЕМ, чтобы получить Users_Id
                    context.SaveChanges();

                    // ШАГ 2: Теперь у newUser.Users_Id есть значение из базы данных
                    int userId = newUser.Users_Id;

                    // ШАГ 3: Создаем студента и передаем ID пользователя
                    var newStudent = new Student
                    {
                        Student_Id = userId, // Связь один к одному: Student_Id = Users_Id
                        Student_SurName = surname,
                        Student_Name = name,
                        Student_LastName = lastName,
                        Student_Age = birthDate.Value,
                        Student_Gender = gender,
                        Student_Number = phone
                    };

                    // Добавляем студента
                    context.Student.Add(newStudent);
                    context.SaveChanges();

                    MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Переход на страницу входа или главную страницу
                    NavigationService?.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}\n\nПодробности: {ex.InnerException?.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
