using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Dashboard.Models.Utility;

namespace Dashboard.Controllers;

public class BaseController(IConfiguration conf) : Controller
{
    // ---------------------------------------------------------------------
    // Field
    // ---------------------------------------------------------------------
    private readonly Logger _logger = Logger.GetInstance(conf);

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
        _logger.Info("TEST");
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
