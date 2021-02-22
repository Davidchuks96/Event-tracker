using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Tracker.Authorization;
using E_Tracker.CreateDto;
using E_Tracker.Data;
using E_Tracker.Data.Enums;
using E_Tracker.Dto;
using E_Tracker.Repository.DepartmentRepo;
using E_Tracker.Repository.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace E_Tracker.Controllers
{
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentRepository _dept;
        private readonly ILogger<DepartmentController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public DepartmentController(IDepartmentRepository deptRepository,IMapper mapper,ILogger<DepartmentController> logger, UserManager<User> userManager, IUserRepository userRepository) 
        {
            _dept = deptRepository;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _userRepository = userRepository;
        }
        private async Task<List<DepartmentDto>> DepartmentListDto(IEnumerable<Department> department)
        {
            var deptDto = new List<DepartmentDto>();
            foreach (var dept in department)
            {
                var map = _mapper.Map<DepartmentDto>(dept);
                var createdUser = await _userRepository.GetUserByIdAsync(dept.CreatedById);
                var deletedUser = await _userRepository.GetUserByIdAsync(dept.DeletedById);
                var updatedUser = await _userRepository.GetUserByIdAsync(dept.UpdatedById);
                map.CreatedBy = _mapper.Map<UserDto>(createdUser);
                map.DeletedBy = _mapper.Map<UserDto>(deletedUser);
                map.UpdatedBy = _mapper.Map<UserDto>(updatedUser);
                deptDto.Add(map);
            }
            return deptDto;
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ViewDepartment)]
        public async Task<IActionResult> Departments()
        {
            var departments = await _dept.GetAllDepartmentAsync();
            var deptDto = await DepartmentListDto(departments);
            return View(deptDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateDepartment)]
        public async Task<IActionResult> ActivateDepartment(string departmentId)
        {
            var dept = await _dept.GetDepartmentByIdAsync(departmentId);
            if (dept is null)
            {
                var msg = $"Department with ID:{departmentId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("Departments");
            }
            await _dept.ActivateDepartmentAsync(dept);
            return RedirectToAction("AllNotActiveDepartments");
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.ActivateDepartment)]
        public async Task<IActionResult> AllNotActiveDepartments()
        {
            var departments = await _dept.GetAllNotActiveDepartmentAsync();
            var departmentDto = await DepartmentListDto(departments);
            return View("Departments", departmentDto);
        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.CreateDepartment)]
        public IActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.CreateDepartment)]
        public async Task<IActionResult> CreateDepartment(CreateDepartmentDto deptDto)
        {
            if (!ModelState.IsValid)
            {
                return View(deptDto);
            }
            var departmentcheck = await _dept.GetDepartmentByNameAsync(deptDto.Name);
            if (departmentcheck == null)
            {
                var department = _mapper.Map<Department>(deptDto);
                var user = await GetCurrentUser();
                department.CreatedById = user.Id;
                var result = await _dept.CreateDepartmentAsync(department);
                if (result.Successful)
                    Alert(result.Message, Notifications.success);
                else
                    Alert(result.Message, Notifications.error);
                return RedirectToAction("Departments");
            }
            else
            {
                Alert($"Department with Name: {deptDto.Name} already exists", Notifications.error);
                return View(deptDto);
            }
        }


        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.UpdateDepartment)]
        public async Task<IActionResult> UpdateDepartment(string departmentId)
        {
            var department = await _dept.GetDepartmentByIdAsync(departmentId);
            if (department is null)
            {
                var msg = $"Department with ID:{departmentId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("Departments");
            }
            var deptDto = _mapper.Map<DepartmentDto>(department);
            return View(deptDto);
        }

        [HttpPost]
        [Authorize(Policy = CustomClaimsValues.UpdateDepartment)]
        public async Task<IActionResult> UpdateDepartment(DepartmentDto DeptDto)
        {
            if (!ModelState.IsValid)
            {
                return View(DeptDto);
            }

            var department = _mapper.Map<Department>(DeptDto);
            var user = await GetCurrentUser();
            department.UpdatedById = user.Id;
            var result = await _dept.UpdateDepartmentAsync(department);
            if (result.Successful)
                Alert(result.Message, Notifications.success);
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("Departments");

        }

        [HttpGet]
        [Authorize(Policy = CustomClaimsValues.DeleteDepartment)]
        public async Task<IActionResult>DeleteDepartment(string departmentId)
        {
            var department = await _dept.GetDepartmentByIdAsync(departmentId);
            if (department is null)
            {
                var msg = $"Department with ID:{departmentId} does not exist";
                Alert(msg, Notifications.error);
                return RedirectToAction("Departments");
            }
            var user = await GetCurrentUser();
            department.DeletedById = user.Id;
            var result = await _dept.DeleteDepartmentAsync(department);
            if (result.Successful)
                Alert(result.Message, Notifications.success);
            else
                Alert(result.Message, Notifications.error);
            return RedirectToAction("Departments");
        }
        public async Task<User> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return user;
        }
    }
}