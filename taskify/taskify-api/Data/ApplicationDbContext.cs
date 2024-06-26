using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using taskify_api.Models;

namespace taskify_api.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options)
           : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<WorkspaceUser> WorkspaceUsers { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<TaskUser> TaskUsers { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<ProjectTag> ProjectTags { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ActivityLog>()
            .HasOne(al => al.Workspace)
            .WithMany()
            .HasForeignKey(al => al.WorkspaceId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ActivityLog>()
                .HasOne(al => al.Activity)
                .WithMany()
                .HasForeignKey(al => al.ActivityId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ActivityLog>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ActivityLog>()
                .HasOne(al => al.ActivityType)
                .WithMany()
                .HasForeignKey(al => al.ActivityTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Note>()
                .HasOne(n => n.Workspace)
                .WithMany(w => w.Notes)
                .HasForeignKey(n => n.WorkspaceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Todo>()
               .HasOne(n => n.Workspace)
               .WithMany(w => w.Todos)
               .HasForeignKey(n => n.WorkspaceId)
               .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<WorkspaceUser>()
               .HasOne(n => n.Workspace)
               .WithMany(w => w.WorkspaceUsers)
               .HasForeignKey(n => n.WorkspaceId)
               .OnDelete(DeleteBehavior.NoAction);


            //modelBuilder.Entity<Color>()
            //   .HasOne(n => n.Owner)
            //   .WithMany(w => w.Colors)
            //   .HasForeignKey(n => n.UserId)
            //   .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<Project>()
            //    .HasOne(p => p.Owner)
            //    .WithMany(u => u.Projects)
            //    .HasForeignKey(p => p.OwnerId)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Workspace)
                .WithMany(w => w.Projects)
                .HasForeignKey(p => p.WorkspaceId)
                .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<ProjectUser>()
            //    .HasOne(pu => pu.User)
            //    .WithMany(u => u.ProjectUsers)
            //    .HasForeignKey(pu => pu.UserId)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProjectUser>()
                .HasOne(pu => pu.Project)
                .WithMany(p => p.ProjectUsers)
                .HasForeignKey(pu => pu.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.Project)
                .WithMany(p => p.TaskModels)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.Status)
                .WithMany(s => s.TaskModels)
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<TaskUser>()
            //    .HasOne(tu => tu.User)
            //    .WithMany(u => u.TaskUsers)
            //    .HasForeignKey(tu => tu.UserId)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TaskUser>()
                .HasOne(tu => tu.Task)
                .WithMany(t => t.TaskUsers)
                .HasForeignKey(tu => tu.TaskId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<ProjectTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.ProjectTags)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.NoAction);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (!string.IsNullOrEmpty(tableName) && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            //SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = "1",
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@admin.com",
                NormalizedUserName = "admin@admin.com",
                Email = "admin@admin.com",
                NormalizedEmail = "admin@admin.com",
                EmailConfirmed = false,
                PasswordHash = "admin@admin.com",
                SecurityStamp = "admin@admin.com",
                ConcurrencyStamp = "admin@admin.com",
                PhoneNumber = "0000000000",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            });

            modelBuilder.Entity<Activity>().HasData(
                new Activity { Title = "Created", Description = "Created" },
                new Activity { Title = "Updated", Description = "Updated" },
                new Activity { Title = "Duplicated", Description = "Duplicated" },
                new Activity { Title = "Uploaded", Description = "Uploaded" },
                new Activity { Title = "Deleted", Description = "Deleted" },
                new Activity { Title = "Updated Status", Description = "Updated Status" },
                new Activity { Title = "Signed", Description = "Signed" },
                new Activity { Title = "Unsigned", Description = "Unsigned" }
            );

            modelBuilder.Entity<ActivityType>().HasData(
                new ActivityType { Title = "Meeting", Description = "Meeting" },
                new ActivityType { Title = "Note", Description = "Note" },
                new ActivityType { Title = "Project", Description = "Project" },
                new ActivityType { Title = "Status", Description = "Status" },
                new ActivityType { Title = "Tag", Description = "Tag" },
                new ActivityType { Title = "Task", Description = "Task" },
                new ActivityType { Title = "Todo", Description = "Todo" },
                new ActivityType { Title = "Activity", Description = "Activity" },
                new ActivityType { Title = "Color", Description = "Color" }
            );

            modelBuilder.Entity<Color>().HasData(
                new Color { Title = "warning", ColorCode = "#ade1f5", UserId = "1", IsDefault = true, Description = "Warning" },
                new Color { Title = "primary", ColorCode = "#e7e7ff", UserId = "1", IsDefault = true, Description = "Primary" },
                new Color { Title = "secondary", ColorCode = "#ebeef0", UserId = "1", IsDefault = true, Description = "Secondary" },
                new Color { Title = "success", ColorCode = "#e8fadf", UserId = "1", IsDefault = true, Description = "Success" },
                new Color { Title = "danger", ColorCode = "#ade1f5", UserId = "1", IsDefault = true, Description = "Danger" },
                new Color { Title = "info", ColorCode = "#d7f5fc", UserId = "1", IsDefault = true, Description = "Info" },
                new Color { Title = "dark", ColorCode = "#dcdfe1", UserId = "1", IsDefault = true, Description = "Dark" }
            );

            modelBuilder.Entity<Status>().HasData(
                new Status { ColorId = 1, Title = "In Review", Description = "Warning", UserId = "1", IsDefault = true },
                new Status { ColorId = 6, Title = "On Going", Description = "Info", UserId = "1", IsDefault = true },
                new Status { ColorId = 2, Title = "Started", Description = "Primary", UserId = "1", IsDefault = true },
                new Status { ColorId = 5, Title = "Default", Description = "Danger", UserId = "1", IsDefault = true }
            );

            modelBuilder.Entity<Priority>().HasData(
                new Priority { Title = "Default", ColorId = 5, IsDefault = true }
            );

            modelBuilder.Entity<Tag>().HasData(
                new Tag { Title = "WEBDESIGN", Description = "WEBDESIGN", ColorId = 2, UserId = "1", IsDefault = true },
                new Tag { Title = "BOOKING AND RESERVATION", Description = "BOOKING AND RESERVATION", ColorId = 7, UserId = "1", IsDefault = true },
                new Tag { Title = "LEARNING AND EDUCATION", Description = "LEARNING AND EDUCATION", ColorId = 6, UserId = "1", IsDefault = true },
                new Tag { Title = "PROJECT MANAGEMENT", Description = "PROJECT MANAGEMENT", ColorId = 1, UserId = "1", IsDefault = true },
                new Tag { Title = "CONTENT MANAGEMENT", Description = "CONTENT MANAGEMENT", ColorId = 5, UserId = "1", IsDefault = true },
                new Tag { Title = "SOCIAL NETWORKING", Description = "SOCIAL NETWORKING", ColorId = 4, UserId = "1", IsDefault = true },
                new Tag { Title = "E-COMMERCE", Description = "E-COMMERCE", ColorId = 3, UserId = "1", IsDefault = true },
                new Tag { Title = "WEB DEVELOPMENT", Description = "WEB DEVELOPMENT", ColorId = 2, UserId = "1", IsDefault = true }
            );
        }

    }
}
