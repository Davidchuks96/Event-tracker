using E_Tracker.Data;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Tracker.Repository.RoleRepository
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllRoleAsync();
        Task<IEnumerable<Role>> GetAllDeactivatedRoleAsync();
        Task<Role> GetRoleByIdAsync(string id);
        Task<(string message, bool successful)> CreateRoleAsync(Role role);
        Task<(string message, bool successful)> UpdateRoleAsync(Role role);
        Task<(string message, bool successful)> DeleteRoleAsync(Role role);
    }
}
