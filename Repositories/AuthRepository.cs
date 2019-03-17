using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using schoolRegistration.Data;
using schoolRegistration.Models;

namespace schoolRegistration.Repositories {
    public class AuthRepository : IAuthRepository {
        private readonly DataContext _context;
        public AuthRepository (DataContext context) {
            _context = context;

        }

        public async Task<User> Login (string username, string password) {

            var user = await _context.User.FirstOrDefaultAsync (x => x.UserName == username);

            if (user == null)
                return null;

            if (!verifyPasswordHash (password, user.PasswordHash, user.PasswordSalt))
                return null;

            //auth successful
            return user;
        }
        private bool verifyPasswordHash (string password, byte[] passwordHash, byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512 (passwordSalt)) {
                var ComputeHash = hmac.ComputeHash (System.Text.Encoding.UTF8.GetBytes (password));
                for (int i = 0; i < ComputeHash.Length; i++) {
                    if (ComputeHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }
        public async Task<bool> UserExist (string username) {
            if (await _context.User.AnyAsync (x => x.UserName == username))
                return true;

            return false;
        }

        public async Task<User> Register (User user, string password) {
            byte[] passwordHash, passwordSalt;

            createPasswordHash (password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;

            user.PasswordSalt = passwordSalt;

            await _context.User.AddAsync (user);

            await _context.SaveChangesAsync ();

            return user;
        }

        private void createPasswordHash (string password, out byte[] passwordHash, out byte[] passwordSalt) 
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512 ()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash (System.Text.Encoding.UTF8.GetBytes (password));
            }
        }

    }
}