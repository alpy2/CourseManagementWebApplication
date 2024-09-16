using System.ComponentModel.DataAnnotations;

namespace OlguSports.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public int Duration { get; set; } 
    }
}
