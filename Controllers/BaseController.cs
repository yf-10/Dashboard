using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Dashboard.Models.Utility;

namespace Dashboard.Controllers;

public class BaseController : Controller
{
    // ---------------------------------------------------------------------
    // Field
    // ---------------------------------------------------------------------
    private readonly IConfiguration _conf;
    private readonly Logger _logger;

    // ---------------------------------------------------------------------
    // Constructor
    // ---------------------------------------------------------------------
    public BaseController(IConfiguration conf) {
        _conf = conf;
        _logger = Logger.GetInstance(conf);
        // Load configurations
        
    }

    // ---------------------------------------------------------------------
    // Filter : Before action
    // ---------------------------------------------------------------------
    public override void OnActionExecuting(ActionExecutingContext filterContext) {
        RouteValueDictionary routeValue = filterContext.RouteData.Values;

        // LoginController
        if (routeValue["controller"]?.ToString() == "Login") {
            return;
        } else {

        }

        base.OnActionExecuting(filterContext);
    }

    // ---------------------------------------------------------------------
    // Filter : After action
    // ---------------------------------------------------------------------
    public override void OnActionExecuted(ActionExecutedContext filterContext) {
        RouteValueDictionary routeValue = filterContext.RouteData.Values;


        base.OnActionExecuted(filterContext);
    }


}
