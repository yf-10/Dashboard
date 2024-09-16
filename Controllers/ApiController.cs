using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Dashboard.Models.Utility;
using Dashboard.Models.Service;
using Dashboard.Models.Data;

namespace Dashboard.Controllers;
public class ApiController(IConfiguration conf) : BaseController(conf) {
    // ---------------------------------------------------------------------
    // Field
    // ---------------------------------------------------------------------
    private readonly Logger _logger = Logger.GetInstance(conf);

    // ---------------------------------------------------------------------
    // [GET] api
    // ---------------------------------------------------------------------
    [HttpGet]
    [Route("api")]
    public IActionResult Api() {
        return View();
    }

    // ---------------------------------------------------------------------
    // [GET] api/key
    // ---------------------------------------------------------------------
    [HttpGet]
    [Route("api/key/{userId}")]
    public IActionResult ApiKey(string userId) {
        ApiResponseJson? response = null;
        try {
            using (PostgresqlWorker worker = new()) {
                User user = new(userId, "", "");
                ApiKeyAccess apiKeyAccess = new(worker);
                List<ApiKey> list = apiKeyAccess.Select(userId);
                string key = string.Empty;
                if (list.Count == 0)
                    key = apiKeyAccess.Generate(user);
                else
                    key = list[0].Key;
                response = new ApiResponseJson(0, "Success", 1, key);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new ApiResponseJson(-1, "Failure", 0, ex);
            return Json(response);
        }
    }

    // ---------------------------------------------------------------------
    // [GET] api/salaries
    // ---------------------------------------------------------------------
    [HttpGet]
    [Authorize]
    [Route("api/salaries/{month?}")]
    public IActionResult GetSalaries(string? month, string? keyword) {
        ApiResponseJson? response = null;
        try {
            using (PostgresqlWorker worker = new()) {
                SalaryAccess salaryAccess = new(worker);
                List<Salary> list = salaryAccess.Select(month, keyword);
                response = new ApiResponseJson(0, "Success", list.Count, list);
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
    [Authorize]
    [Route("api/salaries")]
    public IActionResult PostSalaries([FromBody] SalariesJson salariesJson, string? userName) {
        ApiResponseJson? response = null;
        try {
            User user = new(userName ?? "unknown", "", "");
            using (PostgresqlWorker worker = new()) {
                SalaryAccess salaryAccess = new(worker);
                int count = salaryAccess.Insert(user, salariesJson.Salaries);
                response = new ApiResponseJson(0, "Success", count, null);
            }
            return Json(response);
        } catch (Exception ex) {
            _logger.Error(ex);
            response = new ApiResponseJson(-1, "Failure", 0, ex);
            return Json(response);
        }
    }

}
