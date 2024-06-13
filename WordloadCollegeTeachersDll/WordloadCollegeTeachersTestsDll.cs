using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkloadCollegeTeachers.Model;
using WordloadCollegeTeachersDll.Models;
using WorkloadCollegeTeachers.View;
using System.Text.RegularExpressions;

namespace WordloadCollegeTeachersDll
{
    public class WordloadCollegeTeachersTestsDll
    {
        Core db = new Core();

        public bool AuthButton_Click(string Login, string Password)
        {
            try
            {
                if (!String.IsNullOrEmpty(Login) && !String.IsNullOrEmpty(Password))
                {
                    string enteredPasswordHash = PasswordHasherMD5.Md5Hash(Password);

                    // Поиск пользователя в базе данных по логину и хэшу пароля
                    var user = db.context.Users.FirstOrDefault(x => x.UserLogin == Login && x.UserPassword == enteredPasswordHash);

                    if (user != null)
                    {
                        Console.WriteLine("Авторизация выполнена");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Данные пользователя не найдены");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Не все данные введены!");
                    return false;
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Произошшла ошибка!" + ex.Message);
                return false;
            }
        }

        private bool IsValidRussianText(string text)
        // Метод для проверки, что строка содержит только русские буквы
        {
            return Regex.IsMatch(text, @"^\p{L}+$");
        }

        public bool SpecialitiesAddButton_Click(string Code, string Name)
        {
            //Добавление специальностей
            try
            {
                Core db = new Core();

                if (!String.IsNullOrEmpty(Code) && !String.IsNullOrEmpty(Name))
                {
                    string code = Code;
                    string name = Name;

                    if (!code.All(char.IsDigit))
                    {
                        Console.WriteLine("Код специальности должен состоять только из цифр.");
                        return false;
                    }

                    if (code.Length != 6)
                    {
                        Console.WriteLine("Код специальности должен состоять из 6 цифр.");
                        return false;
                    }

                    bool CodeExist = db.context.Specialties.Any(s => s.Code == code);
                    bool NameExist = db.context.Specialties.Any(s => s.Name == name);

                    if (CodeExist)
                    {
                        Console.WriteLine("Специальность с таким кодом уже существует.");
                        return false;
                    }

                    if (NameExist)
                    {
                        Console.WriteLine("Специальность с таким названием уже существует.");
                        return false;
                    }

                    if (!IsValidRussianText(Name))
                    {
                        Console.WriteLine("Название специальности должно содержать только буквы.");
                        return false;
                    }

                    // Создать новую специальность
                    Models.Specialties newSpecialties = new Models.Specialties()
                    {
                        Code = Code,
                        Name = Name
                    };
                    db.context.Specialties.Add(newSpecialties);

                    int result = db.context.SaveChanges();
                    if (result > 0)
                    {
                        Console.WriteLine("Специальность добавлена!");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка {ex.Message}");
                return false;
            }
            return false;
        }

        public bool AddDisciplineButton_Click(string name, string theory, string practice, string myself, string coursework, string term)
        {
            //Добавление дисциплины
            try
            {
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(theory) || string.IsNullOrEmpty(practice) ||
                    string.IsNullOrEmpty(myself) || string.IsNullOrEmpty(coursework) || string.IsNullOrEmpty(term) == null)
                {
                    Console.WriteLine("Не все поля заполнены");
                    return false;
                }

                var NumberTerm = term;

                if (NumberTerm != "1" && NumberTerm != "2")
                {
                    Console.WriteLine("Семестр может быть только первым или вторым.");
                    return false;
                }

                if (!theory.All(char.IsDigit) || !practice.All(char.IsDigit) || !myself.All(char.IsDigit) || !coursework.All(char.IsDigit))
                {
                    Console.WriteLine("Часы должны содержать только цифры");
                    return false;
                }

                if (!IsValidRussianText(name))
                {
                    Console.WriteLine("Название дисциплины должно содержать только буквы.");
                    return false;
                }

                var newDiscipline = new Models.AcademicDisciplines
                {
                    Name_discipline = name,
                    Hours_theory = int.Parse(theory),
                    Hours_practice = int.Parse(practice),
                    Hours_independent = int.Parse(myself),
                    Hours_coursework = int.Parse(coursework),
                    Number_term = int.Parse(term)
                };

                Console.WriteLine("Дисциплина добавлена!");
                return true;
            }
            catch (Exception ex) { Console.WriteLine($"Произошла ошибика: {ex.Message}"); }
            return false;
        }
    }
}
