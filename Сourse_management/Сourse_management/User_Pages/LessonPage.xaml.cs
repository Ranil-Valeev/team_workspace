using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
//using System.IO;
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
using IOPath = System.IO.Path;  // Создаем алиас для System.IO.Path
using IOFile = System.IO.File;  // Создаем алиас для System.IO.File

namespace Сourse_management.User_Pages
{
    /// <summary>
    /// Логика взаимодействия для LessonPage.xaml
    /// </summary>
    public partial class LessonPage : Page
    {
        private CoursesEntities _context = new CoursesEntities();
        private int _lessonId;
        private int _studentCoursesId;
        private Student_Lessons _studentLesson;
        private string _languageName;
        private List<int> _completedTasks = new List<int>();
        private int _totalTasks = 0;

        public LessonPage(int lessonId, int studentCoursesId)
        {
            InitializeComponent();
            _lessonId = lessonId;
            _studentCoursesId = studentCoursesId;
            LoadLessonData();
            LoadTasks();
            UpdateProgress();
        }

        private void LoadLessonData()
        {
            try
            {
                var lesson = _context.Lessons
                    .Include("Courses")
                    .Include("Courses.language")
                    .FirstOrDefault(l => l.Lessons_Id == _lessonId);

                if (lesson != null)
                {
                    LessonNameTextBlock.Text = lesson.Lessons_Name;
                    LessonPartTextBlock.Text = lesson.Lessons_Part.ToString();
                    LessonContentTextBlock.Text = lesson.Lessons_Content ?? "Содержание отсутствует";
                    _languageName = lesson.Courses?.language?.Language1 ?? "Python";
                    LanguageTextBlock.Text = _languageName;
                }

                _studentLesson = _context.Student_Lessons
                    .FirstOrDefault(sl => sl.Student_Courses_Id == _studentCoursesId && sl.Lessons_Id == _lessonId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке урока: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTasks()
        {
            try
            {
                var tasks = _context.Tasks
                    .Where(t => t.Lessons_Id == _lessonId)
                    .OrderBy(t => t.Tasks_Number)
                    .ToList();

                _totalTasks = tasks.Count;

                if (tasks.Any())
                {
                    TasksItemsControl.ItemsSource = tasks;
                    NoTasksTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TasksItemsControl.ItemsSource = null;
                    NoTasksTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заданий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RunCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null || button.Tag == null) return;

                var border = FindParent<Border>(button);
                var codeTextBox = FindChildByName<TextBox>(border, "CodeTextBox");
                var outputTextBlock = FindChildByName<TextBlock>(border, "OutputTextBlock");

                if (codeTextBox != null && outputTextBlock != null)
                {
                    string code = codeTextBox.Text;

                    if (string.IsNullOrWhiteSpace(code))
                    {
                        outputTextBlock.Text = "Ошибка: Введите код для выполнения!";
                        outputTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 100, 100));
                        return;
                    }

                    string output = ExecuteCode(code, _languageName);
                    outputTextBlock.Text = output;
                    outputTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(212, 212, 212));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении кода: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckSolution_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null || button.Tag == null) return;

                int taskId = (int)button.Tag;
                var task = _context.Tasks.FirstOrDefault(t => t.Tasks_Id == taskId);
                if (task == null) return;

                var border = FindParent<Border>(button);
                var codeTextBox = FindChildByName<TextBox>(border, "CodeTextBox");
                var outputTextBlock = FindChildByName<TextBlock>(border, "OutputTextBlock");
                var completedIndicator = FindChildByName<Border>(border, "CompletedIndicator");

                if (codeTextBox != null && outputTextBlock != null)
                {
                    string code = codeTextBox.Text;

                    if (string.IsNullOrWhiteSpace(code))
                    {
                        MessageBox.Show("Введите код перед проверкой!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string output = ExecuteCode(code, _languageName);
                    string expectedOutput = task.Tasks_Answer?.Trim() ?? "";
                    string actualOutput = output.Trim();

                    if (actualOutput.Equals(expectedOutput, StringComparison.Ordinal))
                    {
                        MessageBox.Show("✓ Отлично! Задание выполнено правильно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        if (!_completedTasks.Contains(taskId))
                        {
                            _completedTasks.Add(taskId);
                            UpdateProgress();
                        }

                        if (completedIndicator != null)
                        {
                            completedIndicator.Visibility = Visibility.Visible;
                        }

                        CheckIfAllTasksCompleted();
                    }
                    else
                    {
                        MessageBox.Show($"Вывод программы не совпадает с ожидаемым.\n\nОжидаемый вывод:\n{expectedOutput}\n\nВаш вывод:\n{actualOutput}",
                                      "Неверно",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке решения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ExecuteCode(string code, string language)
        {
            try
            {
                switch (language.ToLower())
                {
                    case "python":
                        return ExecutePython(code);
                    case "c#":
                    case "csharp":
                        return ExecuteCSharp(code);
                    case "javascript":
                    case "js":
                        return ExecuteJavaScript(code);
                    default:
                        return "Ошибка: Компиляция для языка " + language + " не поддерживается";
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка выполнения: {ex.Message}";
            }
        }

        private string ExecutePython(string code)
        {
            try
            {
                string tempFile = IOPath.Combine(IOPath.GetTempPath(), "temp_code.py");
                IOFile.WriteAllText(tempFile, code);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"{tempFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();
                    process.WaitForExit(5000);

                    try { IOFile.Delete(tempFile); } catch { }

                    if (!string.IsNullOrEmpty(errors))
                        return $"Ошибка:\n{errors}";

                    return output;
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка запуска Python: {ex.Message}\n\nУбедитесь, что Python установлен и доступен в PATH.";
            }
        }

        private string ExecuteCSharp(string code)
        {
            try
            {
                string fullCode = $@"
using System;

class Program
{{
    static void Main()
    {{
        {code}
    }}
}}";

                string tempFile = IOPath.Combine(IOPath.GetTempPath(), "temp_code.cs");
                string exeFile = IOPath.Combine(IOPath.GetTempPath(), "temp_code.exe");

                IOFile.WriteAllText(tempFile, fullCode);

                ProcessStartInfo compileInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe",
                    Arguments = $"/out:\"{exeFile}\" \"{tempFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process compileProcess = Process.Start(compileInfo))
                {
                    compileProcess.WaitForExit(5000);
                    string compileErrors = compileProcess.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(compileErrors) && compileErrors.Contains("error"))
                    {
                        try { IOFile.Delete(tempFile); IOFile.Delete(exeFile); } catch { }
                        return $"Ошибка компиляции:\n{compileErrors}";
                    }
                }

                ProcessStartInfo runInfo = new ProcessStartInfo
                {
                    FileName = exeFile,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process runProcess = Process.Start(runInfo))
                {
                    string output = runProcess.StandardOutput.ReadToEnd();
                    string errors = runProcess.StandardError.ReadToEnd();
                    runProcess.WaitForExit(5000);

                    try { IOFile.Delete(tempFile); IOFile.Delete(exeFile); } catch { }

                    if (!string.IsNullOrEmpty(errors))
                        return $"Ошибка выполнения:\n{errors}";

                    return output;
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка запуска C#: {ex.Message}\n\nУбедитесь, что .NET Framework установлен.";
            }
        }

        private string ExecuteJavaScript(string code)
        {
            try
            {
                string tempFile = IOPath.Combine(IOPath.GetTempPath(), "temp_code.js");
                IOFile.WriteAllText(tempFile, code);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "node",
                    Arguments = $"\"{tempFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();
                    process.WaitForExit(5000);

                    try { IOFile.Delete(tempFile); } catch { }

                    if (!string.IsNullOrEmpty(errors))
                        return $"Ошибка:\n{errors}";

                    return output;
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка запуска JavaScript: {ex.Message}\n\nУбедитесь, что Node.js установлен.";
            }
        }

        private void UpdateProgress()
        {
            ProgressTextBlock.Text = $"Прогресс: {_completedTasks.Count} из {_totalTasks} заданий выполнено";
        }

        private void CheckIfAllTasksCompleted()
        {
            if (_completedTasks.Count == _totalTasks && _totalTasks > 0)
            {
                try
                {
                    if (_studentLesson != null)
                    {
                        int completedStatusId = 2;
                        _studentLesson.Status_Id = completedStatusId;
                        _context.SaveChanges();

                        MessageBox.Show("🎉 Поздравляем! Вы завершили урок!\n\nВсе задания выполнены успешно!",
                                      "Урок завершен",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении прогресса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        private T FindChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            if (parent == null) return null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                T result = child as T;
                if (result != null && result.Name == name)
                    return result;

                result = FindChildByName<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
