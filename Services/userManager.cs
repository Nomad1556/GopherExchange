using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using GopherExchange.Models;
using GopherExchange.Data;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace GopherExchange.Services
{
    public class userManager
    {

        private readonly int _iterations = 10000;

        private readonly int _saltSize = 128 / 8;
        private readonly int _numBytesRequested = 256 / 8;

        public enum Response
        {
            Success,
            Failure
        }
        private readonly ILogger _logger;

        private readonly GeDbContext _context;

        private readonly IHttpContextAccessor _accessor;


        public userManager(ILoggerFactory factory, GeDbContext context, IHttpContextAccessor accessor)
        {
            _logger = factory.CreateLogger<userManager>();
            _context = context;
            _accessor = accessor;
        }

        public async Task<Response> createAccountAsync(GenerateAccountModel cmd)
        {

            Account acc = new Account
            {
                Userid = GenerateAccountNumber(),
                Username = cmd.goucherEmail.Split("@")[0],
                Goucheremail = cmd.goucherEmail,
                Accounttype = cmd.accountType,
                Hashedpassword = HashPassword(cmd.password)
            };
            try
            {
                _logger.LogInformation("Adding...");
                _context.Add(acc);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Account made...");

            }
            catch (Exception e)
            {
                _logger.LogInformation("Unable to create account: " + e.ToString());
                return Response.Failure;
            }

            _logger.LogInformation("Account with password made successfully");
            return Response.Success;
        }
        public async Task<Dictionary<String, String>> getSessionAccount()
        {

            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var acc = await _context.Accounts.FindAsync(claim);

            if (acc == null) throw new NullReferenceException();

            Dictionary<String, String> sessionAccount = new Dictionary<String, String>{
                {"Username", acc.Username},
                {"Email", acc.Goucheremail},
                {"Accountype", _accessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value}
            };

            return sessionAccount;
        }

        public async Task<Dictionary<String, String>> getUserInformationByEmail(string email)
        {
            var acc = await _context.Accounts.FirstOrDefaultAsync(e => e.Goucheremail == email);

            if (acc == null) return null;

            Dictionary<String, String> UserInformation = new Dictionary<String, String>{
                {"Username", acc.Username},
                {"Email", acc.Goucheremail},
            };

            return UserInformation;
        }

        public async Task<bool> checkForDuplicate(string email)
        {
            var p = await _context.Accounts.FirstOrDefaultAsync(e => e.Goucheremail == email);

            return p != null;
        }

        public async Task editAccount(EditAccountModel cmd)
        {
            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var acc = await _context.Accounts.FindAsync(claim);

            if (acc == null)
            {
                throw new NullReferenceException();
            }
            acc.Username = cmd.NewUserName;
            await _context.SaveChangesAsync();
        }

        public async Task<String> getUserName()
        {
            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var acc = await _context.Accounts.FindAsync(claim);

            String userName = acc.Username;

            return userName;
        }

        public Account validateUser(BindingLoginModel cmd)
        {
            if (cmd == null) return null;

            var acc = _context.Accounts
                        .FirstOrDefault(a => a.Goucheremail == cmd.goucherEmail);

            if (acc == null) return null;

            bool verified = VerifyHashedPassword(acc.Hashedpassword, cmd.password);

            if (!verified) return null;
            return acc;
        }

        public async Task DeleteAccount()
        {
            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var acc = await _context.Accounts.FindAsync(claim);

            _context.Accounts.Remove(acc);

            await _context.SaveChangesAsync();
        }
        /* HashPassword and VerifyHashedPassword are functions written from the help of Miscrosoft's ASP.NET Core Identity
            licensed under an MIT license.
            Github: https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Extensions.Core/src/PasswordHasher.cs
        */
        private string HashPassword(string password)
        {

            byte[] salt = new byte[_saltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, _iterations, _numBytesRequested);

            String source = Convert.ToHexString(salt) + Convert.ToHexString(subkey);
            return source;
        }

        private bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] expectedPassword;

            if (hashedPassword == null) return false;
            if (password == null) return false;

            byte[] salt = Convert.FromHexString(hashedPassword.Substring(0, 32));
            if (salt.Length != _saltSize) return false;

            byte[] actualPassword = Convert.FromHexString(hashedPassword.Substring(32, hashedPassword.Length - 32));

            if (actualPassword.Length != _numBytesRequested) return false;

            expectedPassword = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10000, _numBytesRequested);

            return CryptographicOperations.FixedTimeEquals(actualPassword, expectedPassword);
        }

        private int GenerateAccountNumber()
        {
            byte[] accNumber = new byte[0xa];

            using (var rngCsp = RandomNumberGenerator.Create())
            {

                rngCsp.GetNonZeroBytes(accNumber);
            }

            int x = BitConverter.ToInt32(accNumber);
            if (x < 0) return -x;
            return x;
        }
    }
}