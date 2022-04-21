using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using GopherExchange.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace GopherExchange.Services
{
    public class loginManager
    {

        private readonly ILogger _logger;
        private enum Role
        {
            User,
            Admin
        }

        private readonly IHttpContextAccessor _accessor;

        public loginManager(ILoggerFactory factory, IHttpContextAccessor accessor)
        {
            _logger = factory.CreateLogger<userManager>();
            _accessor = accessor;
        }
        public async Task signIn(Account account, bool isPersistent = false)
        {

            _logger.LogInformation("Making identity...");
            ClaimsIdentity identity = new ClaimsIdentity(getAccountClaims(account), CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Making Principal...");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            _logger.LogInformation("Trying to sign in...");
            await _accessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            _logger.LogInformation("Signed in!");

        }

        public async Task signOut()
        {
            HttpContext _httpcontext = _accessor.HttpContext;
            _logger.LogInformation("Trying to sign out...");
            await _accessor.HttpContext.SignOutAsync();
            _logger.LogInformation("Signed out!");
        }

        public Boolean IsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        private IEnumerable<Claim> getAccountClaims(Account account)
        {

            List<Claim> claims = new List<Claim>();
            _logger.LogInformation("Making claims...");
            claims.Add(new Claim(ClaimTypes.NameIdentifier, account.Userid.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, account.Username));
            claims.Add(new Claim(ClaimTypes.Email, account.Goucheremail));

            var permission = account.Accounttype == 1 ? Role.User : Role.Admin;

            claims.Add(new Claim(ClaimTypes.Role, permission.ToString()));
            return claims;
        }
    }
}