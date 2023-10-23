using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week7Sample.Model
{
    public class AddRoleDto
    {
        public List<string> Roles { get; set; }

        public AddRoleDto()
        {
            Roles = new List<string>();
        }
    }
}
