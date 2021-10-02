using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using GopherExchange.Models;
using GopherExchange.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace GopherExchange.Services
{
    public class loginManager{
        
        readonly ILogger _logger;
        private enum Role
        {
            User,
            Admin
        }

        public loginManager(ILoggerFactory factory){
            _logger = factory.CreateLogger<userManager>();

        }
        public async Task signIn(HttpContext httpContext, Account account, bool isPersistent = false){
            _logger.LogInformation("Making identity...");
            ClaimsIdentity identity = new ClaimsIdentity(getAccountClaims(account), CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Making Principal...");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            _logger.LogInformation("Trying to sign in...");
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            _logger.LogInformation("Signed in!");

        }

        public async Task signOut(HttpContext httpContext){
            _logger.LogInformation("Trying to sign out...");
            await httpContext.SignOutAsync();
            _logger.LogInformation("Signed out!");
        }

        private IEnumerable<Claim> getAccountClaims(Account account){

            List<Claim> claims = new List<Claim>();
            _logger.LogInformation("Making claims...");
            claims.Add(new Claim(ClaimTypes.NameIdentifier, account.Userid.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, account.Username));
            claims.Add(new Claim(ClaimTypes.Email, account.Goucheremail));
            claims.AddRange(getAccountRoles(account));

            return claims;
        }

        private IEnumerable<Claim> getAccountRoles(Account account){
            _logger.LogInformation("Making roles...");
            List<Claim> claims = new List<Claim>();

            var permission = account.Accounttype == 1 ? Role.User: Role.Admin;

            claims.Add(new Claim(ClaimTypes.Role, permission.ToString()));

            return claims;
        }
    }
}