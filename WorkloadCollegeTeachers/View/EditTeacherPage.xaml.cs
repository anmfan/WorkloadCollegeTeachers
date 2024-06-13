using DevExpress.XtraEditors;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
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
using static WorkloadCollegeTeachers.View.AddTeacherPage;

namespace WorkloadCollegeTeachers.View
{
    /// <summary>
    /// Логика взаимодействия для EditTeacherPage.xaml
    /// </summary>
    public partial class EditTeacherPage : Page
    {
        private Teachers teacher;
        private Teachers selectedTeacher;
        public event EventHandler<TeacherEventArgs> TeacherUpdated;
        private Users user;
        Core db;

        // Событие для сообщения об обновлении преподавателя

        public EditTeacherPage(Teachers selectedTeacher, Users selectedUser, Core dbContext)
        {
            InitializeComponent();
            teacher = selectedTeacher;
            user = selectedUser;
            db = dbContext;
            PopulateFields();
            LoadSpecialties();
        }

        private void PopulateFields()
        {
            //Вывод всех уже имеющихся данных о преподавателе на страницу редактирования для наглядного изменения этих данных
            string password = PasswordTextBox.Text;
            string hashedPassword = PasswordHasherMD5.Md5Hash(password);

            //Заполнить поля данными выбранного преподавателя
            SecondNameTextBox.Text = teacher.Surname;
            NameTextBox.Text = teacher.Name;
            PatronymicTextBox.Text = teacher.Patronymic;
            AgeTextBox.Text = teacher.Age.ToString();
            LoginTextBox.Text = user.UserLogin;
            //Вывод специальностей выбранного преподавателя
            SpecialtiesComboBox.SelectedValue = teacher.specialties_ID;

            // Преобразовать byte[] обратно в изображение и отобразить
            if (teacher.Image != null)
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(teacher.Image))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
                myImage.Source = bitmap;
            }
            // Заполнить ComboBox дисциплинами преподавателя
            LoadDisciplinesForTeacher(teacher.ID);
        }

        private void LoadDisciplinesForTeacher(int teacherId)
        {
            //Вывод дисциплин, которые имеет редактируемый преподаватель
            var disciplines = db.context.LoadOfTeachers.Where(l => l.IDTeacher == teacherId).Select(l => l.Name_Discipline).Distinct().ToList();

            DisciplinesComboBox.ItemsSource = disciplines;
        }

        private void LoadGroups(int specialtyId)
        {
            // Вывод групп, связанных с выбранной специальностью, в ComboBox
            GroupsComboBox.ItemsSource = db.context.Groups.Where(g => g.specialties_id == specialtyId).ToList();
            GroupsComboBox.DisplayMemberPath = "Name_group";
            GroupsComboBox.SelectedValuePath = "ID";
        }

        private void UpdateTeacherSpecialty(int teacherId, int newSpecialtyId)
        {
            // Метод для обновления специальности преподавателя, если старая специальность сменилась

            var teacher = db.context.Teachers.Include(t => t.LoadOfTeachers).FirstOrDefault(t => t.ID == teacherId);

            if (teacher != null)
            {
                // Удаляем текущие дисциплины преподавателя
                db.context.LoadOfTeachers.RemoveRange(teacher.LoadOfTeachers);
                db.context.SaveChanges();

                // Обновляем специальность преподавателя
                teacher.specialties_ID = newSpecialtyId;
                db.context.SaveChanges();
            }
        }

        private void LoadSpecialties()
        {
            SpecialtiesComboBox.ItemsSource = db.context.Specialties.ToList();
            SpecialtiesComboBox.DisplayMemberPath = "Name";
            SpecialtiesComboBox.SelectedValuePath = "specialties_id";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void AgeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private byte[] ImageToByteArray(BitmapImage bitmapImage)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(ms);
                return ms.ToArray();
            }
        }

        private bool IsValidRussianText(string text)
        // Метод для проверки, что строка содержит только русские буквы
        {
            return Regex.IsMatch(text, @"^[А-Яа-яЁё]+$");
        }

        private void AddGroupToTeacher(Teachers teacher, Groups group, /*string disciplineName*/string discipline)
        {
            if (IsGroupAlreadyAssignedToTeacherForDiscipline(teacher.ID, group.ID, discipline))
            {
                MessageBox.Show($"Группа «{group.Name_group}» уже назначена для дисциплины «{discipline}» у данного преподавателя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var loadOfTeacher = new LoadOfTeachers
            {
                IDTeacher = teacher.ID,
                TeacherGroup = group.ID,
                Name_Discipline = discipline,
                Load_Hours = 0,
                Term_Work = 0
            };

            db.context.LoadOfTeachers.Add(loadOfTeacher);
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int currentSpecialtyId = teacher.specialties_ID;
                int newSpecialtyId = (int)SpecialtiesComboBox.SelectedValue;

                if (currentSpecialtyId != newSpecialtyId)
                {
                    // Удаление всех дисциплин и связанных с ними групп, если специальнось сменилась
                    var loadsToRemove = db.context.LoadOfTeachers.Where(l => l.IDTeacher == teacher.ID).ToList();
                    db.context.LoadOfTeachers.RemoveRange(loadsToRemove);

                    UpdateTeacherSpecialty(teacher.ID, newSpecialtyId);
                    teacher.specialties_ID = newSpecialtyId;
                }

                teacher.Surname = SecondNameTextBox.Text;
                teacher.Name = NameTextBox.Text;
                teacher.Patronymic = PatronymicTextBox.Text;
                teacher.Age = AgeTextBox.Text;
                user.UserLogin = LoginTextBox.Text;
                user.UserName = NameTextBox.Text;
                user.UserLastName = SecondNameTextBox.Text;
                user.UserOtherName = PatronymicTextBox.Text;

                string originalPasswordHash = user.UserPassword;
                string newPasswordHash = PasswordHasherMD5.Md5Hash(PasswordTextBox.Text);

                if (newPasswordHash != originalPasswordHash)
                {
                    if (!string.IsNullOrEmpty(PasswordTextBox.Text))
                    {
                        user.UserPassword = newPasswordHash;
                    }
                }

                if (newPasswordHash == originalPasswordHash)
                {
                    MessageBox.Show("Введенный новый пароль совпадает с исходным паролем.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string Age = AgeTextBox.Text;

                if (!Age.All(char.IsDigit))
                {
                    MessageBox.Show("Неправильно указан возраст.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (Age.Length >= 3)
                {
                    MessageBox.Show("Возраст не может быть больше 99 лет.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!IsValidRussianText(NameTextBox.Text))
                {
                    MessageBox.Show("Имя должно содержать только русские буквы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!IsValidRussianText(SecondNameTextBox.Text))
                {
                    MessageBox.Show("Фамилия должна содержать только русские буквы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!IsValidRussianText(PatronymicTextBox.Text))
                {
                    MessageBox.Show("Отчество должно содержать только русские буквы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (myImage.Source is BitmapImage bitmapImage)
                {
                    teacher.Image = ImageToByteArray(bitmapImage);
                }


                var selectedGroup = GroupsComboBox.SelectedItem as Groups;
                var selectedDiscipline = DisciplinesComboBox.SelectedItem as string;

                if (selectedGroup != null && selectedDiscipline != null)
                {
                    AddGroupToTeacher(teacher, selectedGroup, selectedDiscipline);
                }

                db.context.SaveChanges();

                MessageBox.Show("Данные преподавателя обновлены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                TeacherUpdated?.Invoke(this, new TeacherEventArgs(teacher));
                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsGroupAlreadyAssignedToTeacherForDiscipline(int teacherId, int groupId, string discipline)
        {
            return db.context.LoadOfTeachers.Any(l => l.IDTeacher == teacherId && l.TeacherGroup == groupId && l.Name_Discipline == discipline);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Добавление изображения в DataGrid
            OpenFileDialog image = new OpenFileDialog
            {
                Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif"
            };

            if (image.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(image.FileName);
                long fileSize = fileInfo.Length;
                fileSize = fileSize / 1024;
                if (fileSize > 51200)
                {
                    MessageBox.Show("Слишком большой файл");
                }
                else
                {
                    Uri UriImage = new Uri(image.FileName);
                    myImage.Source = new BitmapImage(UriImage);
                }
            }
        }

        private void SpecialtiesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedSpecialty = (Specialties)SpecialtiesComboBox.SelectedItem;

            if (selectedSpecialty != null)
            {
                LoadGroups(selectedSpecialty.specialties_id);
            }
        }

        private void GroupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
