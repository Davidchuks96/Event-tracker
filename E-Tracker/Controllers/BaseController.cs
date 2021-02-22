using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Tracker.Data.Enums;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Tracker.Controllers
{
    public class BaseController : Controller
    {      
        public void Alert(string message, Notifications notificationType)
        {
           var msg= notificationType.ToString().ToUpper();
            TempData["msg"] = $"<script>Swal.fire('{msg}!','{message}','{notificationType}');</script>";
        }
    }
}
