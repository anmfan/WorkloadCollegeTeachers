using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Data.Entity;
namespace WorkloadCollegeTeachers.View
{
    /// <summary>
    /// Логика взаимодействия для GroupsPage.xaml
    /// </summary>
    public partial class GroupsPage : Page
    {
        Core db = new Core();
        public GroupsPage()
        {
            InitializeComponent();
            LoadGroups();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        public void LoadGroups()
        {
            Core db = new Core();
            try
            {
                    var groups = db.context.Groups.Include("Specialties").ToList();
                    DataGridGroups.ItemsSource = groups;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Переход на страницу редактирования
            try
            {
                if (DataGridGroups.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Группа не выбрана.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (DataGridGroups.SelectedItems.Count > 1)
                {
                    MessageBox.Show("Выберите только одну группу для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получить выбранную группу
                Groups selectedGroup = (Groups)DataGridGroups.SelectedItem;

                // Создать и открыть страницу редактирования, передав данные выбранной группы
                EditGroupsPage editGroupsPage = new EditGroupsPage(selectedGroup);
                editGroupsPage.UpdatedGroups += EditGroupsPage_EditGroups;
                this.NavigationService.Navigate(editGroupsPage);
            }
            catch (Exception ex) { MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); };
        }

        private void EditGroupsPage_EditGroups(object sender, GroupsEventArgs e)
        {
            LoadGroups();
        }

        public void UpdateGridGroups()
        {
            try
            {
                // Обновляем список групп из базы данных
                List<Groups> groups = db.context.Groups.Include("Specialties").ToList();

                // Устанавливаем обновленный список групп в DataGridGroups
                DataGridGroups.ItemsSource = groups;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при обновлении списка групп: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
                AddGroupsPage addGroupsPage = new AddGroupsPage();
                addGroupsPage.AddedGroups += AddGroupsPage_AddedGroups;
                this.NavigationService.Navigate(new AddGroupsPage());
        }
        private void AddGroupsPage_AddedGroups(object sender, GroupsEventArgs e)
        {
            //Обновление для AddGroups
            LoadGroups();
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            LoadGroups();
        }

        private void DataGridGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void UpdateDatGrid()
        {
            DataGridGroups.ItemsSource = db.context.Specialties.ToList();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            //Удаление групп
            Core db = new Core();
            try
            {
                if (DataGridGroups.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите группу для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Получаем выбранную группу из DataGrid
                    Groups selectedSpecialty = DataGridGroups.SelectedItem as Groups;

                    if (selectedSpecialty != null)
                    {
                        var specialtyToRemove = db.context.Groups.FirstOrDefault(s => s.specialties_id == selectedSpecialty.specialties_id);
                        if (specialtyToRemove != null)
                        {
                            db.context.Groups.Remove(specialtyToRemove);
                            db.context.SaveChanges();

                            MessageBox.Show("Группа удалена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadGroups();
                        }
                        else
                        {
                            MessageBox.Show("Выбранная группа не найдена в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении группы.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка: " + ex.Message); }
        }
    }
}
