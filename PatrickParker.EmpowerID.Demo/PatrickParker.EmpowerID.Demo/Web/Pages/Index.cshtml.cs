using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PatrickParker.EmpowerID.Demo.Interfaces;
using PatrickParker.EmpowerID.Demo.Models;
using PatrickParker.EmpowerID.Demo.Web.Services;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IEmployeeRepositoryAsync _employeeRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRazorRenderService _renderService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, IEmployeeRepositoryAsync employeeRepo, IUnitOfWork unitOfWork, IRazorRenderService renderService)
        {
            _logger = logger;
            _employeeRepo = employeeRepo;
            _unitOfWork = unitOfWork;
            _renderService = renderService;
        }
        public IEnumerable<Employee> Employees { get; set; }
        public void OnGet()
        {
        }
        public async Task<PartialViewResult> OnGetViewAllPartial()
        {
            try
            {
                Employees = await _employeeRepo.GetAllAsync();
                return new PartialViewResult
                {
                    ViewName = "_ViewAll",
                    ViewData = new ViewDataDictionary<IEnumerable<Employee>>(ViewData, Employees)
                };
            }
            catch (Exception ex)
            {
                return new PartialViewResult
                {
                    ViewName = "_ViewAll",
                    ViewData = new ViewDataDictionary<IEnumerable<Employee>>(ViewData, Employees)
                };
            }
        }
        public async Task<JsonResult> OnGetCreateOrEditAsync(int id = 0)
        {
            if (id == 0)
                return new JsonResult(new { isValid = true, html = await _renderService.ToStringAsync("_CreateOrEdit", new Employee()) });
            else
            {
                var thisEmployee = await _employeeRepo.GetByIdAsync(id);
                return new JsonResult(new { isValid = true, html = await _renderService.ToStringAsync("_CreateOrEdit", thisEmployee) });
            }
        }
        public async Task<JsonResult> OnPostCreateOrEditAsync(int id, Employee customer)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    await _employeeRepo.AddAsync(customer);
                    await _unitOfWork.Commit();
                }
                else
                {
                    await _employeeRepo.UpdateAsync(customer);
                    await _unitOfWork.Commit();
                }
                Employees = await _employeeRepo.GetAllAsync();
                var html = await _renderService.ToStringAsync("_ViewAll", Employees);
                return new JsonResult(new { isValid = true, html = html });
            }
            else
            {
                var html = await _renderService.ToStringAsync("_CreateOrEdit", customer);
                return new JsonResult(new { isValid = false, html = html });
            }
        }
        public async Task<JsonResult> OnPostDeleteAsync(int id)
        {
            var employee = await _employeeRepo.GetByIdAsync(id);
            if (employee != null)
            {
                await _employeeRepo.DeleteAsync(employee);
                await _unitOfWork.Commit();
            }

            Employees = await _employeeRepo.GetAllAsync();
            var html = await _renderService.ToStringAsync("_ViewAll", Employees);
            return new JsonResult(new { isValid = true, html = html });
        }
    }
}
