using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using System.Security.Claims;
using System.Text.Encodings.Web;

using Dashboard.Models.Data;
using Dashboard.Models.Service;
using Dashboard.Models.Utility;

namespace Dashboard.Controllers;
public class AuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder) {

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {

        // Get and validate API key
        static (bool isAuthorized, string userId) validateApiKey(HttpContext context) {
            // Get User ID from HTTP header
            if (!context.Request.Headers.TryGetValue("x-user-id", out var xUserId))
                return (false, "");
            // Get API key from HTTP header
            if (!context.Request.Headers.TryGetValue("x-api-key", out var xApiKey))
                return (false, xUserId.ToString());
            // Validation
            using PostgresqlWorker worker = new();
            ApiKeyAccess apiKeyAccess = new(worker);
            List<ApiKey> apiKeys = apiKeyAccess.Select(xUserId.ToString());
            if (apiKeys.Count == 0)
                return (false, xUserId.ToString());
            else if (apiKeys[0].Key.Equals(xApiKey.ToString()) == false)
                return (false, xUserId.ToString());
            else
                return (true, xUserId.ToString());
        }

        // Authorize
        var (isAuthorized, name) = validateApiKey(Context);

        // Authorization failed
        if (!isAuthorized) {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key, Authorization failed"));
        }

        // Authorization success
        var p = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, name)], "AuthType"));
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(p, "Api")));
    }

}
