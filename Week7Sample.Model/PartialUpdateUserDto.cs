using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week7Sample.Model
{
    public class PartialUpdateUserDto
    {

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Length should be between 5 and 50 characters")]
        public string LastName { get; set; }
    }
}
