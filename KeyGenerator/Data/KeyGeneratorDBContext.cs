using KeyGenerator.Models;
using Microsoft.EntityFrameworkCore;
using KeyGenerator.Encryptions;

namespace KeyGenerator.Data
{
    public class KeyGeneratorDBContext : DbContext
    {
        public DbSet<Answers> AnswersKeys { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CType> Types { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }
        public DbSet<ExamType> ExamTypes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Paper> Papers { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ProgConfig> ProgConfigs { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<SetofStep> SetofSteps { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAuth> UserAuthentication { get; set; }


        public KeyGeneratorDBContext(DbContextOptions<KeyGeneratorDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Permission>()
                .HasIndex(p => new { p.UserID, p.ModuleID })
                .IsUnique();
            modelBuilder.Entity<User>().HasData(
        new User
        {
            UserID = 1,
            FirstName = "Kartikey",
            MiddleName = "",
            LastName = "Gupta",
            PhoneNumber = "7459076207",
            EmailAddress = "kay.g.3599@gmail.com",
            Designation = "SeniorDev",
            Status = true,
            ProfilePicturePath = ""

        });
            modelBuilder.Entity<UserAuth>().HasData(
        new UserAuth
        {
            UserAuthID = 1,
            UserID = 1,
            Password = Sha256Hasher.ComputeSHA256Hash("Dontaskme!2"),
            AutoGenPass = false

        });
            modelBuilder.Entity<Module>().HasData(
        new Module
        {
            ModuleID = 1,
            ModuleName = "Users"
        });
            modelBuilder.Entity<Module>().HasData(
        new Module
        {
            ModuleID = 2,
            ModuleName = "Key Generator"
        });
            modelBuilder.Entity<Module>().HasData(
        new Module
        {
            ModuleID = 3,
            ModuleName = "Masters"
        });
            modelBuilder.Entity<Permission>().HasData(
        new Permission
        {
            PermissionID = 1,
            ModuleID = 1,
            UserID = 1,
            Can_Add = true,
            Can_Delete = true,
            Can_Update = true,
            Can_View = true,
        });

            modelBuilder.Entity<Permission>().HasData(
        new Permission
        {
            PermissionID = 2,
            ModuleID = 2,
            UserID = 1,
            Can_Add = true,
            Can_Delete = true,
            Can_Update = true,
            Can_View = true,
        });

            modelBuilder.Entity<Permission>().HasData(
        new Permission
        {
            PermissionID = 3,
            ModuleID = 3,
            UserID = 1,
            Can_Add = true,
            Can_Delete = true,
            Can_Update = true,
            Can_View = true,
        });
            _ = modelBuilder.Entity<Session>().HasData(
        new Session
        {
            SessionID = 1,
            SessionName = $"{DateTime.Today.Year.ToString()}-{((DateTime.Today.Year + 1) % 100).ToString()}"
        });
        }
    }
}
