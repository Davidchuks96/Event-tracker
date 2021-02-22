using AutoMapper;
using E_Tracker.CreateDto;
using E_Tracker.Data;
using E_Tracker.Dto;
using Microsoft.AspNetCore.Identity;

namespace E_Tracker
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Service, ServiceDto>().ReverseMap();
            CreateMap<Service, CreateServiceDto>().ReverseMap();
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Department, CreateDepartmentDto>().ReverseMap();
            CreateMap<Item, ItemDto>().ReverseMap();
            CreateMap<Item, CreateItemDto>().ReverseMap();
            CreateMap<ItemGroup, ItemGroupDto>().ReverseMap();
            CreateMap<ItemGroup, CreateItemGroupDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<ItemType, ItemTypeDto>().ReverseMap();
            CreateMap<ItemType, CreateItemTypeDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<Role, CreateRoleDto>().ReverseMap();
            CreateMap<Permission, CreatePermissionDto>().ReverseMap();
            CreateMap<Permission, PermissionDto>().ReverseMap();
        }
    }
}
