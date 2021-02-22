using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using E_Tracker.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using E_Tracker.Authorization;
using FluentValidation.AspNetCore;
using E_Tracker.Dto;
using AutoMapper;
using E_Tracker.Repository.DepartmentRepo;
using E_Tracker.Repository.UserRepository;
using E_Tracker.Repository.RoleRepository;
using Hangfire;
using E_Tracker.Repository.ItemRepository;
using E_Tracker.Repository.ItemTypeRepository;
using E_Tracker.Repository.ServiceRepo;
using E_Tracker.Repository.EmailRepo;
using E_Tracker.Repository.PermissionRepository;
using Microsoft.AspNetCore.Authorization;
using E_Tracker.Data.Hubs;
using E_Tracker.Repository.ReminderRepo;
using Microsoft.AspNetCore.Mvc.Authorization;
using Newtonsoft.Json;
using E_Tracker.Repository.CategoryRepository;
using E_Tracker.Repository.AutoGenServicePeriodRepository;
using E_Tracker.Repository.EmailLogRepo;
using Microsoft.AspNetCore.Mvc;
using E_Tracker.Repository.ItemGroupRepository;

namespace E_Tracker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddHangfire(x => x.UseSqlServerStorage(connectionString));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                   (connectionString)));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddIdentity<User,Role>(
                options => options.SignIn.RequireConfirmedAccount = true               
                )
                .AddRoleManager<RoleManager<Role>>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSignalR();
            services.AddHttpContextAccessor();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(CustomClaimsValues.CreateRole, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.CreateRole)));
                options.AddPolicy(CustomClaimsValues.ViewRole, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ViewRole)));
                options.AddPolicy(CustomClaimsValues.EditRole, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.EditRole)));
                options.AddPolicy(CustomClaimsValues.DeleteRole, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.DeleteRole)));
                options.AddPolicy(CustomClaimsValues.ActivateRole, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ActivateRole)));
                
                options.AddPolicy(CustomClaimsValues.CreateUser, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.CreateUser)));
                options.AddPolicy(CustomClaimsValues.ViewUser, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ViewUser)));
                options.AddPolicy(CustomClaimsValues.UpdateUser, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.UpdateUser)));
                options.AddPolicy(CustomClaimsValues.DeleteUser, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.DeleteUser)));
                options.AddPolicy(CustomClaimsValues.ActivateUser, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ActivateUser)));
                
                
                options.AddPolicy(CustomClaimsValues.CreateCategory, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.CreateCategory)));
                options.AddPolicy(CustomClaimsValues.ViewCategory, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ViewCategory)));
                options.AddPolicy(CustomClaimsValues.UpdateCategory, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.UpdateCategory)));
                options.AddPolicy(CustomClaimsValues.DeleteCategory, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.DeleteCategory)));
                options.AddPolicy(CustomClaimsValues.ActivateCategory, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ActivateCategory)));

                options.AddPolicy(CustomClaimsValues.CreateItemType, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.CreateItemType)));
                options.AddPolicy(CustomClaimsValues.ViewItemType, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ViewItemType)));
                options.AddPolicy(CustomClaimsValues.UpdateItemType, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.UpdateItemType)));
                options.AddPolicy(CustomClaimsValues.DeleteItemType, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.DeleteItemType)));
                options.AddPolicy(CustomClaimsValues.ActivateItemType, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ActivateItemType)));

                options.AddPolicy(CustomClaimsValues.CreateItem, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.CreateItem)));
                options.AddPolicy(CustomClaimsValues.ViewItem, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ViewItem)));
                options.AddPolicy(CustomClaimsValues.UpdateItem, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.UpdateItem)));
                options.AddPolicy(CustomClaimsValues.DeleteItem, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.DeleteItem)));
                options.AddPolicy(CustomClaimsValues.ApproveItem, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ApproveItem)));
                options.AddPolicy(CustomClaimsValues.ActivateItem, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ActivateItem)));
                options.AddPolicy(CustomClaimsValues.ViewAllItems, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ViewAllItems))); options.AddPolicy(CustomClaimsValues.CreateItem, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.CreateItem)));
                
                options.AddPolicy(CustomClaimsValues.CreateItemGroup, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.CreateItemGroup)));
                options.AddPolicy(CustomClaimsValues.ViewItemGroup, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ViewItemGroup)));
                options.AddPolicy(CustomClaimsValues.UpdateItemGroup, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.UpdateItemGroup)));
                options.AddPolicy(CustomClaimsValues.DeleteItemGroup, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.DeleteItemGroup)));
                options.AddPolicy(CustomClaimsValues.ApproveItemGroup, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ApproveItemGroup)));
                options.AddPolicy(CustomClaimsValues.ActivateItemGroup, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ActivateItemGroup)));
                options.AddPolicy(CustomClaimsValues.ViewAllItemGroups, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ViewAllItemGroups)));

               
                options.AddPolicy(CustomClaimsValues.ApproveService, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ApproveService)));
                options.AddPolicy(CustomClaimsValues.UpdateService, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.UpdateService)));
                options.AddPolicy(CustomClaimsValues.ViewServices, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ViewServices)));
                options.AddPolicy(CustomClaimsValues.ViewApprovedServices, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ViewApprovedServices)));
                options.AddPolicy(CustomClaimsValues.ViewAllServices, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ViewAllServices)));

                options.AddPolicy(CustomClaimsValues.CreateDepartment, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.CreateDepartment)));
                options.AddPolicy(CustomClaimsValues.ViewDepartment, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ViewDepartment)));
                options.AddPolicy(CustomClaimsValues.UpdateDepartment, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.UpdateDepartment)));
                options.AddPolicy(CustomClaimsValues.DeleteDepartment, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.DeleteDepartment)));
                options.AddPolicy(CustomClaimsValues.ActivateDepartment, p => p.AddRequirements(new AccountAuthorizationRequirement(CustomClaimsValues.ActivateDepartment)));
            });

            // Automatically perform database migration
            services.BuildServiceProvider().GetService<ApplicationDbContext>().Database.Migrate();
            
            services.AddMvc( options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }).SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0)
                .AddFluentValidation(validation =>
                {
                    validation.RegisterValidatorsFromAssemblyContaining<DepartmentValidation>();
                    validation.RegisterValidatorsFromAssemblyContaining<UserValidation>();
                    validation.RegisterValidatorsFromAssemblyContaining<RoleValidation>();
                    validation.RegisterValidatorsFromAssemblyContaining<CreateDto.DepartmentValidation>();
                    validation.RegisterValidatorsFromAssemblyContaining<CreateDto.UserValidation>();
                    validation.RegisterValidatorsFromAssemblyContaining<CreateDto.RoleValidation>();
                    validation.RegisterValidatorsFromAssemblyContaining<CreateDto.ItemValidation>();
                    validation.RegisterValidatorsFromAssemblyContaining<CreateDto.ItemTypeValidation>();
                    validation.RegisterValidatorsFromAssemblyContaining<CreateDto.CategoryValidation>();
                    validation.RegisterValidatorsFromAssemblyContaining <ItemValidation>();
                    validation.RegisterValidatorsFromAssemblyContaining<ItemTypeValidation>();
                    validation.RegisterValidatorsFromAssemblyContaining<CategoryValidation>();
                    validation.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
                });
            services.AddAutoMapper(typeof(Startup));

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<SendGridSettings>(Configuration.GetSection("SendGrid"));

            services.AddHttpContextAccessor();

            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IReminderService, ReminderService>();
            services.AddTransient<IAutoGenServicePeriodService, AutoGenServicePeriodService>();
            services.AddTransient<IServiceRepository, ServiceRepository>();
            services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IItemRepository, ItemRepository>();
            services.AddTransient<IItemGroupRepository, ItemGroupRepository>();
            services.AddTransient<IEmailRepository, EmailRepository>();
            services.AddTransient<IEmailSentLogRepository, EmailSentLogRepository>();
            services.AddTransient<IItemTypeRepository, ItemTypeRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();

            services.AddSingleton<IAuthorizationHandler, AccountAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
            UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Home/Error");
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() },
                //Back to site app url
                //AppPath = Url.Action("Dashboard", "Admin", null, this.Url.ActionContext.HttpContext.Request.Scheme)
            });

            //BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget!"));
            RecurringJob.AddOrUpdate<IAutoGenServicePeriodService>(x => x.UpdateExpiredAutoGenServicePeriodsAsync(), Cron.Daily(7));
            RecurringJob.AddOrUpdate<IReminderService>(x => x.SendReminderMail(), Cron.Daily(7));
            RecurringJob.AddOrUpdate<IReminderService>(x => x.SendExpiredItemMail(), Cron.Daily(7));
            RecurringJob.AddOrUpdate<IReminderService>(x => x.SendExpiredItemMail(), Cron.Daily(12));
            RecurringJob.AddOrUpdate<IReminderService>(x => x.SendReminderMail(), Cron.Daily(11));
            RecurringJob.AddOrUpdate<IReminderService>(x => x.SendReminderMail(), Cron.Daily(15));
            RecurringJob.AddOrUpdate<IReminderService>(x => x.SendReminderMail(), Cron.Daily(14, 40));
            //RecurringJob.AddOrUpdate<IReminderService>( x =>  x.SendReminderMail(), Cron.MinuteInterval(5));
            app.UseHangfireServer();

            CreateAdmin.SeedData(userManager, roleManager);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SignalServer>("/signalServer");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Admin}/{action=Dashboard}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
