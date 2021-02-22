using System;
using System.Threading;
using System.Threading.Tasks;
using E_Tracker.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace E_Tracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<User,Role,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
         
        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemGroup> ItemGroups { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<MailLog> MailLogs { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<NotificationUser> NotificationUsers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<MailConfig> MailConfigs { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<AutoGenServicePeriod> AutoGenServicePeriods { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Department>().HasNoDiscriminator();

            builder.Entity<NotificationUser>().HasKey(n => new { n.UserId, n.NotificationId });
            base.OnModelCreating(builder);
            builder.Entity<Notification>()
                       .Property(p => p.NotificationType)
                        .HasConversion(
                            new EnumToStringConverter<NotificationType>());

        }


        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var now = DateTime.Now;

            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity is EntityBase entity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            entity.DateCreated = now;
                            entity.IsActive = true;
                            
                            //entity.CreatedById = CurrentUser.GetUsername();
                            
                            break;
                        case EntityState.Modified:
                            Entry(entity).Property(x => x.CreatedById).IsModified = false;
                            Entry(entity).Property(x => x.DateCreated).IsModified = false;
                            entity.DateUpdated = now;
                            
                            break;

                        case EntityState.Detached:
                            Entry(entity).Property(x => x.CreatedById).IsModified = false;
                            Entry(entity).Property(x => x.DateCreated).IsModified = false;
                            entity.DateDeleted = now;

                            break;

                            
                    }
                }

                if (changedEntity.Entity is User userEntity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            userEntity.DateCreated = now;
                            userEntity.IsActive = true;

                            //entity.CreatedById = CurrentUser.GetUsername();

                            break;
                        case EntityState.Modified:
                            Entry(userEntity).Property(x => x.CreatedById).IsModified = false;
                            Entry(userEntity).Property(x => x.DateCreated).IsModified = false;
                            userEntity.DateUpdated = now;

                            break;

                        case EntityState.Detached:
                            Entry(userEntity).Property(x => x.CreatedById).IsModified = false;
                            Entry(userEntity).Property(x => x.DateCreated).IsModified = false;
                            userEntity.DateDeleted = now;

                            break;


                    }
                }

                    if (changedEntity.Entity is Role roleEntity)
                    {
                        switch (changedEntity.State)
                        {
                            case EntityState.Added:
                                roleEntity.DateCreated = now;
                                roleEntity.IsActive = true;

                                //entity.CreatedById = CurrentUser.GetUsername();

                                break;
                            case EntityState.Modified:
                                Entry(roleEntity).Property(x => x.CreatedById).IsModified = false;
                                Entry(roleEntity).Property(x => x.DateCreated).IsModified = false;
                                roleEntity.DateUpdated = now;

                                break;

                            case EntityState.Detached:
                                Entry(roleEntity).Property(x => x.CreatedById).IsModified = false;
                                Entry(roleEntity).Property(x => x.DateCreated).IsModified = false;
                                roleEntity.DateDeleted = now;

                                break;


                        }
                    }
            }

            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

    }
}
