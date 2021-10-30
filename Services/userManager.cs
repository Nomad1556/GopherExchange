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
using System.Security.Claims;


namespace GopherExchange.Services
{
    public class userManager
    {

        private readonly int _iterations = 10000;

        public enum ResponseType
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

        public async Task<ResponseType> createAccountAsync(GenerateAccountModel cmd)
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
                return ResponseType.Failure;
            }

            _logger.LogInformation("Account with password made successfully");
            return ResponseType.Success;
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
        private byte[] HashPassword(string password)
        {
            int saltSize = 128 / 8;
            int numBytesRequested = 256 / 8;

            byte[] salt = new byte[saltSize];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, _iterations, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)KeyDerivationPrf.HMACSHA256);
            WriteNetworkByteOrder(outputBytes, 5, (uint)_iterations);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return outputBytes;
        }

        private bool VerifyHashedPassword(byte[] hashedPassword, string password)
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
                    _logger.LogInformation("Wrong salt size");
                    return false;
                }
                byte[] salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

                // Read the subkey (the rest of the payload): must be >= 128 bits
                int subkeyLength = hashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    _logger.LogInformation("Wrong subkey size");
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
                _logger.LogInformation("Error happened");
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }
        private bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null || a.Length != b.Length) return false;

            bool areSame = true;
            for (int i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }
        private void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

        private int GenerateAccountNumber()
        {
            byte[] accNumber = new byte[0xa];

            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {

                rngCsp.GetBytes(accNumber);
            }

            int x = BitConverter.ToInt32(accNumber);
            if (x < 0) return -x;
            return x;
        }
    }
}