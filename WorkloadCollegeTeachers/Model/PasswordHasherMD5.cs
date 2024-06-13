using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace WorkloadCollegeTeachers.Model
{
    public class PasswordHasherMD5
    {
        // Метод для хэширования пароля на MD5
        public static string Md5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Преобразуем входную строку в массив байт и вычисляем хэш
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Создаем новую строку из массива байт
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        // Метод для проверки пароля
        public static bool VerifyMd5Hash(string input, string hash)
        {
            string hashOfInput = Md5Hash(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}
