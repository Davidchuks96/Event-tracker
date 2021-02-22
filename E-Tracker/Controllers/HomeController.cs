using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using E_Tracker.Models;
using E_Tracker.Repository.DepartmentRepo;
using AutoMapper;
using E_Tracker.Data.Enums;
using Microsoft.AspNetCore.Diagnostics;

namespace E_Tracker.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, 
            IDepartmentRepository departmentRepository, 
            IMapper mapper)
        {
            _logger = logger;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
           
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature =
            HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            _logger.LogError($"The path {exceptionHandlerPathFeature?.Path} " +
                $"threw an exception {exceptionHandlerPathFeature?.Error}");

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
