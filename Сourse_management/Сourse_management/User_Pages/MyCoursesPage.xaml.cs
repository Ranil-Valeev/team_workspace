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
    /// Логика взаимодействия для MyCoursesPage.xaml
    /// </summary>
    public partial class MyCoursesPage : Page
    {
        private CoursesEntities _context = new CoursesEntities();

        public MyCoursesPage()
        {
            InitializeComponent();
            LoadMyCourses();
        }

        private void LoadMyCourses()
        {
            try
            {
                if (CurrentUser.Student == null)
                {
                    MessageBox.Show("Ошибка: данные студента не загружены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получаем курсы студента с включением связанных данных
                var myCourses = _context.Student_Courses
                    .Include("Courses")
                    .Include("Courses_Status")
                    .Where(sc => sc.Student_Id == CurrentUser.Student.Student_Id)
                    .ToList();

                if (myCourses.Any())
                {
                    MyCoursesItemsControl.ItemsSource = myCourses;
                    NoMyCoursesTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MyCoursesItemsControl.ItemsSource = null;
                    NoMyCoursesTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке курсов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenCourse_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null || button.Tag == null) return;

            int courseId = (int)button.Tag;

            NavigationService?.Navigate(new CourseDetailPage(courseId));
        }
    }
}
