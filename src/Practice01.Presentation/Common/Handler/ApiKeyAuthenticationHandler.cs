using System.Diagnostics;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Practice01.Application.Common.Validation;
using Practice01.Presentation.Common.ObjectResult;

namespace Practice01.Presentation.Common.Handler;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;

    // [Obsolete("Obsolete")]
    // public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    //     ILoggerFactory logger,
    //     UrlEncoder encoder,
    //     ISystemClock clock) : base(options,
    //     logger,
    //     encoder,
    //     clock)
    // {
    // }

    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration) : base(options,
        logger,
        encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing API Key"));
        }

        var validApiKeys = _configuration.GetSection("ApiKeys").GetChildren().Select(x => x.Value).ToList();

        if (!validApiKeys.Contains(extractedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
        }

        var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/json";
        return Response.WriteAsJsonAsync(new ApiResponse()
        {
            Ok = false,
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = "API Key is missing or invalid",
            Code = "API_KEY_IS_MISSING_OR_INVALID",
            Metadata = new Metadata
            {
                RequestId = Context.TraceIdentifier,
                TraceId = Activity.Current?.Id ?? string.Empty
            },
            Details =
            [
                new CollectedError
                {
                    Field = "API Key",
                    Code = "API_KEY_IS_MISSING_OR_INVALID",
                    Message = "API Key is missing or invalid. Please provide a valid API Key."
                }
            ]
        });
    }
}