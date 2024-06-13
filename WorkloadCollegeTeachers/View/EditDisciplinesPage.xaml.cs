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
using WorkloadCollegeTeachers.Model;

namespace WorkloadCollegeTeachers.View
{
    /// <summary>
    /// Логика взаимодействия для EditDisciplinesPage.xaml
    /// </summary>
    public partial class EditDisciplinesPage : Page
    {
        private Core db;
        private AcademicDisciplines _selectedDiscipline;
        public event EventHandler<DisciplinesEventArgs> DisciplineUpdated;
        public EditDisciplinesPage(AcademicDisciplines selectedDiscipline, Core dbContext)
        {
            InitializeComponent();
            _selectedDiscipline = selectedDiscipline;
            db = dbContext;
            LoadSpecialties();
            LoadDisciplineData();
            GetAllSpecialties();
        }

        private void LoadSpecialties()
        {
            var specialties = db.context.Specialties.ToList();
            AddDisciplinesComboBox.ItemsSource = specialties;
        }

        private void LoadDisciplineData()
        {
            if (_selectedDiscipline != null)
            {
                //Вывод данных выбранной специальности в поля
                NameTextBox.Text = _selectedDiscipline.Name_discipline;
                HoursTheoryTextBox.Text = _selectedDiscipline.Hours_theory.ToString();
                HoursPracticeTextBox.Text = _selectedDiscipline.Hours_practice.ToString();
                HoursMyselfTextBox.Text = _selectedDiscipline.Hours_independent.ToString();
                HourseCourseWorkTextBox.Text = _selectedDiscipline.Hours_coursework.ToString();
                NumberTermTextBox.Text = _selectedDiscipline.Number_term.ToString();
                AddDisciplinesComboBox.SelectedValue = _selectedDiscipline.Specialties.FirstOrDefault()?.specialties_id;
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GetAllSpecialties()
        {
            //Вывод специальностей в DisciplinesComboBox
            var specialties = db.context.Specialties.ToList();
            AddDisciplinesComboBox.ItemsSource = specialties;
            AddDisciplinesComboBox.DisplayMemberPath = "Name";
            AddDisciplinesComboBox.SelectedValuePath = "specialties_id";
        }

        private void AddDisciplineButton_Click(object sender, RoutedEventArgs e)
        {
            //Обновление дисциплины
            try
            {
                var selectedSpecialty = (Specialties)AddDisciplinesComboBox.SelectedItem;

                if (selectedSpecialty == null)
                {
                    MessageBox.Show("Пожалуйста, выберите специальность.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка на уникальность дисциплины для выбранной специальности
                bool disciplineExists = db.context.AcademicDisciplines.Any(d => d.Name_discipline == NameTextBox.Text
                    && d.academic_ID != _selectedDiscipline.academic_ID
                    && d.Specialties.Any(s => s.specialties_id == selectedSpecialty.specialties_id));

                if (disciplineExists)
                {
                    MessageBox.Show("Дисциплина с таким названием уже существует для выбранной специальности.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _selectedDiscipline.Name_discipline = NameTextBox.Text;
                _selectedDiscipline.Hours_theory = int.Parse(HoursTheoryTextBox.Text);
                _selectedDiscipline.Hours_practice = int.Parse(HoursPracticeTextBox.Text);
                _selectedDiscipline.Hours_independent = int.Parse(HoursMyselfTextBox.Text);
                _selectedDiscipline.Hours_coursework = int.Parse(HourseCourseWorkTextBox.Text);
                _selectedDiscipline.Number_term = int.Parse(NumberTermTextBox.Text);

                // Обновление специальности
                var existingSpecialty = _selectedDiscipline.Specialties.FirstOrDefault();
                if (existingSpecialty != null && existingSpecialty.specialties_id != selectedSpecialty.specialties_id)
                {
                    // Найти все записи в LoadOfTeachers, где дисциплина прикреплена к преподавателю с другой специальностью
                    var loadOfTeachersEntries = db.context.LoadOfTeachers
                        .Where(l => l.Name_Discipline == _selectedDiscipline.Name_discipline && l.Teachers.Any(t => t.specialties_ID != selectedSpecialty.specialties_id))
                        .ToList();

                    // Удалить такие записи
                    foreach (var entry in loadOfTeachersEntries)
                    {
                        // Найти все связанные группы, учитывая возможные значения NULL
                        var loadTeachersGroups = db.context.LoadOfTeachers
                            .Where(lt => lt.IDTeacher == entry.IDTeacher && (lt.TeacherGroup == entry.TeacherGroup || entry.TeacherGroup == null))
                            .ToList();

                        foreach (var group in loadTeachersGroups)
                        {
                            db.context.LoadOfTeachers.Remove(group);
                        }

                        db.context.LoadOfTeachers.Remove(entry);
                    }

                    _selectedDiscipline.Specialties.Remove(existingSpecialty);
                    _selectedDiscipline.Specialties.Add(selectedSpecialty);
                }
                else if (existingSpecialty == null)
                {
                    _selectedDiscipline.Specialties.Add(selectedSpecialty);
                }

                db.context.Entry(_selectedDiscipline).State = EntityState.Modified;

                int result = db.context.SaveChanges();
                if (result > 0)
                {
                    MessageBox.Show("Дисциплина успешно обновлена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    DisciplineUpdated?.Invoke(this, new DisciplinesEventArgs(_selectedDiscipline));
                    this.NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddDisciplinesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
