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
using WorkloadCollegeTeachers.Model;

namespace WorkloadCollegeTeachers.View
{
    /// <summary>
    /// Логика взаимодействия для DeleteDisciplinesTeacherPage.xaml
    /// </summary>
    public partial class DeleteDisciplinesTeacherPage : Page
    {
        private Teachers _selectedTeacher;
        private Groups _groups;
        private Core _db;

        public DeleteDisciplinesTeacherPage(Teachers selectedTeacher, Core db)
        {
            InitializeComponent();
            _selectedTeacher = selectedTeacher;
            _db = db;
            LoadDisciplines();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void LoadDisciplines()
        {
            var disciplinesWithGroups = _db.context.LoadOfTeachers
        .Where(l => l.IDTeacher == _selectedTeacher.ID)
        .Select(l => new
        {
            DisciplineName = l.Name_Discipline,
            GroupName = l.TeacherGroup != null
                ? _db.context.Groups.FirstOrDefault(g => g.ID == l.TeacherGroup).Name_group
                : "Общая"
        })
        .ToList();

            // Объединяем дисциплины и группы в строку
            var itemsSource = disciplinesWithGroups.Select(dg => $"{dg.DisciplineName} - {dg.GroupName}").ToList();

            DeleteTeacherDisciplinesComboBox.ItemsSource = itemsSource;
        }
        private void DeleteTeacherDisciplineButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DeleteTeacherDisciplinesComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите дисциплину для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string selectedItem = DeleteTeacherDisciplinesComboBox.SelectedItem.ToString();
                string[] parts = selectedItem.Split(new[] { " - " }, StringSplitOptions.None);
                if (parts.Length != 2)
                {
                    MessageBox.Show("Неверный формат выбранного элемента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string selectedDiscipline = parts[0];
                string selectedGroup = parts[1];

                // Проверка на "Общая" группа
                if (selectedGroup == "Общая")
                {
                    // Удалить все дисциплины с таким же названием у этого преподавателя
                    var disciplinesToRemove = _db.context.LoadOfTeachers.Where(l => l.IDTeacher == _selectedTeacher.ID && l.Name_Discipline == selectedDiscipline).ToList();

                    if (disciplinesToRemove.Count > 0)
                    {
                        _db.context.LoadOfTeachers.RemoveRange(disciplinesToRemove);
                        _db.context.SaveChanges();
                        MessageBox.Show("Все дисциплины с названием '" + selectedDiscipline + "' удалены у преподавателя!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDisciplines();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти выбранную дисциплину.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Удалить конкретную дисциплину с привязкой к группе
                    var disciplineToRemove = _db.context.LoadOfTeachers.FirstOrDefault(l => l.IDTeacher == _selectedTeacher.ID && l.Name_Discipline == selectedDiscipline &&
                                     _db.context.Groups.FirstOrDefault(g => g.ID == l.TeacherGroup).Name_group == selectedGroup);

                    if (disciplineToRemove != null)
                    {
                        _db.context.LoadOfTeachers.Remove(disciplineToRemove);
                        _db.context.SaveChanges();
                        MessageBox.Show("Дисциплина удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDisciplines();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти выбранную дисциплину.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить выбранную дисциплину: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void TeacherDisciplinesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
