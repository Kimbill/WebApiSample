using System.ComponentModel.DataAnnotations;
using Week7Sample.Model.Enums;

namespace Week7Sample.Model
{
    public class User
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(50, MinimumLength =5, ErrorMessage ="Length should be between 5 and 50 characters")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Length should be between 5 and 50 characters")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        //[DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public UserRole UserRole { get; set; }
    }
}