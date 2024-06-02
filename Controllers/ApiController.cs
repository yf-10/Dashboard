using Microsoft.AspNetCore.Mvc;

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
    // [GET] api/salaries
    // ---------------------------------------------------------------------
    [HttpGet]
    [Route("api/salaries/{month?}")]
    public IActionResult GetSalaries(string? month, string? keyword) {
        ApiResponseJson? response = null;
        try {
            using (PostgresqlWorker worker = new(base.DB_HOST, base.DB_PORT, base.DB_USER, base.DB_PASS, base.DB_NAME)) {
                SalaryAccess salaryAccess = new(worker);
                List<Salary> list = salaryAccess.Select(month, keyword);
                response = new ApiResponseJson(1, "Success", list.Count, list);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new ApiResponseJson(-1, "Failure", 0, ex);
            return Json(response);
        }
    }

    // ---------------------------------------------------------------------
    // [POST] api/salaries
    // ---------------------------------------------------------------------
    [HttpPost]
    [Route("api/salaries")]
    public IActionResult PostSalaries([FromBody] SalariesJson salariesJson, string? userName) {
        ApiResponseJson? response = null;
        try {
            User user = new(userName ?? "unknown", "", "");
            using (PostgresqlWorker worker = new(base.DB_HOST, base.DB_PORT, base.DB_USER, base.DB_PASS, base.DB_NAME)) {
                SalaryAccess salaryAccess = new(worker);
                int count = salaryAccess.Insert(user, salariesJson.Salaries);
                response = new ApiResponseJson(1, "Success", count, null);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new ApiResponseJson(-1, "Failure", 0, ex);
            return Json(response);
        }
    }

}
