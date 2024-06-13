using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WordloadCollegeTeachersDll;
//using WordloadCollegeTeachersDll.Models;
//using WorkloadCollegeTeachersTests.Models;

namespace WorkloadCollegeTeachersTests
{
    [TestClass]
    public class WorkloadCollegeTeachersTests
    {
        /// <summary>
        /// Авторизация существующих данных
        /// </summary>
        [TestMethod]
        public void AuthCheck_ExistData_ReturnedTrue()
        {
            //Arrange
            string Login = "12";
            string Password = "12";

            //Act
            WordloadCollegeTeachersTestsDll obj = new WordloadCollegeTeachersTestsDll();
            bool result = obj.AuthButton_Click(Login, Password);

            //Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Авторизация несуществующих данных
        /// </summary>
        [TestMethod]
        public void AuthCheck_NotExistData_ReturnedFalse()
        {
            //Arrange
            string Login = "123gfrew";
            string Password = "12wg/ra";

            //Act
            WordloadCollegeTeachersTestsDll obj = new WordloadCollegeTeachersTestsDll();
            bool result = obj.AuthButton_Click(Login, Password);

            //Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Добавление корректных данных в специальность
        /// </summary>
        [TestMethod]
        public void SpecCheck_CorrectData_ReturnedTrue()
        {
            //Arrange
            string Code = "141531";
            string Name = "Лесостройка";

            //Act
            WordloadCollegeTeachersTestsDll obj = new WordloadCollegeTeachersTestsDll();
            bool result = obj.SpecialitiesAddButton_Click(Code, Name);

            //Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Добавление некорректных данных в специальность (буквы в коде)
        /// </summary>
        [TestMethod]
        public void SpecCheck_NotCorrectData_ReturnedFalse()
        {
            //Arrange
            string Code = "gfga";
            string Name = "Лесостройка";

            //Act
            WordloadCollegeTeachersTestsDll obj = new WordloadCollegeTeachersTestsDll();
            bool result = obj.SpecialitiesAddButton_Click(Code, Name);

            //Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Добавление некорректных данных в специальность (много цифр)
        /// </summary>
        [TestMethod]
        public void SpecCheck_NotCorrectDigits_ReturnedFalse()
        {
            //Arrange
            string Code = "1414190";
            string Name = "Лесостройка";

            //Act
            WordloadCollegeTeachersTestsDll obj = new WordloadCollegeTeachersTestsDll();
            bool result = obj.SpecialitiesAddButton_Click(Code, Name);

            //Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Добавление некорректных данных в специальность (цифры в названии)
        /// </summary>
        [TestMethod]
        public void SpecCheck_NotCorrectName_ReturnedFalse()
        {
            //Arrange
            string Code = "141418";
            string Name = "Лесостройка42";

            //Act
            WordloadCollegeTeachersTestsDll obj = new WordloadCollegeTeachersTestsDll();
            bool result = obj.SpecialitiesAddButton_Click(Code, Name);

            //Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Добавление корректных данных в дисциплину 
        /// </summary>
        [TestMethod]
        public void DiscCheck_CorrectName_ReturnedTrue()
        {
            //Arrange
            string name = "Математика";
            string theory = "34";
            string practice = "22";
            string myself = "11";
            string coursework = "55";
            string term = "1";

            //Act
            WordloadCollegeTeachersTestsDll obj = new WordloadCollegeTeachersTestsDll();
            bool result = obj.AddDisciplineButton_Click(name, theory, practice, myself, coursework, term);

            //Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Добавление некорректных данных в дисциплину (буквы в имени)
        /// </summary>
        [TestMethod]
        public void DiscCheck_NotCorrectName_ReturnedFalse()
        {
            //Arrange
            string name = "Математика423";
            string theory = "34";
            string practice = "22";
            string myself = "11";
            string coursework = "55";
            string term = "1";

            //Act
            WordloadCollegeTeachersTestsDll obj = new WordloadCollegeTeachersTestsDll();
            bool result = obj.AddDisciplineButton_Click(name, theory, practice, myself, coursework, term);

            //Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Добавление некорректных данных в дисциплину (семестр, не равный 1 или 2)
        /// </summary>
        [TestMethod]
        public void DiscCheck_NotCorrectTerm_ReturnedFalse()
        {
            //Arrange
            string name = "Математика";
            string theory = "34";
            string practice = "22";
            string myself = "11";
            string coursework = "55";
            string term = "3";

            //Act
            WordloadCollegeTeachersTestsDll obj = new WordloadCollegeTeachersTestsDll();
            bool result = obj.AddDisciplineButton_Click(name, theory, practice, myself, coursework, term);

            //Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Добавление некорректных данных в дисциплину (буквы в часах)
        /// </summary>
        [TestMethod]
        public void DiscCheck_NotCorrectHours_ReturnedFalse()
        {
            //Arrange
            string name = "Математика";
            string theory = "34gfsd";
            string practice = "22";
            string myself = "11/";
            string coursework = "55";
            string term = "1";

            //Act
            WordloadCollegeTeachersTestsDll obj = new WordloadCollegeTeachersTestsDll();
            bool result = obj.AddDisciplineButton_Click(name, theory, practice, myself, coursework, term);

            //Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Добавление некорректных данных в дисциплину (пустое поле)
        /// </summary>
        [TestMethod]
        public void DiscCheck_NotCorrectSpace_ReturnedFalse()
        {
            //Arrange
            string name = "";
            string theory = "33";
            string practice = "22";
            string myself = "11";
            string coursework = "55";
            string term = "1";

            //Act
            WordloadCollegeTeachersTestsDll obj = new WordloadCollegeTeachersTestsDll();
            bool result = obj.AddDisciplineButton_Click(name, theory, practice, myself, coursework, term);

            //Assert
            Assert.IsFalse(result);
        }
    }
}
