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
                Hashedpassword = Convert.ToBase64String(HashPassword(cmd.password))
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

            bool verified = VerifyHashedPassword(Convert.FromBase64String(acc.Hashedpassword), cmd.password);

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
        private byte[] HashPassword(string password)
        {

            byte[] salt = new byte[_saltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, _iterations, _numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)KeyDerivationPrf.HMACSHA256);
            WriteNetworkByteOrder(outputBytes, 5, (uint)_iterations);
            WriteNetworkByteOrder(outputBytes, 9, (uint)_saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + _saltSize, subkey.Length);
            return outputBytes;
        }

        private static bool VerifyHashedPassword(byte[] hashedPassword, string password)
        {
            try
            {
                // Read header information
                KeyDerivationPrf prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
                int iterCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
                int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

                // Read the salt: must be >= 128 bits
                if (saltLength < 128 / 8)
                {
                    return false;
                }
                byte[] salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

                // Read the subkey (the rest of the payload): must be >= 128 bits
                int subkeyLength = hashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    return false;
                }
                byte[] expectedSubkey = new byte[subkeyLength];
                Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                // Hash the incoming password and verify it
                byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);

                return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
            }
            catch
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }
        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }
        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
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