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


            modelBuilder.Entity<Color>()
           .HasOne(n => n.Owner)
           .WithMany(w => w.Colors)
           .HasForeignKey(n => n.UserId)
           .OnDelete(DeleteBehavior.NoAction);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (!string.IsNullOrEmpty(tableName) && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }
    }
}
