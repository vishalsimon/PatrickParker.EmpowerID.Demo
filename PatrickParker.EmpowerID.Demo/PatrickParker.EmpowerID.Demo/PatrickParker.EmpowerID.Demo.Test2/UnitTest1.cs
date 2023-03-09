using Microsoft.Extensions.Configuration;
using PatrickParker.EmpowerID.Demo.Interfaces;

namespace PatrickParker.EmpowerID.Demo.Test2
{
    [TestClass]
    public class UnitTest1
    {
        private readonly IEmployeeRepositoryAsync _employee;
        private readonly Interfaces.IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        string _dbConnection = "";

        //public UnitTest1(IEmployeeRepositoryAsync employee, IUnitOfWork unitOfWork)
        //{
        //    _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        //    _dbConnection = _configuration.GetSection("ConnectionStrings").GetSection("WebappContextConnection").Value ?? "";

        //    _employee = employee;
        //    _unitOfWork = unitOfWork;
        //}

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}