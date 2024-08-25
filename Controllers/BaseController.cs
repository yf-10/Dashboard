using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

using Dashboard.Models.Utility;
using Dashboard.Models.Data;
using Microsoft.AspNetCore.Http.Extensions;

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
        conf.GetValue<string>("Administrator:id", "unknown") ?? "unknown",
        conf.GetValue<string>("Administrator:name", "unknown") ?? "unknown",
        conf.GetValue<string>("Administrator:email", "unknown") ?? "unknown"
    );

    // ---------------------------------------------------------------------
    // Filter : OnActionExecuting
    // ---------------------------------------------------------------------
    public override void OnActionExecuting(ActionExecutingContext filterContext) {
        RouteValueDictionary routeValues = filterContext.RouteData.Values;
        string requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        string requestUrl = Request.GetDisplayUrl();
        string controller = routeValues["controller"]?.ToString() ?? "";
        string action = routeValues["action"]?.ToString() ?? "";
        string ipv4 = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "";
        string ipv6 = HttpContext.Connection.RemoteIpAddress?.MapToIPv6().ToString() ?? "";
        User loginUser = new(
                HttpContext.Session.GetString("user_id") ?? "",
                HttpContext.Session.GetString("user_name") ?? "",
                HttpContext.Session.GetString("user_email") ?? ""
        );
        _logger.Info("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
        _logger.Info("RequestID  : " + requestId);
        _logger.Info("RequestURL : " + requestUrl);
        _logger.Info("Controller : " + controller);
        _logger.Info("Action     : " + action);
        _logger.Info("Addr(ipv4) : " + ipv4);
        _logger.Info("Addr(ipv6) : " + ipv6);
        _logger.Info("UserID     : " + loginUser.Id);
        _logger.Info("UserName   : " + loginUser.Name);
        _logger.Info("UserEmail  : " + loginUser.Email);
        base.OnActionExecuting(filterContext);
    }

    // ---------------------------------------------------------------------
    // Filter : OnActionExecuted
    // ---------------------------------------------------------------------
    public override void OnActionExecuted(ActionExecutedContext filterContext) {
        string requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.Info("DoneAction : " + requestId);
        base.OnActionExecuted(filterContext);
    }


}
