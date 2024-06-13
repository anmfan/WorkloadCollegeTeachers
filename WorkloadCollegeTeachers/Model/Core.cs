using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkloadCollegeTeachers.Model;

namespace WorkloadCollegeTeachers
{
    public class Core
    {
        public MainCollegeEntities context = new MainCollegeEntities();

        public List<Specialties> GetSpecialties()
        {
            List<Specialties> spec = new List<Specialties>();

            using (var db = new MainCollegeEntities()) 
            {
                spec = db.Specialties.ToList();
            }
            return spec;
        }

        public Specialties GetSpecialtyId(int id)
        {
            using (var db = new MainCollegeEntities()) 
            {
                return db.Specialties.FirstOrDefault(s => s.specialties_id == id);
            }
        }

        public List<Teachers> GetTeachers()
        {
            using (var db = new MainCollegeEntities()) 
            {
                return db.Teachers.ToList();
            }
        }
    }
}
