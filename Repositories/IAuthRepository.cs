using System.Threading.Tasks;
using schoolRegistration.Models;

namespace schoolRegistration.Repositories {
    public interface IAuthRepository {
        Task<User> Login (string username, string password);
        Task<bool> UserExist (string username);
        Task<User> Register (User user, string password);
    }
}