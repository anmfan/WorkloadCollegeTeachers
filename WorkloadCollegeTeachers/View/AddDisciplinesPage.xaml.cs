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
    /// Логика взаимодействия для AddDisciplinesPage.xaml
    /// </summary>
    public partial class AddDisciplinesPage : Page
    {
        Core db;
        public AddDisciplinesPage()
        {
            InitializeComponent();
            db = new Core();
            GetAllDisciplines();
        }

        private void GetAllDisciplines()
        {
            //Вывод специальностей в DisciplinesComboBox
            var specialties = db.context.Specialties.ToList();
            AddDisciplinesComboBox.ItemsSource = specialties;
            AddDisciplinesComboBox.DisplayMemberPath = "Name";
            AddDisciplinesComboBox.SelectedValuePath = "specialties_id";
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void AddDisciplineButton_Click(object sender, RoutedEventArgs e)
        {
            //Добавление дисциплины
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(HoursTheoryTextBox.Text) ||
                    string.IsNullOrWhiteSpace(HoursPracticeTextBox.Text) ||
                    string.IsNullOrWhiteSpace(HoursMyselfTextBox.Text) ||
                    string.IsNullOrWhiteSpace(HourseCourseWorkTextBox.Text) ||
                    string.IsNullOrWhiteSpace(NumberTermTextBox.Text) ||
                    AddDisciplinesComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.");
                    return;
                }

                var selectedSpecialty = (Specialties)AddDisciplinesComboBox.SelectedItem;
                var NumberTerm = NumberTermTextBox.Text;

                // Проверка на то, есть ли уже такая дисциплина для данной специальности, или нет
                bool disciplineExists = db.context.AcademicDisciplines
                    .Any(d => d.Name_discipline == NameTextBox.Text && d.Specialties.Any(s => s.specialties_id == selectedSpecialty.specialties_id));

                if (disciplineExists)
                {
                    MessageBox.Show("Дисциплина с таким названием уже существует для выбранной специальности.");
                    return;
                }
                if (NumberTerm != "1" && NumberTerm != "2")
                {
                    MessageBox.Show("Семестр может быть только первым или вторым.");
                    return;
                }

                var newDiscipline = new AcademicDisciplines
                {
                    Name_discipline = NameTextBox.Text,
                    Hours_theory = int.Parse(HoursTheoryTextBox.Text),
                    Hours_practice = int.Parse(HoursPracticeTextBox.Text),
                    Hours_independent = int.Parse(HoursMyselfTextBox.Text),
                    Hours_coursework = int.Parse(HourseCourseWorkTextBox.Text),
                    Number_term = int.Parse(NumberTermTextBox.Text),
                    Specialties = new List<Specialties> { selectedSpecialty }
                };

                
                if (selectedSpecialty != null)
                {
                    newDiscipline.Specialties = new List<Specialties> { selectedSpecialty };
                }

                db.context.AcademicDisciplines.Add(newDiscipline);
                db.context.SaveChanges();

                MessageBox.Show("Дисциплина добавлена!");
                this.NavigationService.GoBack();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }

    public class DisciplinesEventArgs : EventArgs
    {
        public AcademicDisciplines DisciplinesUpdated { get; }

        public DisciplinesEventArgs(AcademicDisciplines updateddisciplines)
        {
            DisciplinesUpdated = updateddisciplines;
        }
    }
}
