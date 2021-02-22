using System;
using E_Tracker.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using System.IO;
//using NLog.Web;

namespace E_Tracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            //try
            //{
            //    logger.Debug("init main");
            //    CreateHostBuilder(args).Build().Run();
            //}
            //catch (Exception exception)
            //{
            //    //NLog: catch setup errors
            //    logger.Error(exception, "Stopped program because of exception");
            //    throw;
            //}
            //finally
            //{
            //    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            //    NLog.LogManager.Shutdown();
            //}

            var host = CreateHostBuilder(args).Build();

            ////using (var scope = host.Services.CreateScope())
            ////{
            ////    var serviceProvider = scope.ServiceProvider;
            ////    try
            ////    {
            ////        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            ////        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();                  

            ////        CreateAdmin.SeedData(userManager, roleManager);
            ////    }
            ////    catch(Exception ex)
            ////    {
            ////       var error = ex.Message;
            ////    }
            ////}

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
               .ConfigureLogging((hostingContext, logging) =>
               {
                   logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                   logging.AddDebug();
                   logging.AddNLog();
               })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
                //.ConfigureLogging(logging =>
                //{
                //    logging.ClearProviders();
                //    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                //})
                //.UseNLog();  // NLog: Setup NLog for Dependency injection
    }
}
