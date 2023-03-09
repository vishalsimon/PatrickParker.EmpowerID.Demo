using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatrickParker.EmpowerID.Demo.Interfaces;
using PatrickParker.EmpowerID.Demo.Models;
using PatrickParker.EmpowerID.Demo.Web.Services;

namespace PatrickParker.EmpowerID.Demo.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class RestAPIController : ControllerBase
    {
        private readonly IEmployeeRepositoryAsync _employee;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRazorRenderService _renderService;
        private readonly ILogger<RestAPIController> _logger;

        public RestAPIController(ILogger<RestAPIController> logger, IEmployeeRepositoryAsync employee, 
                                        IUnitOfWork unitOfWork, IRazorRenderService renderService)
        {
            _logger = logger;
            _employee = employee;
            _unitOfWork = unitOfWork;
            _renderService = renderService;
        }
        public IEnumerable<Employee> Employees { get; set; }
        [HttpGet]
        public async Task<ActionResult<Employee>> Get(int id)
        {
            var employee = new Employee();
            try
            {
                if (id > 0)
                {
                    employee = await _employee.GetByIdAsync(id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("On Get Employee: " + ex.Message);
            }
            return Ok(employee);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Employee>>> GetList(int id)
        {
            try
            {
                if (id > 0)
                {
                    Employees = await _employee.GetAllAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("On Get List Employee: " + ex.Message);
            }
            return Ok(Employees);
        }

        public async Task<JsonResult> Post(int id, Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == 0)
                    {
                        await _employee.AddAsync(employee);
                        await _unitOfWork.Commit();
                    }
                    else
                    {
                        await _employee.UpdateAsync(employee);
                        await _unitOfWork.Commit();
                    }
                    Employees = await _employee.GetAllAsync();
                    var html = await _renderService.ToStringAsync("_ViewAll", Employees);
                    return new JsonResult(new { isValid = true, html = html });
                }
                else
                {
                    var html = await _renderService.ToStringAsync("_CreateOrEdit", employee);
                    return new JsonResult(new { isValid = false, html = html });
                }
            }
            catch(Exception ex)
            {
                _logger.LogInformation("On Post Employee: " + ex.Message);
            }
            return new JsonResult(new { isValid = false, html = "" });
        }

        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var employee = await _employee.GetByIdAsync(id);
                await _employee.DeleteAsync(employee);
                await _unitOfWork.Commit();
                Employees = await _employee.GetAllAsync();
                var html = await _renderService.ToStringAsync("_ViewAll", Employees);
                return new JsonResult(new { isValid = true, html = html });
            }
            catch (Exception ex)
            {
                _logger.LogInformation("On Delete Employee: " + ex.Message);
            }
            return new JsonResult(new { isValid = false, html = "" });
        }
    }
}