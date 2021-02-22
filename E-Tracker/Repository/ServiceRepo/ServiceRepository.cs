using E_Tracker.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.ServiceRepo
{
    public class ServiceRepository:IServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<(string Message, bool Successful)> CreateServiceAsync(Service service)
        {
            if (service is null) return await Task.FromResult(("Please provide a Service to be created", false));
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
            return ("service has been created successfully", true);
        }
        public async Task<(string Message, bool Successful)> UpdateServicesAsync(Service service)
        {
            if (service is null) return await Task.FromResult(("Please provide a service to be updated", false));
            _context.Entry(service).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ("service has been updated successfully", true);

        }
        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            var services = await _context.Services.Where(x => x.IsActive).Include(x => x.Item).ThenInclude(x => x.ItemGroup).Include(x => x.ServiceDepartment).ToListAsync();
            return services;
        }
        public async Task<IEnumerable<Service>> GetAllServicesByServiceDepartmentAsync(string departmentId)
        {
            var services = await _context.Services.Where(x => x.ServiceDepartmentId == departmentId && x.IsActive).Include(x => x.Item).ThenInclude(x => x.ItemGroup).Include(x => x.ServiceDepartment).ToListAsync();
            return services;
        }
        public async Task<Service>GetServiceByIdAsync(string serviceId)
        {
            var service = await _context.Services.Where(x=>x.Id==serviceId)
                                                .Include(x => x.ServiceDepartment)
                                                .Include(x => x.Item)
                                                    .ThenInclude(x => x.ItemGroup)
                                                 .FirstOrDefaultAsync();
            return service;
        }

        public async Task<IEnumerable<Service>> GetAllServicesApprovedAsync() 
        {
            var services = await _context.Services.Where(x => x.IsServiceApproved == true && x.IsActive == true ).Include(x => x.Item).ThenInclude(x => x.ItemGroup).Include(x => x.ServiceDepartment).ToListAsync();
            return services;
        }
        public async Task<IEnumerable<Service>> GetApprovedServicesByServiceDepartmentAsync(string departmentId)
        {
            var services = await _context.Services.Where(x => x.ServiceDepartmentId == departmentId && x.IsServiceApproved == true && x.IsActive == true).Include(x => x.Item).ThenInclude(x => x.ItemGroup).Include(x => x.ServiceDepartment).ToListAsync();
            services.OrderBy(x => x.Id).ThenBy(x => x.ServiceDepartment.Name);
            return services;
        }
        public async Task<IEnumerable<Service>> GetAllApprovedServicesByDateAsync(DateTime? startDate, DateTime? endDate)
        {
            var services = await _context.Services.Where(x => x.NewExpiryDate >= startDate && x.NewExpiryDate <= endDate && x.IsServiceApproved == true).ToListAsync();
            services.OrderByDescending(x => x.NewExpiryDate);
            return services;
        }

        public async Task<IEnumerable<Service>> GetAllApprovedServicesByDateItemTypeItemDepartmentAndCategoryAsync(DateTime startDate, DateTime endDate, string itemTypeId, string categoryId, string itemDepartmentId, string userDepartmentId = null)
        {
            IQueryable<Service> services = _context.Services.Where(x => x.DateServiced.Date >= startDate.Date && x.DateServiced.Date <= endDate.Date
                                                       && x.IsServiceApproved == true && x.IsActive == true).Include(x => x.Item).ThenInclude(x => x.ItemGroup);
            if (userDepartmentId != null)
            {
                services = services.Where(x => x.ServiceDepartmentId == userDepartmentId);
            }
            //Check if it is not no selection and it is not ALL Item Types selection
            if (itemTypeId != null && itemTypeId != "0")
            {
                services = services.Where(x => x.Item.ItemTypeId == itemTypeId);
            }
            //Check if it is not no selection and it is not ALL Categories selection
            if (categoryId != null && categoryId != "0")
            {
                services = services.Where(x => x.Item.ItemGroup.CategoryId == categoryId);
            }
            //Check if it is not no selection and it is not ALL Item Departments selection
            if (itemDepartmentId != null && itemDepartmentId != "0")
            {
                services = services.Where(x => x.Item.ItemGroup.DepartmentId == itemDepartmentId);
            }
            return await services.Include(x => x.ServiceApprovedBy)
            .OrderByDescending(x => x.DateServiced).ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetAllNotApprovedServicesAsync()
        {
            var services = await _context.Services.Where(x => x.IsServiceApproved == false).Include(x=>x.ServiceDepartment).Include(x => x.Item).ToListAsync();
            services.OrderByDescending(x => x.NewExpiryDate).ThenBy(x => x.ServiceDepartment.Name);
            return services;
        }
    }
}
