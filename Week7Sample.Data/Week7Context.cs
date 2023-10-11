using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Week7Sample.Model;

namespace Week7Sample.Data
{
    public class Week7Context : DbContext
    {
        public Week7Context(DbContextOptions<Week7Context> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}