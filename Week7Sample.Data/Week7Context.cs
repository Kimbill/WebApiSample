using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Week7Sample.Model;

namespace Week7Sample.Data
{
    public class Week7Context : IdentityDbContext<User>
    {
        public Week7Context(DbContextOptions<Week7Context> options) : base(options)
        {

        }

        //public DbSet<User> Users { get; set; }
    }
}