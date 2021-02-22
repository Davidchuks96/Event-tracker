using E_Tracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.PermissionRepository
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetAllPermissionAsync();
        Task<Permission> GetPermissionByIdAsync(string id);
        Task<(string message, bool successful)> CreatePermissionAsync(Permission permission);
        Task<(string message, bool successful)> UpdatePermissionAsync(Permission permission);
        Task<(string message, bool successful)> DeletePermissionAsync(Permission permission);
        Task<(string message, bool successful)> ManageRoleClaims(string permissionId);
        Task<IEnumerable<Permission>> GetAllNonActivePermissionAsync();
        Task<Permission> GetPermissionByNameAsync(string permissionName);
    }
}
