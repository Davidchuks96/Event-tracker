using E_Tracker.Data;
using E_Tracker.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_Tracker.Repository.PermissionRepository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public PermissionRepository(RoleManager<Role> roleManager, UserManager<User> userManager,
            SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        public async Task<(string message, bool successful)> CreatePermissionAsync(Permission permission)
        {
            if (permission is null) return await Task.FromResult(("Please provide the permission to be created", false));

            var permissions = await GetPermissionByNameAsync(permission.Name);
            if (permissions == null)
            {
                permission.IsActive = true;
                await _context.Permissions.AddAsync(permission);
                await _context.SaveChangesAsync();
                return ($"{permission.Name} has been created successfully", true);
            }
            return ($"{permission.Name} was not created please try again", false);
        }

        public async Task<(string message, bool successful)> DeletePermissionAsync(Permission permission)
        {
            if (permission is null) return await Task.FromResult(("Please provide the permission to be deleted", false));
            permission.IsActive = false;
            _context.Entry(permission).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{permission.Name} has been deleted successfully", true);
        }

        public async Task<IEnumerable<Permission>> GetAllNonActivePermissionAsync()
        {
            return await _context.Permissions.Where(x => x.IsActive == false).ToListAsync();
        }

        public async Task<IEnumerable<Permission>> GetAllPermissionAsync()
        {
            return await _context.Permissions.Where(x => x.IsActive == true).ToListAsync();
        }

        public async Task<Permission> GetPermissionByIdAsync(string id)
        {
           return await _context.Permissions.FindAsync(id);
        }

        public async Task<Permission> GetPermissionByNameAsync(string permissionName)
        {
            return await _context.Permissions.Where(x => x.Name == permissionName).FirstOrDefaultAsync();
        }

        public async Task<(string message, bool successful)> ManageRoleClaims(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return ("This role does not exist", false);
            }
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            var model = new RoleClaimsDto
            {
                RoleId = roleId,
            };

            //foreach(Claim claim in   )
            return ("This role does not exist", true);
        }

        public async Task<(string message, bool successful)> UpdatePermissionAsync(Permission permission)
        {
            if (permission is null) return await Task.FromResult(("Please provide the permission to be updated", false));
            _context.Entry(permission).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{permission.Name} has been updated successfully", true);
        }
    }
}
