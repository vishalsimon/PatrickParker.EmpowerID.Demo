using Microsoft.EntityFrameworkCore;
using PatrickParker.EmpowerID.Demo.Models;

namespace PatrickParker.EmpowerID.Demo.Repository.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
