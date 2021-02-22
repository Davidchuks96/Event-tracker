using E_Tracker.Authorization;
using E_Tracker.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Tracker
{
    public static class CreateAdmin
    {
        public static void SeedData(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            SeedRole(roleManager);
            SeedAdmin(userManager);
            
        }

        public static void SeedAdmin(UserManager<User> userManager)
        {
            string email = "admin@gmail.com";
            string password = "Secret@123";
            string role = RolesList.SuperAdmin;
            if (userManager.FindByNameAsync(email).Result == null)
            {
                User user = new User();
                user.Id = Guid.NewGuid().ToString();
                user.UserName = email;
                user.Email = email;
                user.EmailConfirmed = true;
                user.IsActive = true;
                IdentityResult result = userManager.CreateAsync(user, password).Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, role).Wait();
                    var newUser = userManager.FindByIdAsync(user.Id).Result;
                    var claims = userManager.GetClaimsAsync(newUser).Result;
                }
            }
        }

        public static void SeedRole(RoleManager<Role> roleManager)
        {
            List<string> roles = new List<string> { RolesList.SuperAdmin, RolesList.Admin, RolesList.User};
            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    Role seededRole = new Role();
                    seededRole.Name = role;
                    seededRole.Id = Guid.NewGuid().ToString();
                    seededRole.IsActive = true;
                    IdentityResult roleResult = roleManager.CreateAsync(seededRole).Result;
                    if (roleResult.Succeeded)
                    {
                        SeedPermissions(roleManager, seededRole.Name);
                    }
                }
            }
        }

        public static void SeedPermissions(RoleManager<Role> roleManager, string roleName)
        {
            var role = roleManager.FindByNameAsync(roleName).Result;
            if (roleName == RolesList.SuperAdmin)
            {
                var claimsList = ClaimsStore.AllClaims;
                foreach (var claim in claimsList)
                {
                    IdentityResult result = roleManager.AddClaimAsync(role, claim).Result;
                }
            }
            else if (roleName == RolesList.Admin)
            {
                var claimsList = ClaimsStore.AdminClaims;
                foreach (var claim in claimsList)
                {
                    IdentityResult result = roleManager.AddClaimAsync(role, claim).Result;
                }
            }
            else if (roleName == RolesList.User)
            {
                var claimsList = ClaimsStore.UserClaims;
                foreach (var claim in claimsList)
                {
                    IdentityResult result = roleManager.AddClaimAsync(role, claim).Result;
                }
            }
        }
    }
}
