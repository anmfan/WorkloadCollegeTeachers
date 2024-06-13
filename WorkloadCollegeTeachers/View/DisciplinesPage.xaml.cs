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
using System.Data.Entity;

namespace WorkloadCollegeTeachers.View
{
    /// <summary>
    /// Логика взаимодействия для DisciplinesPage.xaml
    /// </summary>
    public partial class DisciplinesPage : Page
    {
        Core db = new Core();
        public DisciplinesPage()
        {
            InitializeComponent();
            GetAllDisciplines();
            GetAllSpecialties();
        }

        private void GetAllSpecialties()
        {
            //Вывод специальностей в DisciplinesComboBox
            var specialties = db.context.Specialties.ToList();
            DisciplinesComboBox.ItemsSource = specialties;
            DisciplinesComboBox.DisplayMemberPath = "Name";
            DisciplinesComboBox.SelectedValuePath = "specialties_id";
        }

        private void GetAllDisciplines()
        {
            //Вывод дисцплин, включая специальности, к которым они принадлежат
            var disciplines = db.context.AcademicDisciplines.Include(d => d.Specialties).Select(d => new
                                    {
                                        d.academic_ID,
                                        d.Name_discipline,
                                        SpecialtyName = d.Specialties.FirstOrDefault().Name,
                                        d.Hours_theory,
                                        d.Hours_practice,
                                        d.Hours_independent,
                                        d.Hours_coursework,
                                        d.Number_term}).ToList();

            DisciplinesDataGrid.ItemsSource = disciplines;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DisciplinesComboBox.SelectedItem != null)
            {
                Specialties selectedSpecialty = (Specialties)DisciplinesComboBox.SelectedItem;
                LoadDisciplines(selectedSpecialty);
            }
        }

        private void LoadDisciplines(Specialties specialty)
        {
            // Вывод дисциплин и специальностей, когда выбирается какая-то специальность в ComboBox
            var disciplines = db.context.AcademicDisciplines.Where(d => d.Specialties.Any(s => s.specialties_id == specialty.specialties_id)).Select(d => new
            {
                d.academic_ID,
                d.Name_discipline,
                SpecialtyName = d.Specialties.FirstOrDefault().Name,
                d.Hours_theory,
                d.Hours_practice,
                d.Hours_independent,
                d.Hours_coursework,
                d.Number_term
            }).ToList();

            DisciplinesDataGrid.ItemsSource = disciplines;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Кнопка Добавления
            this.NavigationService.Navigate(new AddDisciplinesPage());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Кнопка Удаления
            if (DisciplinesDataGrid.SelectedItem != null)
            {
                var selectedDiscipline = DisciplinesDataGrid.SelectedItem as dynamic;
                int academicID = selectedDiscipline.academic_ID;

                var disciplineToDelete = db.context.AcademicDisciplines.Include(d => d.Specialties).FirstOrDefault(d => d.academic_ID == academicID);

                if (disciplineToDelete != null)
                {
                    // Удаление всех записей из LoadOfTeachers, связанных с данной дисциплиной
                    var loadsToDelete = db.context.LoadOfTeachers.Where(l => l.Name_Discipline == disciplineToDelete.Name_discipline).ToList();
                    db.context.LoadOfTeachers.RemoveRange(loadsToDelete);

                    db.context.AcademicDisciplines.Remove(disciplineToDelete);
                    db.context.SaveChanges();
                    MessageBox.Show("Дисциплина удалена!");

                    GetAllDisciplines();
                }
            }
            else
            {
                MessageBox.Show($"Пожалуйста, выберите дисциплину для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Кнопка Обновления
            GetAllDisciplines();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // Переход на страницу редактирования
            var selectedDiscipline = (dynamic)DisciplinesDataGrid.SelectedItem;
            if (selectedDiscipline != null)
            {
                var discipline = db.context.AcademicDisciplines.Find(selectedDiscipline.academic_ID);
                var editDisciplinesPage = new EditDisciplinesPage(discipline, db);
                editDisciplinesPage.DisciplineUpdated += OnDisciplineUpdated;
                this.NavigationService.Navigate(editDisciplinesPage);
            }
        }
        private void EditDisciplinesPage_EditDisciplines(object sender, DisciplinesEventArgs e)
        {
            GetAllDisciplines();
            GetAllSpecialties();
        }

        private void OnDisciplineAdded(object sender, DisciplinesEventArgs e)
        {
            GetAllDisciplines();
        }

        private void OnDisciplineUpdated(object sender, DisciplinesEventArgs e)
        {
            GetAllDisciplines();
        }
    }
}
