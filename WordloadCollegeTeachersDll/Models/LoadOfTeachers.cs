//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WordloadCollegeTeachersDll.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class LoadOfTeachers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LoadOfTeachers()
        {
            this.Teachers = new HashSet<Teachers>();
        }
    
        public int Load_HoursID { get; set; }
        public string Name_Discipline { get; set; }
        public int Load_Hours { get; set; }
        public int Term_Work { get; set; }
        public int IDTeacher { get; set; }
        public Nullable<int> TeacherGroup { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Teachers> Teachers { get; set; }
    }
}
