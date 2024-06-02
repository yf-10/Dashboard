using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Dashboard.Models.Utility;
using Dashboard.Models.Item;

namespace Dashboard.Controllers;
public class BaseController(IConfiguration conf) : Controller
{
    // ---------------------------------------------------------------------
    // Field
    // ---------------------------------------------------------------------
    private readonly Logger _logger = Logger.GetInstance(conf);

    // ---------------------------------------------------------------------
    // Field : Configuration
    // ---------------------------------------------------------------------
    public readonly User ADMIN = new(
        conf.GetValue<string>("Administrator:name", "unknown") ?? "unknown",
        conf.GetValue<string>("Administrator:fullName", "unknown") ?? "unknown",
        conf.GetValue<string>("Administrator:email", "unknown") ?? "unknown"
    );
    
    public readonly string  DB_HOST = conf.GetValue<string>("DatabaseConfig:host", "localhost") ?? "localhost";
    public readonly int     DB_PORT = conf.GetValue<int>("DatabaseConfig:port", 5432);
    public readonly string  DB_USER = conf.GetValue<string>("DatabaseConfig:user", "postgres") ?? "postgres";
    public readonly string  DB_PASS = conf.GetValue<string>("DatabaseConfig:pass", "") ?? "";
    public readonly string  DB_NAME = conf.GetValue<string>("DatabaseConfig:name", "postgres") ?? "postgres";

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
