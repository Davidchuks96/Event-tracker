using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using E_Tracker.Controllers;
using E_Tracker.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_Tracker.Repository.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;

        public UserRepository(UserManager<User> userManager, ILogger<UserController> logger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task<(string message, IdentityResult result, bool successful)> CreateUserAsync(User user, string password)
        {
            if (user is null) return await Task.FromResult(("Please provide the record to be created", new IdentityResult(), false));

            var users = await _userManager.FindByNameAsync(user.Email);

            if (users != null)
            {
                return ("This user already exists", new IdentityResult(), false);
            }

            user.UserName = user.Email;
            user.IsActive = true;
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return ($"{user.Email} has been created successfully", result, true);
            }
            return ($"{user.Email} not created", result, false);
        }

        public async Task<(string message, bool successful)> DeleteUserAsync(User user)
        {
            if (user is null) return await Task.FromResult(("Please provide user to be deleted", false));

            user.IsActive = false;
            user.DateDeleted = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return ($"{user.UserName} has been deleted successfully", true);
            }
            return ($"An error occured while deleting user {user.UserName}", false);
        }

        public async Task<IEnumerable<User>> GetAllUserAsync()
        {
            return await _userManager.Users.Where(u => u.IsActive == true).ToListAsync();
        }
        
        public async Task<IEnumerable<User>> GetAllDeactivatedUsersAsync()
        {
            return await _userManager.Users.Where(u => u.IsActive == false).ToListAsync();
        }

        public Task<User> GetUserByIdAsync(string id)
        {
            return _userManager.FindByIdAsync(id);
        }

        public async Task<(string message, IdentityResult result, bool successful)> UpdateUserAsync(User user)
        {
            if (user is null) return await Task.FromResult(("Please provide user to be updated", new IdentityResult(), false));

            user.UserName = user.Email;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return ($"{user.UserName} has been updated successfully", result, true);
            }
            return ($"An error occured while updating {user.UserName}", result, false);
        }       
    }
}
