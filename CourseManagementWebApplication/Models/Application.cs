using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OlguSports.Models
{
    public class Application
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Course selection is required.")]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        public Course Course { get; set; }

        // Property to store the ID of the user who submitted the application
        public string UserId { get; set; }

        // Property to track the status of the application (Pending, Approved, Rejected)
        [Required]
        [Display(Name = "Application Status")]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending; // Default status set to Pending
    }

    // Enum to represent the status of the application
    public enum ApplicationStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
