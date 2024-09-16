using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OlguSports.Models;

namespace OlguSports.Data
{
    public class CoursesDbContext : IdentityDbContext // Inherit from IdentityDbContext
    {
        public CoursesDbContext(DbContextOptions<CoursesDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Application> Applications { get; set; }
    }
}
