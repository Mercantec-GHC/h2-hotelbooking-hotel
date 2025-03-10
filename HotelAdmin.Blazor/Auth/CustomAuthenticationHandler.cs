using HotelsRazorLibrary.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace HotelAdmin.Blazor.Auth
{
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] {
                new Claim(ClaimTypes.Role, "ConcernAdmin"),
                new Claim(ClaimTypes.Role, "GlobalAdmin"),
                new Claim(ClaimTypes.Role, "HotelAdmin"),
                new Claim(ClaimTypes.Role, "HotelWorker")
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        //private readonly CustomAuthenticationStateProvider _authenticationStateProvider;

        //public CustomAuthenticationHandler(
        //    IOptionsMonitor<AuthenticationSchemeOptions> options,
        //    ILoggerFactory logger,
        //    UrlEncoder encoder,
        //    ISystemClock clock,
        //    CustomAuthenticationStateProvider authenticationStateProvider)
        //    : base(options, logger, encoder, clock)
        //{
        //    _authenticationStateProvider = authenticationStateProvider;
        //}

        //protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        //{
        //    var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        //    var user = authenticationState.User;

        //    if (user.Identity.IsAuthenticated)
        //    {
        //        var ticket = new AuthenticationTicket(user, Scheme.Name);
        //        return AuthenticateResult.Success(ticket);
        //    }

        //    return AuthenticateResult.Fail("User is not authenticated");
        //}
    }
}
