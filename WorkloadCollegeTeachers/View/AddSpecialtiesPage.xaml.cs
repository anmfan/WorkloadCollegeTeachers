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
    /// Логика взаимодействия для EditSpecialtiesPage.xaml
    /// </summary>
    public partial class AddSpecialtiesPage : Page
    {
        Core db = new Core();
        public event EventHandler<SpecialtiesEventArgs> SpecialtiesAdded;
        public AddSpecialtiesPage()
        {
            InitializeComponent();
        }                   

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private bool IsValidRussianText(string text)
        // Метод для проверки, что строка содержит только русские буквы
        {
            return Regex.IsMatch(text, @"^\p{L}+$");
        }

        private void SpecialitiesAddButton_Click(object sender, RoutedEventArgs e)
        {
            //Добавление специальностей
            try
            {
                Core db = new Core();

                if (!String.IsNullOrEmpty(CodeTextBox.Text) && !String.IsNullOrEmpty(NameTextBox.Text))
                {
                    string code = CodeTextBox.Text;
                    string name = NameTextBox.Text;

                    if (!code.All(char.IsDigit))
                    {
                        MessageBox.Show("Код специальности должен состоять только из цифр.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (code.Length != 6)
                    {
                        MessageBox.Show("Код специальности должен состоять из 6 цифр.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return; 
                    }

                    bool CodeExist = db.context.Specialties.Any(s => s.Code == code);
                    bool NameExist = db.context.Specialties.Any(s => s.Name == name);

                    if (CodeExist)
                    {
                        MessageBox.Show("Специальность с таким кодом уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (NameExist)
                    {
                        MessageBox.Show("Специальность с таким названием уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!IsValidRussianText(NameTextBox.Text))
                    {
                        MessageBox.Show("Название специальности должно содержать только буквы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Создать новую специальность
                    Specialties newSpecialties = new Specialties()
                    {
                        Code = CodeTextBox.Text,
                        Name = NameTextBox.Text
                    };
                    db.context.Specialties.Add(newSpecialties);

                    int result = db.context.SaveChanges();
                    if (result > 0)
                    {
                        MessageBox.Show("Специальность добавлена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                        OnSpecialtiesAdded(new SpecialtiesEventArgs(newSpecialties));
                        this.NavigationService.GoBack();
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        protected virtual void OnSpecialtiesAdded(SpecialtiesEventArgs e)
        {
            SpecialtiesAdded?.Invoke(this, e);
        }

        private void SpecialitiesEditButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
