using Microsoft.AspNetCore.Mvc;

using Dashboard.Models.Utility;

namespace Dashboard.Controllers;
public class HomeController(IConfiguration conf) : BaseController(conf) {
    // ---------------------------------------------------------------------
    // Field
    // ---------------------------------------------------------------------
    private readonly Logger _logger = Logger.GetInstance(conf);

    // ---------------------------------------------------------------------
    // [GET] index
    // ---------------------------------------------------------------------
    public IActionResult Index() {
        return View();
    }

}
