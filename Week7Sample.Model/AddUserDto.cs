using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week7Sample.Model.Enums;

namespace Week7Sample.Model
{
    public class AddUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Length should be between 5 and 50 characters")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Length should be between 5 and 50 characters")]
        public string LastName { get; set; }

        [Required]
        public UserRole UserRole { get; set; }
    }
}
