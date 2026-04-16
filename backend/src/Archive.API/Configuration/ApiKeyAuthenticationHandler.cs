using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Shiron.TheArchive.API.Services;

namespace Shiron.TheArchive.API.Configuration;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions {
    public const string SchemeName = "ApiKey";
}

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IApiKeyService apiKeyService
)
    : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder) {
    protected async override Task<AuthenticateResult> HandleAuthenticateAsync() {
        var apiKeyValue = ExtractApiKey();
        if (string.IsNullOrWhiteSpace(apiKeyValue))
            return AuthenticateResult.NoResult();

        var apiKey = await apiKeyService.ValidateAsync(apiKeyValue);
        if (apiKey == null)
            return AuthenticateResult.Fail("Invalid API key");

        var principal = await apiKeyService.BuildClaimsPrincipalAsync(apiKey);
        if (principal.Identity is not { IsAuthenticated: true })
            return AuthenticateResult.Fail("Failed to build claims for API key");

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    internal static bool RequestHasApiKey(HttpContext context) {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader is not null && authHeader.StartsWith("ApiKey ", StringComparison.OrdinalIgnoreCase))
            return true;
        return context.Request.Query.ContainsKey("apikey");
    }

    private string? ExtractApiKey() {
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (authHeader is not null && authHeader.StartsWith("ApiKey ", StringComparison.OrdinalIgnoreCase))
            return authHeader["ApiKey ".Length..].Trim();

        if (Request.Query.TryGetValue("apikey", out var queryValue))
            return queryValue.FirstOrDefault();

        return null;
    }
}
