using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using GopherExchange.Models;
using GopherExchange.Data;
using System.Linq;
using Microsoft.Extensions.Logging;


namespace GopherExchange.Services
{
    public class userManager{

        
        public enum ResponseType
        {
            Success,
            Failure
        }
        readonly ILogger _logger;

        readonly GeDbConext _context;


        public userManager(ILoggerFactory factory, GeDbConext context){
            _logger = factory.CreateLogger<userManager>();
            _context = context;
        }

        public async Task <ResponseType> createAccountAsync(GenerateAccountModel cmd){

            Account acc = new Account{
                Userid = GenerateAccountNumber(),
                Username = cmd.goucherEmail.Split("@")[0],
                Goucheremail = cmd.goucherEmail,
                Accounttype = cmd.accountType,
                HashedPassword = HashPassword(cmd.password)
            };
                try{
                    _logger.LogInformation("Adding...");
                    _context.Add(acc);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Account made...");
                    
                }catch(Exception e){
                    _logger.LogInformation("Unable to create account: " + e.ToString());
                    return ResponseType.Failure;
                }

            _logger.LogInformation("Account with password made successfully");
            return ResponseType.Success;
        }

        public Account validateUser(BindingLoginModel cmd){
            if(cmd == null) return null;

            var acc = _context.Accounts
                        .FirstOrDefault(a => a.Goucheremail == cmd.goucherEmail);

            if (acc == null) return null;

            bool verified = VerifyHashedPassword(acc.HashedPassword, cmd.password);
            
            if(!verified) return null;
            return acc;
        }
        private string HashPassword(string password) {
            byte[] salt;
            byte[] buffer2;
            if(password == null) return "No password";

            // Salt and hash the password. Store them for writing as a string.
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10,0x3e8)) {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }

            // Store the salt and the hash, one after another, into a base64-converted string.
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt,0,dst,1,0x10);
            Buffer.BlockCopy(buffer2,0,dst,0x11,0x20);
            return Convert.ToBase64String(dst);
        }

        private int GenerateAccountNumber(){
            byte[] accNumber = new byte[0xa];

            using(RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider()){

                rngCsp.GetBytes(accNumber);
            }

            int x = BitConverter.ToInt32(accNumber);
            if (x < 0) return -x;
            return x;
        }

        private bool VerifyHashedPassword(string HashedPassword, string password) {
            byte[] buffer4;

            if (HashedPassword == null) return false;
            if (password == null) return false;

            // Verify that the hashed password is in the correct format.
            byte[] src = Convert.FromBase64String(HashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0)) return false;

            // Split the salt and the hash.
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src,1,dst,0,0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src,0x11,buffer3,0,0x20);

            // Run them through the algorithm to get the hashed password.
            using(Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password,dst,0x3e8)){
                buffer4 = bytes.GetBytes(0x20);
            }

            // Verify the hashes are the same.
            return ByteArraysEqual(buffer3,buffer4);
        }
        private bool ByteArraysEqual(byte[] a, byte[] b) {
            if(a == null && b == null) return true;
            if(a == null || b ==  null || a.Length != b.Length) return false;
            
            bool areSame = true;
            for(int i = 0; i < a.Length; i++){
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
    }
}