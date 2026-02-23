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
using Сourse_management.model;
using System.Data.Entity;

namespace Сourse_management.Admin_Pages
{
    /// <summary>
    /// Логика взаимодействия для Add_Lesson.xaml
    /// </summary>
    public partial class Add_Lesson : Page
    {
        private CoursesEntities _context = new CoursesEntities();
        private Lessons _currentLesson;

        public Add_Lesson()
        {
            InitializeComponent();
            LoadLessons();
            LoadCourses();
        }

        private void LoadLessons()
        {
            _context.Lessons.Load();
            LessonsComboBox.ItemsSource = _context.Lessons.Local;
        }

        private void LoadCourses()
        {
            _context.Courses.Load();
            CoursesComboBox.ItemsSource = _context.Courses.Local;
        }

        private void LessonsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentLesson = LessonsComboBox.SelectedItem as Lessons;

            if (_currentLesson != null)
            {
                LessonPartTextBox.Text = _currentLesson.Lessons_Part.ToString() ?? "";
                LessonNameTextBox.Text = _currentLesson.Lessons_Name;
                LessonContentTextBox.Text = _currentLesson.Lessons_Content;

                // Выбираем курс в ComboBox
                var selectedCourse = _context.Courses.FirstOrDefault(c => c.Courses_Id == _currentLesson.Courses_Id);
                CoursesComboBox.SelectedItem = selectedCourse;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LessonNameTextBox.Text))
                {
                    MessageBox.Show("Название урока обязательно для заполнения",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                    return;
                }

                var selectedCourse = CoursesComboBox.SelectedItem as Courses;
                if (selectedCourse == null)
                {
                    MessageBox.Show("Выберите курс",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                    return;
                }

                if (_currentLesson == null)
                {
                    _currentLesson = new Lessons();
                    _context.Lessons.Add(_currentLesson);
                }

                _currentLesson.Courses_Id = selectedCourse.Courses_Id;
                _currentLesson.Lessons_Part = int.Parse(LessonPartTextBox.Text);
                _currentLesson.Lessons_Name = LessonNameTextBox.Text;
                _currentLesson.Lessons_Content = LessonContentTextBox.Text;

                _context.SaveChanges();
                LoadLessons();
                MessageBox.Show("Урок сохранен успешно",
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
            _currentLesson = null;
            LessonsComboBox.SelectedIndex = -1;
            CoursesComboBox.SelectedIndex = -1;
            LessonPartTextBox.Text = "";
            LessonNameTextBox.Text = "";
            LessonContentTextBox.Text = "";
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentLesson == null)
            {
                MessageBox.Show("Выберите урок для удаления",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить урок '{_currentLesson.Lessons_Name}'?",
                                       "Подтверждение удаления",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Lessons.Remove(_currentLesson);
                    _context.SaveChanges();
                    LoadLessons();
                    NewButton_Click(null, null);
                    MessageBox.Show("Урок удален успешно",
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
