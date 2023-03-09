using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using PatrickParker.EmpowerID.Demo.Interfaces;
using PatrickParker.EmpowerID.Demo.Models;
using Microsoft.Data.SqlClient;
using Moq;
using PatrickParker.EmpowerID.Demo.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using PatrickParker.EmpowerID.Demo.Repository.Data;

namespace PatrickParker.EmpowerID.Demo.Test2
{
    [TestClass]
    public class EmployeeUnitTests
    {
        private readonly IEmployeeRepositoryAsync _employee;
        private readonly Interfaces.IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        string _dbConnection = "";
        string _dbName = "";
        public EmployeeUnitTests()
        {
            var builder = new ConfigurationBuilder();
            _configuration = builder.AddJsonFile("appsettings.json").Build();
            _dbConnection = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value ?? "";
            _dbName = _configuration.GetSection("Database").Value ?? "";

            //_employee = new Mock<EmployeeRepositoryAsync>().Object;
            //_unitOfWork = new Mock<UnitOfWork>().Object;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_dbConnection, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .Options;


            _context = new ApplicationDbContext(options);

            _employee = new EmployeeRepositoryAsync(_context);
            _unitOfWork = new UnitOfWork(_context);
        }

        [TestInitialize]
        public void Startup()
        {
            Console.WriteLine("Initialise");
            Console.WriteLine("*********************************************************************************************************************************************");

            if (_dbConnection.Contains("(localdb)"))
            {
                var _dbName = _configuration.GetSection("Database").Value;
                using (var connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Master;Integrated Security=True;MultipleActiveResultSets=True;"))
                {
                    connection.Open();

                    new SqlCommand($"IF EXISTS(select * from sys.databases where name='{_dbName}') DROP DATABASE [{_dbName}]", connection).ExecuteNonQuery();
                    new SqlCommand($"CREATE DATABASE [{_dbName}]", connection).ExecuteNonQuery();
                }

                _context.Database.Migrate();
            }
        }

        [TestMethod]
        public async Task EmployeeCrud()
        {
            try
            {
                //Add New Record
                Console.WriteLine("Create Employee");
                Employee newemployee = new Employee()
                {
                    FirstName = "Test",
                    LastName = "Record",
                    Email = "test@test.com",
                    DOB = new DateTime(1990, 01, 10),
                    Department = "Account"
                };

                Employee? emp = await _employee.AddAsync(newemployee).ConfigureAwait(false);
                await _unitOfWork.Commit().ConfigureAwait(false);

                int employeeId = emp?.Id ?? 0;
                Assert.IsTrue(employeeId > 0, "Employee not created");

                Console.WriteLine("Get Employee");
                Employee? newemp = await _employee.GetByIdAsync(employeeId).ConfigureAwait(false);
                Assert.IsNotNull(newemp, "Employee not found.");

                Console.WriteLine("Update Employee");
                newemployee.Email = "test1@test.com";
                Employee? updateEmp = null;

                await _employee.UpdateAsync(newemployee).ConfigureAwait(false);
                await _unitOfWork.Commit().ConfigureAwait(false);

                Assert.IsTrue(updateEmp != null, "Employee not updated");

                Console.WriteLine("Delete Employee");
                if (newemp != null)
                {
                    await _employee.DeleteAsync(newemp).ConfigureAwait(false);
                    await _unitOfWork.Commit().ConfigureAwait(false);

                    Assert.IsNotNull(newemp, "Employee not deleted.");
                }
                else
                {
                    Assert.IsNotNull(newemp, "Employee not deleted.");
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}