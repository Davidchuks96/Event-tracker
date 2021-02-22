using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Tracker.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Tracker.Repository.RoleRepository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public RoleRepository(RoleManager<Role> roleManager, UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<(string message, bool successful)> CreateRoleAsync(Role role)
        {
            if (role is null) return await Task.FromResult(("Please provide role to be created", false));
            var roles = await _roleManager.FindByNameAsync(role.Name);

            if (roles != null)
            {
                return ("This role already exists", false);
            }

            role.IsActive = true;
            await _roleManager.CreateAsync(role);
            return ($"{role.Name} has been created successfully", true);
        }

        public async Task<(string message, bool successful)> DeleteRoleAsync(Role role)
        {
            if (role is null) return await Task.FromResult(("Please provide role to be deleted", false));

            role.IsActive = false;
            //_roleManager.r
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return ($"{role.Name} has been deleted successfully", true);
            }
            return ($"An error occured while updating {role.Name}", false);
        }

        public async Task<IEnumerable<Role>> GetAllRoleAsync()
        {
            return await _roleManager.Roles.Where(r => r.IsActive == true).ToListAsync();
        }
        
        public async Task<IEnumerable<Role>> GetAllDeactivatedRoleAsync()
        {
            return await _roleManager.Roles.Where(r => r.IsActive == false).ToListAsync();
        }

        public Task<Role> GetRoleByIdAsync(string id)
        {
            return _roleManager.FindByIdAsync(id);
        }

        public async Task<(string message, bool successful)> UpdateRoleAsync(Role role)
        {
            if (role is null) return await Task.FromResult(("Please provide role to be updated", false));

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return ($"{role.Name} has been updated successfully", true);
            }
            return ($"An error occured while updating {role.Name}", false);
        }
    }
}
