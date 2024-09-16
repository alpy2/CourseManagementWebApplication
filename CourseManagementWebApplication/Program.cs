using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OlguSports.Data;
using OlguSports.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Configure the in-memory database
builder.Services.AddDbContext<CoursesDbContext>(options =>
    options.UseInMemoryDatabase("OlguSportsDb"));

// Configure Identity with role management
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Simplified for testing
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<CoursesDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed data into the in-memory database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider; // Correctly getting the service provider from the scope
    var context = services.GetRequiredService<CoursesDbContext>();

    // Seed Courses if they don't exist
    if (!context.Courses.Any())
    {
        context.Courses.AddRange(
            new Course { Id = 1, Name = "Basketball Basics", Description = "Learn the fundamentals of basketball. Unleash your inner Derrick Rose.", Duration = 10 },
            new Course { Id = 2, Name = "Advanced Football", Description = "Improve your football skills. Become the next Lionel Messi.", Duration = 15 }
        );
        context.SaveChanges();
    }

    // Seed Applications if they don't exist
    if (!context.Applications.Any())
    {
        context.Applications.AddRange(
            new Application { Name = "Ali", Surname = "Bey", Email = "ali.bey@example.com", Age = 25, CourseId = 1, UserId = "dummy-user-id-1", Status = ApplicationStatus.Pending },
            new Application { Name = "Ajda", Surname = "Hanım", Email = "ajda.hanim@example.com", Age = 30, CourseId = 2, UserId = "dummy-user-id-2", Status = ApplicationStatus.Pending },
            new Application { Name = "Hakan", Surname = "Unal", Email = "hakan.unal@example.com", Age = 22, CourseId = 1, UserId = "dummy-user-id-3", Status = ApplicationStatus.Pending }
        );
        context.SaveChanges();
    }

    // Seed roles and an admin user
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // Create Admin role if it doesn't exist
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Create an admin user if it doesn't exist
    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // Create a dummy user for testing
    var userEmail = "user@example.com";
    var dummyUser = await userManager.FindByEmailAsync(userEmail);
    if (dummyUser == null)
    {
        dummyUser = new IdentityUser { UserName = userEmail, Email = userEmail };
        var result = await userManager.CreateAsync(dummyUser, "User123!");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
    