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
    /// Логика взаимодействия для CoursesListPage.xaml
    /// </summary>
    public partial class CoursesListPage : Page
    {
        private CoursesEntities _context = new CoursesEntities();

        public CoursesListPage()
        {
            InitializeComponent();
            LoadAvailableCourses();
        }

        private void LoadAvailableCourses()
        {
            try
            {
                if (CurrentUser.Student == null)
                {
                    MessageBox.Show("Ошибка: данные студента не загружены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получаем все курсы
                var allCourses = _context.Courses.Include("language").ToList();

                // Получаем ID курсов, на которые студент уже записан
                var enrolledCoursesIds = _context.Student_Courses
                    .Where(sc => sc.Student_Id == CurrentUser.Student.Student_Id)
                    .Select(sc => sc.Courses_Id)
                    .ToList();

                // Фильтруем курсы - показываем только те, на которые студент НЕ записан
                var availableCourses = allCourses
                    .Where(c => !enrolledCoursesIds.Contains(c.Courses_Id))
                    .ToList();

                if (availableCourses.Any())
                {
                    CoursesItemsControl.ItemsSource = availableCourses;
                    NoCoursesTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    CoursesItemsControl.ItemsSource = null;
                    NoCoursesTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке курсов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartCourse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null || button.Tag == null) return;

                int courseId = (int)button.Tag;

                // Убрано подтверждение - сразу записываем на курс
                EnrollInCourse(courseId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EnrollInCourse(int courseId)
        {
            try
            {
                // Получаем ID статуса "В процессе" (предполагается что это ID = 1)
                int inProgressStatusId = 1; // Измените на правильный ID из вашей таблицы Courses_Status

                // Проверяем, не записан ли студент уже на этот курс
                var existingEnrollment = _context.Student_Courses
                    .FirstOrDefault(sc => sc.Student_Id == CurrentUser.Student.Student_Id && sc.Courses_Id == courseId);

                if (existingEnrollment != null)
                {
                    MessageBox.Show("Вы уже записаны на этот курс!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // 1. Добавляем запись в Student_Courses
                var studentCourse = new Student_Courses
                {
                    Student_Id = CurrentUser.Student.Student_Id,
                    Courses_Id = courseId,
                    Status_Id = inProgressStatusId
                };

                _context.Student_Courses.Add(studentCourse);
                _context.SaveChanges();

                // 2. Получаем все уроки этого курса
                var courseLessons = _context.Lessons
                    .Where(l => l.Courses_Id == courseId)
                    .ToList();

                // 3. Добавляем все уроки в Student_Lessons со статусом "В процессе"
                foreach (var lesson in courseLessons)
                {
                    var studentLesson = new Student_Lessons
                    {
                        Student_Courses_Id = studentCourse.Student_Courses_Id,
                        Lessons_Id = lesson.Lessons_Id,
                        Status_Id = inProgressStatusId
                    };

                    _context.Student_Lessons.Add(studentLesson);
                }

                _context.SaveChanges();

                MessageBox.Show("Вы успешно записались на курс! Уроки добавлены в ваш список.",
                              "Успех",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);

                // Обновляем список доступных курсов
                LoadAvailableCourses();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при записи на курс: {ex.Message}\n\nДетали: {ex.InnerException?.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
    }
}
