using E_Tracker.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Tracker.Repository.DepartmentRepo
{
    public interface IDepartmentRepository
    {
        Task<(string Message, bool Successful)> CreateDepartmentAsync(Department department);
        Task<(string Message, bool Successful)> UpdateDepartmentAsync(Department department);
        Task<(string Message, bool Successful)> DeleteDepartmentAsync(Department department);
        Task<(string Message, bool Successful)> ActivateDepartmentAsync(Department department);
        Task<IEnumerable<Department>> GetAllNotActiveDepartmentAsync();
        Task<IEnumerable<Department>> GetAllDepartmentAsync();
        Task<Department> GetDepartmentByIdAsync(string DepartmentId);
        Task<Department> GetDepartmentByNameAsync(string departmentName);

    }
}