using System.ComponentModel.DataAnnotations;

namespace PatrickParker.EmpowerID.Demo.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [MinLength(2)]
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        [Required]
        public DateTime? DOB { get; set; }
        public string Department { get; set; } = "";
    }
}
