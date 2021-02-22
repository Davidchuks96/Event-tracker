using E_Tracker.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.DepartmentRepo
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(string Message, bool Successful)> CreateDepartmentAsync(Department department)
        {
            //sample code ///
            if (department is null) return await Task.FromResult(("Please provide the department to be created", false));
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            return ($"{department.Name} has been created successfully", true);

        }
        public async Task<(string Message, bool Successful)> UpdateDepartmentAsync(Department department)
        {
            if (department is null) return await Task.FromResult(("Please provide the department to be updated", false));
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{department.Name} has been updated successfully", true);

        }
        public async Task<(string Message, bool Successful)> ActivateDepartmentAsync(Department department)
        {
            if (department is null) return await Task.FromResult(("Please provide the Department to be activated", false));
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{department.Name} has been activated successfully", true);
        }
        public async Task<IEnumerable<Department>> GetAllNotActiveDepartmentAsync()
        {
            var department = await _context.Departments.Where(x => x.IsActive == false).ToListAsync();
            return department;
        }

        public async Task<(string Message, bool Successful)> DeleteDepartmentAsync(Department department)
        {
            if (department is null) return await Task.FromResult(("Please provide the department to be deleted", false));
            department.IsActive = false;
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ($"{department.Name} has been Deleted successfully", true);

        }
        public async Task<IEnumerable<Department>> GetAllDepartmentAsync()
        {
            var department = await _context.Departments.Include(x => x.Users).ToListAsync();
            return department;

        }
        public async Task<Department> GetDepartmentByIdAsync(string DepartmentId)
        {
            var department = await _context.Departments.FindAsync(DepartmentId);
            return department;

        }
        public async Task<Department> GetDepartmentByNameAsync(string departmentName)
        {
            var department = await _context.Departments.Where(x => x.Name == departmentName).FirstOrDefaultAsync();
            return department;
        }

    }
}
