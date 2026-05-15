
using DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Utilities;


namespace Models
{
    public class Course : Record
    {
  
        public string Code { get; set; }


        public string Title { get; set; }


        public int Session { get; set; }

        [JsonIgnore]
        public string Caption => $"[{Session}] {Code} {Title}";

        [JsonIgnore]
        public List<Registration> Registrations =>
            DB.Registrations.ToList().Where(r => r.CourseId == Id).ToList();

        [JsonIgnore]
        public List<Allocation> Allocations =>
            DB.Allocations.ToList().Where(a => a.CourseId == Id).ToList();

        [JsonIgnore]
        public List<Registration> NextSessionRegistrations =>
            DB.Registrations.ToList()
              .Where(r => r.CourseId == Id && r.IsNextSession)
              .ToList();

        [JsonIgnore]
        public List<Student> NextSessionStudents
        {
            get
            {
                var students = new List<Student>();
                foreach (var reg in NextSessionRegistrations.OrderBy(r => r.Student.LastName))
                    students.Add(reg.Student);
                return students;
            }
        }

        public Teacher GetTeacherForYear(int year)
        {
            var alloc = DB.Allocations.ToList()
                .FirstOrDefault(a => a.CourseId == Id && a.Year == year);
            return alloc?.Teacher;
        }

        [JsonIgnore]
        public SelectList NextSessionStudentsToSelectList =>
            SelectListUtilities<Student>.Convert(NextSessionStudents, "Caption");
    }
}