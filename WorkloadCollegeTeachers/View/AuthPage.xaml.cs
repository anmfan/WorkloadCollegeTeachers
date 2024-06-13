using WorkloadCollegeTeachers.Model;
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
using Microsoft.Win32;
using System.Security.Cryptography;

namespace WorkloadCollegeTeachers.View
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private string GetFullErrorMessage(Exception ex)
        {
            //Метод для конкретизации выявленных ошибок
            var sb = new StringBuilder();
            sb.AppendLine(ex.Message);

            var innerException = ex.InnerException;
            while (innerException != null)
            {
                sb.AppendLine(innerException.Message);
                innerException = innerException.InnerException;
            }

            sb.AppendLine(ex.StackTrace);
            return sb.ToString();
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            Core db = new Core();
            try
            {
                if (!String.IsNullOrEmpty(Login.Text) && !String.IsNullOrEmpty(Password.Password))
                {
                    string enteredPasswordHash = PasswordHasherMD5.Md5Hash(Password.Password);

                    // Поиск пользователя в базе данных по логину и хэшу пароля
                    Users user = db.context.Users.FirstOrDefault(x => x.UserLogin == Login.Text && x.UserPassword == enteredPasswordHash);

                    if (user != null)
                    {
                        // Пользователь найден и пароль совпадает, переход на страницу IntermediatePage
                        this.NavigationService.Navigate(new IntermediatePage());
                    }
                    else
                    {
                        // Пользователь не найден или пароль не совпадает
                        MessageBox.Show("Такого пользователя не существует.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Не все данные введены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch(Exception ex) { MessageBox.Show("Произошла ошибка: " + GetFullErrorMessage(ex)); }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
