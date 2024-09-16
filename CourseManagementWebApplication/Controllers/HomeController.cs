using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OlguSports.Data;
using OlguSports.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OlguSports.Controllers
{
    public class HomeController : Controller
    {
        private readonly CoursesDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(CoursesDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var courses = _context.Courses.ToList(); // Fetch all courses
            return View(courses); // Ensure that the view exists and is correctly set up
        }

        [Authorize]
        [HttpGet]
        public IActionResult Apply()
        {
            ViewBag.Courses = _context.Courses.ToList(); // Fetch courses for the dropdown
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Apply(Application application)
        {
            if (application.Age < 18 || application.Age > 100)
            {
                TempData["ErrorMessage"] = "Failed to apply. Age must be between 18 and 100.";
                ViewBag.Courses = _context.Courses.ToList();
                return View(application);
            }

            if (application.CourseId == 0)
            {
                TempData["ErrorMessage"] = "Failed to apply. You must select a course.";
                ViewBag.Courses = _context.Courses.ToList();
                return View(application);
            }
            
            try
            {
                application.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Applications.Add(application);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Success! Your application has been submitted.";
                return RedirectToAction("Apply");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing the application");
                TempData["ErrorMessage"] = "An unexpected error occurred while processing your application. Please try again later.";
            }

            ViewBag.Courses = _context.Courses.ToList();
            return View(application);
        }

        [Authorize]
        [HttpGet]
        public IActionResult MyApplications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applications = _context.Applications
                .Where(a => a.UserId == userId)
                .Include(a => a.Course) // Ensure courses are included in the query
                .ToList();
            
            return View(applications);
        }
    }
}
