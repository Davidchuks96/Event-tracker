using E_Tracker.Data;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Tracker.Repository.UserRepository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUserAsync();
        Task<IEnumerable<User>> GetAllDeactivatedUsersAsync();
        Task<User> GetUserByIdAsync(string id);
        Task<(string message, IdentityResult result, bool successful)> CreateUserAsync(User user, string password);
        Task<(string message, IdentityResult result, bool successful)> UpdateUserAsync(User user);
        Task<(string message, bool successful)>DeleteUserAsync(User user);
    }
}
