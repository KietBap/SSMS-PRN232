using Microsoft.EntityFrameworkCore;
using SMMS.Domain.Entity;
using BCrypt.Net;

namespace SMMS.Infrastructure.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<ActivityConsent> ActivityConsent { get; set; }
        public virtual DbSet<Blog> Blog { get; set; }
        public virtual DbSet<SchoolClass> SchoolClass { get; set; }
        public virtual DbSet<ConselingSchedule> ConselingSchedule { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<HealthActivity> HealthActivity { get; set; }
        public virtual DbSet<HealthCheckupRecord> HealthCheckupRecord { get; set; }
        public virtual DbSet<HealthProfile> HealthProfile { get; set; }
        public virtual DbSet<MedicalIncident> MedicalIncident { get; set; }
        public virtual DbSet<MedicalRequest> MedicalRequest { get; set; }
        public virtual DbSet<MedicalStock> MedicalStock { get; set; }
        public virtual DbSet<MedicalUsage> MedicalUsage { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<VaccinationCampaign> VaccinationCampaign { get; set; }
        public virtual DbSet<VaccinationRecord> VaccinationRecord { get; set; }
        public virtual DbSet<Otp> Otps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User - Role (N-1)
            modelBuilder.Entity<User>()
                .HasOne(w => w.Role)
                .WithMany(u => u.Users)
                .HasForeignKey(w => w.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // blog - User (N-1)
            modelBuilder.Entity<Blog>()
                .HasOne(w => w.User)
                .WithMany(u => u.Blogs)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification - User (N-1)
            modelBuilder.Entity<Notification>()
                .HasOne(w => w.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Student - Class, User (N-1)
            modelBuilder.Entity<Student>(student =>
            {
                student.HasOne(ur => ur.Parent)
                    .WithMany(u => u.Students)
                    .HasForeignKey(ur => ur.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
                student.HasOne(ur => ur.SchoolClass)
                    .WithMany(r => r.Students)
                    .HasForeignKey(ur => ur.ClassId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // HealthProfile - Student (N-1)
            modelBuilder.Entity<HealthProfile>()
                .HasOne(w => w.Student)
                .WithMany(u => u.HealthProfiles)
                .HasForeignKey(w => w.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // VaccinationRecord - Student, VaccinationCampaign (N-1)
            modelBuilder.Entity<VaccinationRecord>(VR =>
            {
                VR.HasOne(ur => ur.Student)
                    .WithMany(u => u.VaccinationRecords)
                    .HasForeignKey(ur => ur.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
                VR.HasOne(ur => ur.VaccinationCampaign)
                    .WithMany(r => r.VaccinationRecords)
                    .HasForeignKey(ur => ur.VaccinationCampaignId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ActivityConsent - Student, VaccinationCampaign, User, HealthActivity (N-1)
            modelBuilder.Entity<ActivityConsent>(AC =>
            {
                AC.HasOne(ur => ur.Student)
                    .WithMany(u => u.ActivityConsents)
                    .HasForeignKey(ur => ur.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
                AC.HasOne(ur => ur.VaccinationCampaign)
                    .WithMany(r => r.ActivityConsents)
                    .HasForeignKey(ur => ur.VaccinationCampaignId)
                    .OnDelete(DeleteBehavior.Restrict);
                AC.HasOne(ur => ur.User)
                    .WithMany(r => r.ActivityConsents)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                AC.HasOne(ur => ur.HealthActivity)
                    .WithMany(r => r.ActivityConsents)
                    .HasForeignKey(ur => ur.HealthActivityId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // HealthActivity - User (N-1)
            modelBuilder.Entity<HealthActivity>()
                .HasOne(w => w.User)
                .WithMany(u => u.HealthActivities)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // HealthCheckupRecords - HealthActivity, Student (N-1)
            modelBuilder.Entity<HealthCheckupRecord>(hcr =>
            {
                hcr.HasOne(ur => ur.Student)
                    .WithMany(u => u.HealthCheckupRecords)
                    .HasForeignKey(ur => ur.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
                hcr.HasOne(ur => ur.HealthActivity)
                    .WithMany(r => r.HealthCheckupRecords)
                    .HasForeignKey(ur => ur.HealthActivityId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ConselingSchedule - Student, MedicalStaff, Parent, HealthCheckup (N-1)
            modelBuilder.Entity<ConselingSchedule>(AC =>
            {
                AC.HasOne(ur => ur.Student)
                    .WithMany(u => u.ConselingSchedules)
                    .HasForeignKey(ur => ur.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
                AC.HasOne(ur => ur.MedicalStaff)
                    .WithMany(r => r.StaffConselingSchedules)
                    .HasForeignKey(ur => ur.MedicalStaffId)
                    .OnDelete(DeleteBehavior.Restrict);
                AC.HasOne(ur => ur.Parent)
                    .WithMany(r => r.ParentConselingSchedules)
                    .HasForeignKey(ur => ur.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
                AC.HasOne(ur => ur.HealthCheckupRecord)
                    .WithMany(r => r.ConselingSchedules)
                    .HasForeignKey(ur => ur.HealthCheckupId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MedicalIncident - User, Student (N-1)
            modelBuilder.Entity<MedicalIncident>(hcr =>
            {
                hcr.HasOne(ur => ur.Student)
                    .WithMany(u => u.MedicalIncidents)
                    .HasForeignKey(ur => ur.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
                hcr.HasOne(ur => ur.User)
                    .WithMany(r => r.MedicalIncidents)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MedicalUsage - MedicalStock, MedicalIncident (N-1)
            modelBuilder.Entity<MedicalUsage>(hcr =>
            {
                hcr.HasOne(ur => ur.MedicalStock)
                    .WithMany(u => u.MedicalUsages)
                    .HasForeignKey(ur => ur.MedicalStockId)
                    .OnDelete(DeleteBehavior.Restrict);
                hcr.HasOne(ur => ur.MedicalIncident)
                    .WithMany(r => r.MedicalUsages)
                    .HasForeignKey(ur => ur.MedicalIncidentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MedicalRequest - User, Student (N-1)
            modelBuilder.Entity<MedicalRequest>(hcr =>
            {
                hcr.HasOne(ur => ur.User)
                    .WithMany(u => u.MedicalRequests)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                hcr.HasOne(ur => ur.Student)
                    .WithMany(r => r.MedicalRequests)
                    .HasForeignKey(ur => ur.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Otp>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(o => o.User)
                    .WithMany(u => u.Otps)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.SetNull); // Changed from Cascade to SetNull

                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(15);
                entity.Property(e => e.OtpCode).IsRequired().HasMaxLength(6);
                entity.Property(e => e.ExpirationTime).IsRequired();
                entity.Property(e => e.IsUsed).IsRequired();
                entity.Property(e => e.UserId).IsRequired(false); // Explicitly nullable
            });
            //===================================================Seed data================================================================

            //role
            var roleIdAdmin = Guid.NewGuid().ToString();
            var roleIdNurse = Guid.NewGuid().ToString();
            var roleIdManager = Guid.NewGuid().ToString();
            var roleIdParent = Guid.NewGuid().ToString();
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = roleIdAdmin,
                    RoleName = "Admin",
                    CreatedBy = "System",
                    CreatedTime = DateTimeOffset.UtcNow,
                },
                new Role
                {
                    Id = roleIdManager,
                    RoleName = "Manager",
                    CreatedBy = "System",
                    CreatedTime = DateTimeOffset.UtcNow,
                    LastUpdatedTime = DateTimeOffset.UtcNow
                },
                new Role
                {
                    Id = roleIdNurse,
                    RoleName = "Nurse",
                    CreatedBy = "System",
                    CreatedTime = DateTimeOffset.UtcNow,
                    LastUpdatedTime = DateTimeOffset.UtcNow
                },
                new Role
                {
                    Id = roleIdParent,
                    RoleName = "Parent",
                    CreatedBy = "System",
                    CreatedTime = DateTimeOffset.UtcNow,
                    LastUpdatedTime = DateTimeOffset.UtcNow
                });

            // user
            var adminId = Guid.NewGuid().ToString();
            var nurseId = Guid.NewGuid().ToString();
            var managerId = Guid.NewGuid().ToString();
            var parentId = Guid.NewGuid().ToString();
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    RoleId = roleIdAdmin,
                    Email = "admin@gmail.com",
                    FullName = "KICM vippro",
                    Phone = "0987654321",
                    Password = BCrypt.Net.BCrypt.HashPassword("123"),
                    CreatedBy = "SeedData",
                    CreatedTime = DateTimeOffset.UtcNow,
                },
                new User
                {
                    Id = nurseId,
                    RoleId = roleIdNurse,
                    Email = "nurse@gmail.com",
                    FullName = "Jack97",
                    Phone = "0912345678",
                    Password = BCrypt.Net.BCrypt.HashPassword("123"),
                    CreatedBy = "SeedData",
                    CreatedTime = DateTimeOffset.UtcNow,
                },
                new User
                {
                    Id = managerId,
                    RoleId = roleIdManager,
                    Email = "manager@gmail.com",
                    FullName = "FireFly",
                    Phone = "0987651234",
                    Password = BCrypt.Net.BCrypt.HashPassword("123"),
                    CreatedBy = "SeedData",
                    CreatedTime = DateTimeOffset.UtcNow,
                },
                new User
                {
                    Id = parentId,
                    RoleId = roleIdParent,
                    Email = "parent@gmail.com",
                    FullName = "KietBap",
                    Phone = "0987051234",
                    Password = BCrypt.Net.BCrypt.HashPassword("123"),
                    CreatedBy = "SeedData",
                    CreatedTime = DateTimeOffset.UtcNow
                });
        }
    }
}
