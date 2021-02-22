
using Hangfire.Dashboard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Authorization
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        //private readonly ILogger logger;

        //public HangfireAuthorizationFilter(ILogger logger)
        //{
        //    this.logger = logger;
        //}
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Allow only SuperAdmin authenticated users to see the Dashboard.
            var IsAuthorized =  httpContext.User.IsInRole(RolesList.SuperAdmin);
            //logger.LogInformation($"-------- {httpContext.User.ToString()} --------");
            //logger.LogInformation($"-------- ISAuthorized: {IsAuthorized} --------");
            return IsAuthorized;
        }
    }
}
