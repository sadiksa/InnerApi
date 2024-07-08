using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace InnerApi;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.Fail("Missing Authorization Header");

        string username = null;
        string password = null;
        try
        {
            var headerText = Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization];
            if (string.IsNullOrEmpty(headerText))
                return AuthenticateResult.Fail("Invalid Authorization Header");
            var authHeader =
                AuthenticationHeaderValue.Parse(headerText!);
            if (authHeader.Scheme != "Basic")
                return AuthenticateResult.Fail("Invalid Authorization Header");
            if (string.IsNullOrEmpty(authHeader.Parameter))
                return AuthenticateResult.Fail("Invalid Authorization Header");
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
            username = credentials[0];
            password = credentials[1];
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        if (username != "temp_user" || password != "temp_password")
            return AuthenticateResult.Fail("Invalid Username or Password");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Name, username),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}