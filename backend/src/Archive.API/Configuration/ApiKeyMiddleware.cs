using Microsoft.AspNetCore.Authentication;

namespace Shiron.TheArchive.API.Configuration;

public class ApiKeyMiddleware(RequestDelegate next) {
    public async Task InvokeAsync(HttpContext context) {
        if (ApiKeyAuthenticationHandler.RequestHasApiKey(context)) {
            var result = await context.AuthenticateAsync(ApiKeyAuthenticationOptions.SchemeName);
            if (result is { Succeeded: true, Principal: not null })
                context.User = result.Principal;
        }

        await next(context);
    }
}
