using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week7Sample.Model
{
    public class UpdateUserDto
    {
        //public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
    }
}
