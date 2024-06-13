using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для EditSpecialtiesPage.xaml
    /// </summary>
    public partial class EditSpecialtiesPage : Page
    {
        Core db;
        private Specialties specialt;
        public event EventHandler<SpecialtiesEventArgs> UpdatedSpecialty;
        public EditSpecialtiesPage(Specialties specialt, Core dbContext)
        {
            InitializeComponent();
            db = dbContext;
            this.specialt = specialt;
            PopulateFieldsSecond();
        }

        private void PopulateFieldsSecond()
        {
            //Заполнить поля данными выбранной специальности
            CodeTextBox.Text = specialt.Code;
            NameTextBox.Text = specialt.Name;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void SpecialitiesAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SpecialitiesEditButton_Click(object sender, RoutedEventArgs e)
        {
            //Редактирование специальности
            string code = CodeTextBox.Text;

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

            // Обновить данные специальности
            specialt.Code = CodeTextBox.Text;
            specialt.Name = NameTextBox.Text;

            // Сохранить изменения в базе данных
            try
            {
                db.context.SaveChanges();
                UpdatedSpecialty?.Invoke(this, new SpecialtiesEventArgs(specialt));
                MessageBox.Show("Данные специальности обновлены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                this.NavigationService.GoBack();
            }
            catch(Exception ex) { MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

    }
    public class SpecialtiesEventArgs : EventArgs
    {
        public Specialties UpdatedSpecialty { get; }

        public SpecialtiesEventArgs(Specialties updatedSpecialty)
        {
            UpdatedSpecialty = updatedSpecialty;
        }
    }
}
