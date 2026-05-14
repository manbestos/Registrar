using Controllers;
using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Utilities;

namespace Registrar.Controllers
{
    public class StudentsController : Controller
    {
        [AccessControl.UserAccess(Access.View)]
        public ActionResult Index()
        {
            List<Student> students = DB.Students.ToList();
            Session["StudentsYearsList"] = students
                    .Select(s => s.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToList();
            return View(students);
        }

        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetStudents()
        {
            if (!DB.Students.HasChanged && !DB.Registrations.HasChanged)
                return Content("blocked");

            List<Student> students = DB.Students.ToList();
            Session["StudentsYearsList"] = students
                   .Select(s => s.Year)
                   .Distinct()
                   .OrderByDescending(y => y)
                   .ToList();
            return PartialView("GetStudents", students);
        }

        [AccessControl.UserAccess(Access.View)]
        public ActionResult Details(int id)
        {
            Student student = DB.Students.Get(id);
            if (student == null)
                return RedirectToAction("Index");

            Session["id"] = id;
            Session["code"] = student.Code;
            return View("StudentsDetails", student);
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            Student student = DB.Students.Get(id);
            if (student == null)
                return RedirectToAction("Index");

            Session["id"] = id;
            Session["code"] = student.Code;

            ViewBag.Registrations = student.NextSessionCoursesToSelectList;
            ViewBag.Courses = SelectListUtilities<Course>.Convert(
                DB.Courses.ToList(), "Caption"
            );

            return View("StudentForm", student);
        }

        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(Student student, List<int> selectedCoursesId)
        {
            if (ModelState.IsValid)
            {
                student.Id = (int)Session["id"];
                student.Code = (string)Session["code"];
                student.UpdateRegistrations(selectedCoursesId);
                DB.Students.Update(student);
                return RedirectToAction("Details", new { id = student.Id });
            }
            return View("StudentForm", student);
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Create()
        {
            Student student = new Student
            {
                Code = GenerateStudentCode(),
                BirthDate = DateTime.Now.AddYears(-20)
            };

            ViewBag.Registrations = new SelectList(new List<Course>(), "Id", "Caption");
            ViewBag.Courses = SelectListUtilities<Course>.Convert(
                DB.Courses.ToList(), "Caption"
            );

            return View("StudentForm", student);
        }

        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Create(Student student, List<int> selectedCoursesId)
        {
            if (ModelState.IsValid && !student.Code.Equals(""))
            {
                DB.Students.Add(student);

                if (selectedCoursesId != null)
                {
                    foreach (int courseId in selectedCoursesId)
                    {
                        DB.Registrations.Add(new Registration
                        {
                            StudentId = student.Id,
                            CourseId = courseId
                        });
                    }
                }

                return RedirectToAction("Details", new { id = student.Id });
            }
            

            return View("StudentForm", student);
        }

        [AccessControl.UserAccess(Access.Admin)]
        public ActionResult Delete(int id)
        {
            Student student = DB.Students.Get(id);
            if (student != null)
            {
                student.DeleteAllRegistrations();
                DB.Students.Delete(id);
            }
            return RedirectToAction("Index");
        }

        private string GenerateStudentCode()
        {
            Random random = new Random();
            int year = DateTime.Now.Year;
            string code;
            do
            {
                int randomNum = random.Next(100000, 999999);
                code = year + randomNum.ToString();
            }
            while (DB.Students.ToList().Find(s => s.Code == code) != null);

            return code;
        }
        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetStudentRegistrations(int id)
        {
            Student student = DB.Students.Get(id);
            if (student == null) return new EmptyResult();
            return PartialView("GetStudentRegistrations", student);
        }
        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetTeacherAllocations(int id)
        {
            Teacher teacher = DB.Teachers.Get(id);
            if (teacher == null) return new EmptyResult();
            return PartialView("GetTeacherAllocations", teacher);
        }
    }

}