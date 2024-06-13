using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkloadCollegeTeachers.Model
{
    partial class Teachers
    {

        public string fio
        {
            //Класс для объединения строк имении, фамилии и отчества
            get
            {
                return $"{Surname} {Name} {Patronymic}";
            }
        }

        [NotMapped]
        public string SpecialtyName { get; internal set; }

    }
}
