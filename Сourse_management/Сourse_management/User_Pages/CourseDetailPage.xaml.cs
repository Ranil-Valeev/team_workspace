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

namespace Сourse_management.User_Pages
{
    /// <summary>
    /// Логика взаимодействия для CourseDetailPage.xaml
    /// </summary>
    public partial class CourseDetailPage : Page
    {
        private CoursesEntities _context = new CoursesEntities();
        private int _courseId;
        private int _studentCoursesId;

        public CourseDetailPage(int courseId)
        {
            InitializeComponent();
            _courseId = courseId;
            LoadCourseDetails();
            LoadCourseLessons();
        }

        private void LoadCourseDetails()
        {
            try
            {
                // Получаем информацию о курсе
                var course = _context.Courses.Include("language").FirstOrDefault(c => c.Courses_Id == _courseId);

                if (course != null)
                {
                    CourseNameTextBlock.Text = course.Courses_Name;
                    CourseDescriptionTextBlock.Text = course.Courses_Descriptions ?? "Описание отсутствует";
                    CoursePartTextBlock.Text = course.Courses_Part?.ToString() ?? "Не указано";
                    CourseLanguageTextBlock.Text = course.language?.Language1 ?? "Не указан";
                }

                // Получаем статус курса для студента
                var studentCourse = _context.Student_Courses
                    .Include("Courses_Status")
                    .FirstOrDefault(sc => sc.Student_Id == CurrentUser.Student.Student_Id && sc.Courses_Id == _courseId);

                if (studentCourse != null)
                {
                    _studentCoursesId = studentCourse.Student_Courses_Id;
                    CourseStatusTextBlock.Text = studentCourse.Courses_Status?.Status ?? "Не указан";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке курса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCourseLessons()
        {
            try
            {
                // Получаем уроки студента для этого курса
                var studentLessons = _context.Student_Lessons
                    .Include("Lessons")
                    .Include("Courses_Status")
                    .Where(sl => sl.Student_Courses.Student_Id == CurrentUser.Student.Student_Id &&
                                 sl.Student_Courses.Courses_Id == _courseId)
                    .OrderBy(sl => sl.Lessons.Lessons_Part)
                    .ToList();

                if (studentLessons.Any())
                {
                    LessonsItemsControl.ItemsSource = studentLessons;
                    NoLessonsTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    LessonsItemsControl.ItemsSource = null;
                    NoLessonsTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке уроков: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Lesson_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var border = sender as Border;
                if (border == null || border.Tag == null) return;

                int lessonId = (int)border.Tag;

                // Переход на страницу урока
                NavigationService?.Navigate(new LessonPage(lessonId, _studentCoursesId));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
