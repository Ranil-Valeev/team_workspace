using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using Сourse_management.model;

namespace Сourse_management.Admin_Pages
{
    /// <summary>
    /// Логика взаимодействия для Add_Course.xaml
    /// </summary>
    public partial class Add_Course : Page
    {
        private CoursesEntities _context = new CoursesEntities();
        private Courses _currentCourse;

        public Add_Course()
        {
            InitializeComponent();
            LoadCourses();
            LoadLanguages();
        }

        private void LoadCourses()
        {
            _context.Courses.Load();
            CoursesComboBox.ItemsSource = _context.Courses.Local;
        }

        private void LoadLanguages()
        {
            //_context.language.Load();
            //LanguageComboBox.ItemsSource = _context.language.Local;
            _context.language.Load();
            LanguageComboBox.ItemsSource = _context.language.Local.ToBindingList();
        }

        private void CoursesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentCourse = CoursesComboBox.SelectedItem as Courses;

            if (_currentCourse != null)
            {
                CourseNameTextBox.Text = _currentCourse.Courses_Name;
                CourseDescriptionTextBox.Text = _currentCourse.Courses_Descriptions;
                CoursePartTextBox.Text = _currentCourse.Courses_Part?.ToString();

                // Выбираем язык в ComboBox
                var selectedLanguage = _context.language.FirstOrDefault(l => l.Language_Id == _currentCourse.Language_Id);
                LanguageComboBox.SelectedItem = selectedLanguage;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CourseNameTextBox.Text))
                {
                    MessageBox.Show("Название курса обязательно для заполнения",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                    return;
                }

                if (_currentCourse == null)
                {
                    _currentCourse = new Courses();
                    _context.Courses.Add(_currentCourse);
                }

                _currentCourse.Courses_Name = CourseNameTextBox.Text;
                _currentCourse.Courses_Descriptions = CourseDescriptionTextBox.Text;
                

                if (int.TryParse(CoursePartTextBox.Text, out int part))
                {
                    _currentCourse.Courses_Part = part;
                }

                // Устанавливаем выбранный язык
                var selectedLanguage = LanguageComboBox.SelectedItem as language;
                if (selectedLanguage != null)
                {
                    _currentCourse.Language_Id = selectedLanguage.Language_Id;
                }

                _context.SaveChanges();
                LoadCourses();
                MessageBox.Show("Курс сохранен успешно",
                              "Успех",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            _currentCourse = null;
            CoursesComboBox.SelectedIndex = -1;
            CourseNameTextBox.Text = "";
            CourseDescriptionTextBox.Text = "";
            CoursePartTextBox.Text = "";

            LanguageComboBox.SelectedIndex = -1;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCourse == null)
            {
                MessageBox.Show("Выберите курс для удаления",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить курс '{_currentCourse.Courses_Name}'?",
                                       "Подтверждение удаления",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Courses.Remove(_currentCourse);
                    _context.SaveChanges();
                    LoadCourses();
                    NewButton_Click(null, null);
                    MessageBox.Show("Курс удален успешно",
                                  "Успех",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
        }
    }
}
