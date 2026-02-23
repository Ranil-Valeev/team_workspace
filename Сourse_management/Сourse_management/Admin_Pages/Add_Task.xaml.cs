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
    /// Логика взаимодействия для Add_Task.xaml
    /// </summary>
    public partial class Add_Task : Page
    {
        private CoursesEntities _context = new CoursesEntities();
        private Tasks _currentTask;

        public Add_Task()
        {
            InitializeComponent();
            LoadTasks();
            LoadLessons();
        }

        private void LoadTasks()
        {
            _context.Tasks.Load();
            TasksComboBox.ItemsSource = _context.Tasks.Local;
        }

        private void LoadLessons()
        {
            _context.Lessons.Load();
            LessonsComboBox.ItemsSource = _context.Lessons.Local;
        }

        private void TasksComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentTask = TasksComboBox.SelectedItem as Tasks;

            if (_currentTask != null)
            {
                TaskNameTextBox.Text = _currentTask.Tasks_Name;
                TaskTextTextBox.Text = _currentTask.Tasks_Text;
                TaskAnswerTextBox.Text = _currentTask.Tasks_Answer;
                TaskNumberTextBox.Text = _currentTask.Tasks_Number.ToString();

                // Выбираем урок в ComboBox
                var selectedLesson = _context.Lessons.FirstOrDefault(l => l.Lessons_Id == _currentTask.Lessons_Id);
                LessonsComboBox.SelectedItem = selectedLesson;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TaskNameTextBox.Text))
                {
                    MessageBox.Show("Название задания обязательно для заполнения",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                    return;
                }

                var selectedLesson = LessonsComboBox.SelectedItem as Lessons;
                if (selectedLesson == null)
                {
                    MessageBox.Show("Выберите урок",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                    return;
                }

                if (_currentTask == null)
                {
                    _currentTask = new Tasks();
                    _context.Tasks.Add(_currentTask);
                }

                _currentTask.Lessons_Id = selectedLesson.Lessons_Id;
                _currentTask.Tasks_Name = TaskNameTextBox.Text;
                _currentTask.Tasks_Text = TaskTextTextBox.Text;
                _currentTask.Tasks_Answer = TaskAnswerTextBox.Text;

                if (int.TryParse(TaskNumberTextBox.Text, out int taskNumber))
                {
                    _currentTask.Tasks_Number = taskNumber;
                }

                _context.SaveChanges();
                LoadTasks();
                MessageBox.Show("Задание сохранено успешно",
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
            _currentTask = null;
            TasksComboBox.SelectedIndex = -1;
            LessonsComboBox.SelectedIndex = -1;
            TaskNameTextBox.Text = "";
            TaskTextTextBox.Text = "";
            TaskAnswerTextBox.Text = "";
            TaskNumberTextBox.Text = "";
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTask == null)
            {
                MessageBox.Show("Выберите задание для удаления",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить задание '{_currentTask.Tasks_Name}'?",
                                       "Подтверждение удаления",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Tasks.Remove(_currentTask);
                    _context.SaveChanges();
                    LoadTasks();
                    NewButton_Click(null, null);
                    MessageBox.Show("Задание удалено успешно",
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
