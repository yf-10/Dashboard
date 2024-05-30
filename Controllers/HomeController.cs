using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using Dashboard.Models.Utility;

namespace Dashboard.Controllers;

public class HomeController : BaseController
{
    // ---------------------------------------------------------------------
    // Field
    // ---------------------------------------------------------------------
    private readonly Logger? _logger = Logger.GetInstance();

    // ---------------------------------------------------------------------
    // Constructor
    // ---------------------------------------------------------------------
    public HomeController(IConfiguration conf) : base(conf) {}

    // ---------------------------------------------------------------------
    // [Get] Index
    // ---------------------------------------------------------------------
    public IActionResult Index() {
        _logger?.Debug("Get Index!");

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
