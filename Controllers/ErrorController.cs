using Microsoft.AspNetCore.Mvc;

using Dashboard.Models.Utility;
using Dashboard.Models;

namespace Dashboard.Controllers;
public class ErrorController(IConfiguration conf) : BaseController(conf)
{
    // ---------------------------------------------------------------------
    // Field
    // ---------------------------------------------------------------------
    private readonly Logger _logger = Logger.GetInstance(conf);

    // ---------------------------------------------------------------------
    // [GET] error
    // ---------------------------------------------------------------------
    [HttpGet]
    [Route("error/{code?}")]
    public IActionResult Error(string? code) {
        _logger.Error("ERROR CODE:" + code ?? "");
        return View(code);
    }

}
