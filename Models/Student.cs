// Models/Student.cs
using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Utilities;

namespace Models
{
    public class Student : Record
    {
        [Required(ErrorMessage = "Le prénom est obligatoire")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "La date de naissance est obligatoire")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Le courriel est obligatoire")]
        [EmailAddress(ErrorMessage = "Courriel invalide")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le téléphone est obligatoire")]
        public string Phone { get; set; }

        public string Code { get; set; }

       

        [JsonIgnore]
        public string FullName => LastName + " " + FirstName;

        [JsonIgnore]
        public string Caption => Code + " " + FullName;

        [JsonIgnore]
        public int Year => int.Parse(Code.Substring(0, 4));

        [JsonIgnore]
        public List<Registration> Registrations =>
            DB.Registrations.ToList().Where(r => r.StudentId == Id).ToList();

        [JsonIgnore]
        public List<Registration> NextSessionRegistrations =>
            DB.Registrations.ToList()
              .Where(r => r.StudentId == Id && r.IsNextSession)
              .ToList();

        [JsonIgnore]
        public List<Course> Courses
        {
            get
            {
                var courses = new List<Course>();
                foreach (var reg in Registrations.OrderBy(r => r.Course.Code))
                    courses.Add(reg.Course);
                return courses;
            }
        }

        [JsonIgnore]
        public List<Course> NextSessionCourses
        {
            get
            {
                var courses = new List<Course>();
                foreach (var reg in NextSessionRegistrations.OrderBy(r => r.Course.Code))
                    courses.Add(reg.Course);
                return courses;
            }
        }

        [JsonIgnore]
        public SelectList CoursesSelectList =>
            SelectListUtilities<Course>.Convert(Courses, "Caption");

        [JsonIgnore]
        public SelectList NextSessionCoursesToSelectList =>
            SelectListUtilities<Course>.Convert(NextSessionCourses, "Caption");

        public void DeleteAllRegistrations()
        {
            foreach (Registration reg in Registrations)
                DB.Registrations.Delete(reg.Id);
        }

        public void DeleteNextSessionRegistrations()
        {
            foreach (Registration reg in NextSessionRegistrations)
                DB.Registrations.Delete(reg.Id);
        }

        public void UpdateRegistrations(List<int> selectedCoursesId)
        {
            DeleteNextSessionRegistrations();
            if (selectedCoursesId != null)
                foreach (int courseId in selectedCoursesId)
                    DB.Registrations.Add(new Registration { StudentId = Id, CourseId = courseId });
        }
    }
}