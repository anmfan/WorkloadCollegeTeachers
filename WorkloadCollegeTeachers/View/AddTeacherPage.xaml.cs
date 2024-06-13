using DevExpress.XtraPrinting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
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
using Microsoft.SqlServer.Server;
using System.Data.Entity;
using System.Text.RegularExpressions;
using static WorkloadCollegeTeachers.Model.GroupsOnSpecialties;
using System.Data.Entity.Validation;

namespace WorkloadCollegeTeachers.View
{
    /// <summary>
    /// Логика взаимодействия для AddTeacherPage.xaml
    /// </summary>
    public partial class AddTeacherPage : Page
    {
        Core db = new Core();
        public event EventHandler<TeacherEventArgs> TeacherAdded;

        public AddTeacherPage()
        {
            InitializeComponent();
            LoadSpecialties();
        }

        private void LoadSpecialties()
        {
            //Вывод специальностей групп в ComboBox
            SpecialtiesComboBox.ItemsSource = db.context.Specialties.ToList();
            SpecialtiesComboBox.DisplayMemberPath = "Name";
            SpecialtiesComboBox.SelectedValuePath = "specialties_id";
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
           
        private byte[] ImageToByteArray(BitmapImage bitmapImage)
        {
            //Преобразование изображения в тип byte[]
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    encoder.Save(stream);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка конвертации изображения: {ex.Message}");
                throw;
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Добавление изображения в DataGrid
            OpenFileDialog image = new OpenFileDialog();
            image.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            if (image.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(image.FileName);
                long fileSize = fileInfo.Length;
                fileSize = fileSize / 1024;
                if (fileSize > 51200)
                {
                    MessageBox.Show("Слишком большой файл", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Uri UriImage = new Uri(image.FileName);
                    myImage.Source = new BitmapImage(UriImage);
                }
            }
        }

        private void ClearInputFields()
        {
            //Очищение полей после добавления преподавателя
            NameTextBox.Text = "";
            SecondNameTextBox.Text = "";
            PatronymicTextBox.Text = "";
            AgeTextBox.Text = "";
            LoginTextBox.Text = "";
            PasswordTextBox.Text = "";
            myImage.Source = null;
            SpecialtiesComboBox.SelectedItem = null;
        }

        private void AgeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private bool IsValidRussianText(string text)
        // Метод для проверки, что строка содержит только русские буквы
        {
            return Regex.IsMatch(text, @"^[А-Яа-яЁё]+$");
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Добавление преподавателя
            try
            {
                BitmapImage bitmapImage = myImage.Source as BitmapImage;

                if (bitmapImage == null)
                {
                    MessageBox.Show("Изображение не загружено!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                byte[] imageBytes = ImageToByteArray(bitmapImage);

                if (!String.IsNullOrEmpty(NameTextBox.Text) && !String.IsNullOrEmpty(SecondNameTextBox.Text) &&
                    !String.IsNullOrEmpty(PatronymicTextBox.Text) && !String.IsNullOrEmpty(AgeTextBox.Text) &&
                    imageBytes != null)
                {

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

                    int? roleId = adminCheckBox.IsChecked == true ? db.context.Roles.FirstOrDefault(r => r.Role == "Администратор")?.ID :
                                     db.context.Roles.FirstOrDefault(r => r.Role == "Преподаватель")?.ID;

                    if (roleId == null)
                    {
                        MessageBox.Show("Роль не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

                    var selectedSpecialty = (Specialties)SpecialtiesComboBox.SelectedItem;

                    if (selectedSpecialty == null)
                    {
                        MessageBox.Show("Пожалуйста, выберите специальность.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string userLogin = LoginTextBox.Text;

                    if (db.context.Users.Any(u => u.UserLogin == userLogin))
                    {
                        MessageBox.Show("Логин уже существует. Пожалуйста, выберите другой логин.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    Teachers newTeacher = new Teachers()
                    {
                        Surname = SecondNameTextBox.Text,
                        Name = NameTextBox.Text,
                        Patronymic = PatronymicTextBox.Text,
                        Age = AgeTextBox.Text,
                        Image = imageBytes,
                        specialties_ID = selectedSpecialty.specialties_id
                    };
                    db.context.Teachers.Add(newTeacher);

                    string password = PasswordTextBox.Text;
                    PasswordTextBox.Clear();
                    string hashedPassword = PasswordHasherMD5.Md5Hash(password);

                    Users newUser = new Users()
                    {
                        IDRole = roleId.Value,
                        UserName = NameTextBox.Text,
                        UserLastName = SecondNameTextBox.Text,
                        UserOtherName = PatronymicTextBox.Text,
                        UserLogin = LoginTextBox.Text,
                        UserPassword = hashedPassword
                    };
                    db.context.Users.Add(newUser);

                    int result = db.context.SaveChanges();
                    if (result > 0)
                    {
                        MessageBox.Show("Преподаватель добавлен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearInputFields();
                    }
                    
                    TeacherAdded?.Invoke(this, new TeacherEventArgs(newTeacher));
                    this.NavigationService.GoBack();
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        MessageBox.Show($"Свойство: {validationError.PropertyName} Ошибка: {validationError.ErrorMessage}", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = GetFullExceptionMessage(ex);
                MessageBox.Show($"Произошла ошибка: {errorMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SpecialtiesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void GroupsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void GroupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
    public class TeacherEventArgs : EventArgs
    {
        //Класс для передачи данных между двумя формами
        public Teachers TeacherAdded { get; }

        public TeacherEventArgs(Teachers newTeacher)
        {
            TeacherAdded = newTeacher;
        }
    }
} 
