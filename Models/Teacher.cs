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
    public class Teacher : Record
    {

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Le courriel est obligatoire")]
        [EmailAddress(ErrorMessage = "Courriel invalide")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le téléphone est obligatoire")]
        public string Phone { get; set; }


        public string Code { get; set; }

        public DateTime StartDate { get; set; }


        const string Avatars_Folder = @"/App_Assets/Users/";
        const string Default_Avatar = @"no_avatar.png";
        public string Avatar { get; set; } = Avatars_Folder + Default_Avatar;

        [JsonIgnore]
        public string FullName => LastName + " " + FirstName;

        [JsonIgnore]
        public string Caption => Code + " " + FullName;

        [JsonIgnore]
        public double Seniority =>
            Math.Round((DateTime.Now - StartDate).TotalDays / 365.25, 1);

        [JsonIgnore]
        public List<Allocation> Allocations =>
            DB.Allocations.ToList().Where(a => a.TeacherId == Id).ToList();

        [JsonIgnore]
        public List<Allocation> NextSessionAllocations =>
            DB.Allocations.ToList()
              .Where(a => a.TeacherId == Id && a.IsNextSession)
              .ToList();

        [JsonIgnore]
        public List<Course> NextSessionCourses
        {
            get
            {
                var courses = new List<Course>();
                foreach (var alloc in NextSessionAllocations.OrderBy(a => a.Course.Code))
                    courses.Add(alloc.Course);
                return courses;
            }
        }

        [JsonIgnore]
        public SelectList NextSessionCoursesToSelectList =>
            SelectListUtilities<Course>.Convert(NextSessionCourses, "Caption");

        public void DeleteNextSessionAllocations()
        {
            foreach (Allocation alloc in NextSessionAllocations)
                DB.Allocations.Delete(alloc.Id);
        }

        public void UpdateAllocations(List<int> selectedCoursesId)
        {
            DeleteNextSessionAllocations();
            if (selectedCoursesId != null)
                foreach (int courseId in selectedCoursesId)
                    DB.Allocations.Add(new Allocation { TeacherId = Id, CourseId = courseId });
        }
    }
}