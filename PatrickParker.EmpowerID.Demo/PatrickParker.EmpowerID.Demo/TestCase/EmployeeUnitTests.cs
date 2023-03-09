using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using PatrickParker.EmpowerID.Demo.Interfaces;
using Microsoft.Data.SqlClient;
using PatrickParker.EmpowerID.Demo.Models;

namespace PatrickParker.EmpowerID.Demo.Test
{
    [TestClass]
    public class EmployeeUnitTests
    {
        private readonly IEmployeeRepositoryAsync _employee;
        private readonly Interfaces.IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        string _dbConnection = "";
        public EmployeeUnitTests(IEmployeeRepositoryAsync employee, IUnitOfWork unitOfWork)
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _dbConnection = _configuration.GetSection("ConnectionStrings").GetSection("WebappContextConnection").Value ?? "";

            _employee = employee;
            _unitOfWork = unitOfWork;
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
            }
        }

        [TestMethod]
        public void EmployeeCrud()
        {
            int employeeId = 0;

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

            Employee? emp = null;
            var task = Task.Run(async () => emp = await _employee.AddAsync(newemployee));
            task.Wait();

            var committask = Task.Run(async () => await _unitOfWork.Commit());
            committask.Wait();

            if (emp != null)
            {
                employeeId = emp.Id;
            }
            Assert.IsTrue(employeeId > 0, "Employee not created");

            Console.WriteLine("Get Employee");
            Employee? newemp = null;
            task = Task.Run(async () => newemp = await _employee.GetByIdAsync(employeeId));
            task.Wait();
            Assert.IsNotNull(newemp, "Employee not found.");

            Console.WriteLine("Update Employee");
            newemployee.Email = "test1@test.com";
            Employee? updateEmp = null;

            var updatetask = Task.Run(async () => await _employee.UpdateAsync(newemployee));
            updatetask.Wait();

            committask = Task.Run(async () => await _unitOfWork.Commit());
            committask.Wait();
            
            Assert.IsTrue(updateEmp != null, "Employee not updated");

            Console.WriteLine("Delete Employee");
            if (newemp != null)
            {
                var deletetask = Task.Run(async () => await _employee.DeleteAsync(newemp));
                deletetask.Wait();

                committask = Task.Run(async () => await _unitOfWork.Commit());
                committask.Wait();
                
                Assert.IsNotNull(newemp, "Employee not deleted.");
            }
            else
            {
                Assert.IsNotNull(newemp, "Employee not deleted.");
            }
        }
    }
}