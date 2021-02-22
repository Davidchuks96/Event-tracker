using E_Tracker.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Authorization
{
    public class RolesList
    {
        public List<Role> RolesLList { get { return GetRoles(); } private set { } }
        private readonly RoleManager<Role> roleManager;

        public RolesList(RoleManager<Role> roleManager)
        {
            this.roleManager = roleManager;
            
        }

        public List<Role> GetRoles()
        {
            List<Role> roles = new List<Role>();
            roles = roleManager.Roles.ToList();
            return roles;
        }

        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string User = "User";
    }

}
