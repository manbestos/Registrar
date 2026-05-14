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
    public class TeachersController : Controller
    {
        [AccessControl.UserAccess(Access.View)]
        public ActionResult Index()
        {
            List<Teacher> teachers = DB.Teachers.ToList()
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToList();
            return View(teachers);
        }

        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetTeachers()
        {
            List<Teacher> teachers = DB.Teachers.ToList()
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToList();
            return PartialView("GetTeachers", teachers);
        }

        [AccessControl.UserAccess(Access.View)]
        public ActionResult Details(int id)
        {
            Teacher teacher = DB.Teachers.Get(id);
            if (teacher == null)
                return RedirectToAction("Index");

            Session["id"] = id;
            Session["teacherCode"] = teacher.Code;
            return View("TeachersDetails", teacher);
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            Teacher teacher = DB.Teachers.Get(id);
            if (teacher == null)
                return RedirectToAction("Index");

            Session["id"] = id;
            Session["teacherCode"] = teacher.Code;

            ViewBag.AllocatedCourses = teacher.NextSessionCoursesToSelectList;
            ViewBag.AllCourses = SelectListUtilities<Course>.Convert(
                DB.Courses.ToList(), "Caption"
            );

            return View("TeacherForm", teacher);
        }

        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(Teacher teacher, List<int> selectedCoursesId)
        {
            if (ModelState.IsValid)
            {
                teacher.Id = (int)Session["id"];
                teacher.Code = (string)Session["teacherCode"];
                teacher.UpdateAllocations(selectedCoursesId);
                DB.Teachers.Update(teacher);
                return RedirectToAction("Details", new { id = teacher.Id });
            }
            return View("TeacherForm", teacher);
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Create()
        {
            Teacher teacher = new Teacher
            {
                Code = GenerateTeacherCode(),
                StartDate = DateTime.Now
            };

            ViewBag.AllocatedCourses = new SelectList(new List<Course>(), "Id", "Caption");
            ViewBag.AllCourses = SelectListUtilities<Course>.Convert(
                DB.Courses.ToList(), "Caption"
            );

            return View("TeacherForm", teacher);
        }

        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Create(Teacher teacher, List<int> selectedCoursesId)
        {
            if (ModelState.IsValid && !teacher.Code.Equals(""))
            {
                DB.Teachers.Add(teacher);

                if (selectedCoursesId != null)
                {
                    foreach (int courseId in selectedCoursesId)
                    {
                        DB.Allocations.Add(new Allocation
                        {
                            TeacherId = teacher.Id,
                            CourseId = courseId
                        });
                    }
                }

                return RedirectToAction("Details", new { id = teacher.Id });
            }
            return View("TeacherForm", teacher);
        }

        [HttpPost]
        [AccessControl.UserAccess(Access.Admin)]
        public ActionResult Delete(int id)
        {
            Teacher teacher = DB.Teachers.Get(id);
            if (teacher != null)
            {
                teacher.DeleteNextSessionAllocations();
                DB.Teachers.Delete(id);
            }
            return RedirectToAction("Index");
        }

        private string GenerateTeacherCode()
        {
            Random random = new Random();
            string code;
            do
            {
                int randomNum = random.Next(10000, 99999);
                code = $"CLG-420-{randomNum}";
            }
            while (DB.Teachers.ToList().Find(t => t.Code == code) != null);

            return code;
        }
    }
}