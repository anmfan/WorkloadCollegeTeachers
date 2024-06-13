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
    /// Логика взаимодействия для AddGroupsPage.xaml
    /// </summary>
    public partial class AddGroupsPage : Page   
    {
        Core db = new Core();
        public event EventHandler<GroupsEventArgs> AddedGroups;
        public AddGroupsPage()
        {
            InitializeComponent();
            LoadSpecialties();
        }


        private void FormEducationTextBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void FormEducationZaochnoTextBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void FormEducationOchnoTextBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void LoadSpecialties()
        {
            try
            {
                Core db = new Core();
                List<Specialties> specialties = db.GetSpecialties();

                SpecialtiesIdTextBox.ItemsSource = specialties;
                SpecialtiesIdTextBox.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {GetFullExceptionMessage(ex)}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GroupsAddButton_Click(object sender, RoutedEventArgs e)
        {
            //Добавление групп
            try
            {
                if (!String.IsNullOrEmpty(NameGroupTextBox.Text))
                {
                    string groupName = NameGroupTextBox.Text;

                    // Проверка, что выбран один тип обучения
                    if (!(FormEducationOchno.IsChecked == true || FormEducationZaochno.IsChecked == true))
                    {
                        MessageBox.Show("Выберите тип обучения (Очно или Заочно).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Проверка, что выбран один тип бюджета
                    if (!(BudgetRadioButton.IsChecked == true || NonBudgetRadioButton.IsChecked == true))
                    {
                        MessageBox.Show("Выберите тип бюджета (Бюджет или Внебюджет).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string formEducation = FormEducationOchno.IsChecked == true ? "Очно" : "Заочно";
                    string budget = BudgetRadioButton.IsChecked == true ? "Бюджет" : "Внебюджет";


                    // Проверка выбранной группы
                    if (SpecialtiesIdTextBox.SelectedItem == null)
                    {
                        MessageBox.Show("Выберите специальность.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Получение выбранной группы и её ID
                    Specialties selectedSpecialty = (Specialties)SpecialtiesIdTextBox.SelectedItem;
                    int specialtiesId = selectedSpecialty.specialties_id;

                    // Добавить букву "К" к названию группы, если выбран "Внебюджет"
                    if (budget == "Внебюджет")
                    {
                        groupName += "к";
                    }

                    // Проверка на существование группы с таким же названием
                    bool groupExists = db.context.Groups.Any(g => g.Name_group == groupName);
                    if (groupExists)
                    {
                        MessageBox.Show("Группа с таким названием уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Создать новую группу
                    Groups newGroup = new Groups()
                    {
                        specialties_id = specialtiesId,
                        Name_group = groupName,
                        Form_education = formEducation,
                        Budget = budget,
                    };
                    db.context.Groups.Add(newGroup);

                    int result = db.context.SaveChanges();
                    if (result > 0)
                    {
                        MessageBox.Show("Группа добавлена.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                        OnGroupsAdded(new GroupsEventArgs(newGroup));
                        this.NavigationService.GoBack();
                    }
                    
                }
                else
                {
                    MessageBox.Show("Пожалуйста, введите название группы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {GetFullExceptionMessage(ex)}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        protected virtual void OnGroupsAdded(GroupsEventArgs e)
        {
            AddedGroups?.Invoke(this, e);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
    public class GroupsEventArgs : EventArgs
    {
        public Groups AddedGroups { get; }

        public GroupsEventArgs(Groups addedgroups)
        {
            AddedGroups = addedgroups;
        }
    }
}
