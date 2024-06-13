using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkloadCollegeTeachers.Model
{
    public partial class Groups
    {
        Core db = new Core();
        public string SpecialtyName
        {
            get
            {
                if (this.Specialties != null)
                {
                    return this.Specialties.Name;
                }
                return string.Empty;
            }
        }
        public string GroupName
        {
            get
            {
                if (this.Name_group != null)
                {
                    return this.Name_group;
                }
                return string.Empty;
            }
        }
    }
}
