using Microsoft.AspNetCore.Mvc;
using System.Data;
using Npgsql;

using Dashboard.Models.Utility;
using Dashboard.Models.Service;
using Dashboard.Models.Item;

namespace Dashboard.Controllers;
public class ApiController(IConfiguration conf) : BaseController(conf)
{
    // ---------------------------------------------------------------------
    // Field
    // ---------------------------------------------------------------------
    private readonly Logger _logger = Logger.GetInstance(conf);

    // ---------------------------------------------------------------------
    // [GET] api/salary
    // ---------------------------------------------------------------------
    [Route("api/salary/{month?}")]
    public IActionResult Saraly(string? month, string? keyword) {
        ApiResponseJson? response = null;
        try {
            using (PostgresqlWorker worker = new(base.DB_HOST, base.DB_PORT, base.DB_USER, base.DB_PASS, base.DB_NAME)) {
                SalaryAccess salaryAccess = new(worker);
                List<Salary> list = salaryAccess.GetSalary(month, keyword);
                response = new ApiResponseJson(1, "Success", list);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new ApiResponseJson(-1, "Failure", null);
            return Json(response);
        }
    }

}
