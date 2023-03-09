using Microsoft.EntityFrameworkCore;
using PatrickParker.EmpowerID.Demo.Interfaces;
using PatrickParker.EmpowerID.Demo.Models;
using PatrickParker.EmpowerID.Demo.Repository.Data;

namespace PatrickParker.EmpowerID.Demo.Repository.Repositories
{
    public class EmployeeRepositoryAsync : GenericRepositoryAsync<Employee>, IEmployeeRepositoryAsync
    {
        private readonly DbSet<Employee> _employee;

        public EmployeeRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _employee = dbContext.Set<Employee>();
        }
    }
}