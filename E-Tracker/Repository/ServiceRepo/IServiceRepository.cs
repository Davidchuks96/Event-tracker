using E_Tracker.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Tracker.Repository.ServiceRepo
{
    public interface IServiceRepository
    {
        Task<(string Message, bool Successful)> CreateServiceAsync(Service service);
        Task<(string Message, bool Successful)> UpdateServicesAsync(Service service);
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<Service> GetServiceByIdAsync(string serviceId);
        Task<IEnumerable<Service>> GetAllServicesApprovedAsync();
        Task<IEnumerable<Service>> GetAllServicesByServiceDepartmentAsync(string departmentId);
        Task<IEnumerable<Service>> GetAllApprovedServicesByDateAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<Service>> GetAllApprovedServicesByDateItemTypeItemDepartmentAndCategoryAsync(DateTime startDate, DateTime endDate, string itemTypeId, string categoryId, string itemDepartmentId, string userDepartmentId = null);
        Task<IEnumerable<Service>> GetAllNotApprovedServicesAsync();
        Task<IEnumerable<Service>> GetApprovedServicesByServiceDepartmentAsync(string departmentId);


    }
}