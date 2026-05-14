using Controllers;
using DAL;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Utilities;

namespace Registrar.Controllers
{
    public class CoursesController : Controller
    {
        [AccessControl.UserAccess(Access.View)]
        public ActionResult Index()
        {
            List<Course> courses = DB.Courses.ToList().OrderBy(c => c.Session).ToList();
            return View(courses);
        }

        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetCourses()
        {
            if (!DB.Courses.HasChanged && !DB.Registrations.HasChanged)
                return Content("blocked");

            List<Course> courses = DB.Courses.ToList().OrderBy(c => c.Session).ToList();
            return PartialView("GetCourses", courses);
        }

        [AccessControl.UserAccess(Access.View)]
        public ActionResult Details(int id)
        {
            Course course = DB.Courses.Get(id);
            if (course == null)
                return RedirectToAction("Index");

            Session["id"] = id;
            Session["courseCode"] = course.Code;
            return View("CourseDetails", course);
        }

        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            Course course = DB.Courses.Get(id);
            if (course == null)
                return RedirectToAction("Index");

            Session["id"] = id;
            Session["courseCode"] = course.Code;

            ViewBag.SelectedStudents = course.NextSessionStudentsToSelectList;
            ViewBag.AllStudents = SelectListUtilities<Student>.Convert(
                DB.Students.ToList(), "Caption"
            );

            return View("CourseForm", course);
        }

        [HttpPost]
        [AccessControl.UserAccess(Access.Write)]
        public ActionResult Edit(Course course, List<int> selectedStudentsId)
        {
            if (ModelState.IsValid)
            {
                course.Id = (int)Session["id"];
                course.Code = (string)Session["courseCode"];

                List<Registration> nextSessionRegs = DB.Registrations.ToList()
                    .Where(r => r.CourseId == course.Id && r.IsNextSession)
                    .ToList();

                foreach (Registration reg in nextSessionRegs)
                    DB.Registrations.Delete(reg.Id);

                if (selectedStudentsId != null)
                {
                    foreach (int studentId in selectedStudentsId)
                    {
                        DB.Registrations.Add(new Registration
                        {
                            StudentId = studentId,
                            CourseId = course.Id
                        });
                    }
                }

                DB.Courses.Update(course);
                return RedirectToAction("Details", new { id = course.Id });
            }
            return View("CourseForm", course);
        }


        [HttpPost]
        [AccessControl.UserAccess(Access.Admin)]
        public ActionResult Delete(int id)
        {
            Course course = DB.Courses.Get(id);
            if (course != null)
            {
                foreach (Registration reg in course.Registrations)
                    DB.Registrations.Delete(reg.Id);

                foreach (Allocation alloc in course.Allocations)
                    DB.Allocations.Delete(alloc.Id);

                DB.Courses.Delete(id);
            }
            return RedirectToAction("Index");
        }
        [AccessControl.UserAccess(Access.View)]
        public ActionResult GetCourseRegistrations(int id)
        {
            Course course = DB.Courses.Get(id);
            if (course == null) return new EmptyResult();
            return PartialView("GetCourseRegistrations", course);
        }
    }

}