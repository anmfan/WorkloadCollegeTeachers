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
    /// Логика взаимодействия для AddDisciplinesTeacherPage.xaml
    /// </summary>
    public partial class AddDisciplinesTeacherPage : Page
    {
        Core db;
        public event EventHandler<DisciplineEventArgs> DisciplineAdded;
        private Teachers selectedTeacher;
        private AcademicDisciplines _selectedDiscipline;
        private int selectedTeacherID;
        public AddDisciplinesTeacherPage(Teachers teacher, Core dbContext)
        {
            InitializeComponent();
            selectedTeacher = teacher;
            selectedTeacherID = teacher.ID;
            db = dbContext;
            LoadDisciplines();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void LoadDisciplines()
        {
            //Метод для вывода дисциплин, связанных со специальностью выбранного преподавателя
            try
            {
                var disciplines = db.context.AcademicDisciplines.Where(d => d.Specialties.Any(s => s.specialties_id == selectedTeacher.specialties_ID)).ToList();

                TeacherDisciplinesComboBox.ItemsSource = disciplines;
                TeacherDisciplinesComboBox.DisplayMemberPath = "Name_discipline";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке дисциплин: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public class DisciplineEventArgs : EventArgs
        {
            //Класс для передачи данных между двумя формами
            public Teachers DisciplineAdded { get; }

            public DisciplineEventArgs(Teachers newTeacher)
            {
                DisciplineAdded = newTeacher;
            }
        }

        private string GetFullExceptionMessage(Exception ex)
        {
            //Вызывает внутреннее исключение
            StringBuilder sb = new StringBuilder();
            while (ex != null)
            {
                sb.AppendLine(ex.Message);
                ex = ex.InnerException;
            }
            return sb.ToString();
        }

        private void AddTeacherDisciplineButton_Click(object sender, RoutedEventArgs e)
        {
            //Добавление дисциплины преподавателю
            try
            {
                // Проверяем, существует ли уже дисциплина с таким же названием у преподавателя
                if (selectedTeacher.LoadOfTeachers.Any(l => l.Name_Discipline == _selectedDiscipline.Name_discipline))
                {
                    MessageBox.Show("Данная дисциплина уже добавлена этому преподавателю!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Суммирование всех часов по дисциплине
                int totalHours = (_selectedDiscipline.Hours_theory ?? 0) + (_selectedDiscipline.Hours_practice ?? 0) +
                                 (_selectedDiscipline.Hours_independent ?? 0) + (_selectedDiscipline.Hours_coursework ?? 0);

                LoadOfTeachers load = new LoadOfTeachers
                {
                    Name_Discipline = _selectedDiscipline.Name_discipline,
                    Load_Hours = totalHours,
                    Term_Work = _selectedDiscipline.Number_term ?? 0,
                    IDTeacher = selectedTeacherID
                };

                
                selectedTeacher.LoadOfTeachers.Add(load);

                db.context.SaveChanges();

                MessageBox.Show("Дисциплина успешно добавлена преподавателю!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DisciplineAdded?.Invoke(this, new DisciplineEventArgs(selectedTeacher));
                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {GetFullExceptionMessage(ex)}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private void TeacherDisciplinesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDiscipline = TeacherDisciplinesComboBox.SelectedItem as AcademicDisciplines;
        }
    }
}
