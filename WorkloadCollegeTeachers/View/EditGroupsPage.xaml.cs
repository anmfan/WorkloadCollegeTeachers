using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для EditGroupsPage.xaml
    /// </summary>
    public partial class EditGroupsPage : Page
    {
        Core db;
        private Groups _group;
        public event EventHandler<GroupsEventArgs> UpdatedGroups;
        public EditGroupsPage(Groups selectedGroup)
        {
            InitializeComponent();
            _group = selectedGroup;
            db = new Core();
            _group = db.context.Groups.Find(selectedGroup.ID);
            PopulateFieldsSecond();
            LoadGroupDetails();
        }

        private void PopulateFieldsSecond()
        {
            if (_group != null)
            {
                NameGroupTextBox.Text = _group.Name_group;
                SpecialtiesIdComboBox.ItemsSource = db.context.Specialties.ToList();
                SpecialtiesIdComboBox.DisplayMemberPath = "Name"; 
                SpecialtiesIdComboBox.SelectedValuePath = "specialties_id"; 
                SpecialtiesIdComboBox.SelectedValue = _group.specialties_id;
            }
        }

        private void GroupsEditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newSpecialty = (Specialties)SpecialtiesIdComboBox.SelectedItem;
                if (_group.Specialties.specialties_id != newSpecialty.specialties_id)
                {
                    // Get all LoadOfTeachers entries for the current group
                    var loadOfTeachersEntries = db.context.LoadOfTeachers.Where(l => l.TeacherGroup == _group.ID).ToList();

                    // Check if any teacher linked to this group has a different specialty
                    foreach (var entry in loadOfTeachersEntries)
                    {
                        var teacher = db.context.Teachers.FirstOrDefault(t => t.ID == entry.IDTeacher);
                        if (teacher != null && teacher.specialties_ID != newSpecialty.specialties_id)
                        {
                            db.context.LoadOfTeachers.Remove(entry);
                        }
                    }
                }

                // Обновление данных группы
                _group.Name_group = NameGroupTextBox.Text;
                _group.Form_education = FormEducationOchno.IsChecked == true ? "Очно" : "Заочно";
                string oldBudget = _group.Budget;
                _group.Budget = BudgetRadioButton.IsChecked == true ? "Бюджет" : "Внебюджет";
                _group.Specialties = (Specialties)SpecialtiesIdComboBox.SelectedItem;

                if (oldBudget == "Внебюджет" && _group.Budget == "Бюджет" && _group.Name_group.EndsWith("к"))
                {
                    _group.Name_group = _group.Name_group.Substring(0, _group.Name_group.Length - 1);
                }

                if (_group.Budget == "Внебюджет" && !_group.Name_group.EndsWith("к"))
                {
                    _group.Name_group += "к"; 
                }

                    int result = db.context.SaveChanges();
                    if (result > 0)
                {
                    UpdatedGroups?.Invoke(this, new GroupsEventArgs(_group));
                    MessageBox.Show("Группа обновлена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.NavigationService.GoBack(); 
                }
                else
                {
                    MessageBox.Show("Изменений не внесено!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Сохранение не удалось: {GetFullExceptionMessage(ex)}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }
        

        private string GetFullExceptionMessage(Exception ex)
        {
            //Вызывает внутреннее исключение, чтобы увидеть подробности ошибки
            StringBuilder sb = new StringBuilder();
            while (ex != null)
            {
                sb.AppendLine(ex.Message);
                ex = ex.InnerException;
            }
            return sb.ToString();
        }
        private void LoadGroupDetails()
        {
            if (_group != null)
            {
                NameGroupTextBox.Text = _group.Name_group;
                if (_group.Form_education == "Очно")
                {
                    FormEducationOchno.IsChecked = true;
                }
                else if (_group.Form_education == "Заочно")
                {
                    FormEducationZaochno.IsChecked = true;
                }

                if (_group.Budget == "Бюджет")
                {
                    BudgetRadioButton.IsChecked = true;
                }
                else if (_group.Budget == "Внебюджет")
                {
                    NonBudgetRadioButton.IsChecked = true;
                }

                // Загрузка специальности
                SpecialtiesIdComboBox.SelectedItem = _group.Specialties;
            }
        }


        private void FormEducationOchnoTextBox_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void FormEducationZaochnoTextBox_Checked(object sender, RoutedEventArgs e)
        {
           
        }

        private void SpecialtiesIdTextBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpecialtiesIdComboBox.SelectedItem != null)
            {
                _group.specialties_id = ((Specialties)SpecialtiesIdComboBox.SelectedItem).specialties_id;
            }
        }

        private void NameGroupTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void BudgetRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void NonBudgetRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
