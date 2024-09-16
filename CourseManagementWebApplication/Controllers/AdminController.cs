using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlguSports.Data;
using OlguSports.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OlguSports.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly CoursesDbContext _context;

        public AdminController(CoursesDbContext context)
        {
            _context = context;
        }

        // Display all courses
        [HttpGet]
        public IActionResult ViewCourses()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }

        // Display all applications
        [HttpGet]
        public IActionResult ViewApplications()
        {
            var applications = _context.Applications
                .Include(a => a.Course)
                .ToList();
            return View(applications);
        }

        // Approve an application
        [HttpPost]
        public async Task<IActionResult> ApproveApplication(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application != null)
            {
                application.Status = ApplicationStatus.Approved; // Set status to Approved
                _context.Applications.Update(application);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ViewApplications");
        }

        // Reject an application
        [HttpPost]
        public async Task<IActionResult> RejectApplication(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application != null)
            {
                application.Status = ApplicationStatus.Rejected; // Set status to Rejected
                _context.Applications.Update(application);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ViewApplications");
        }

        // Add a new course
        [HttpGet]
        public IActionResult AddCourse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewCourses");
            }
            return View(course);
        }

        // Delete a course
        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ViewCourses");
        }
    }
}
