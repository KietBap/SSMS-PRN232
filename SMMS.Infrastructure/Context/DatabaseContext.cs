using Microsoft.EntityFrameworkCore;
using SMMS.Domain.Entity;
using SMMS.Domain.Enum;

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
        public virtual DbSet<HealthActivity> HealthActivity { get; set; }
        public virtual DbSet<HealthCheckupRecord> HealthCheckupRecord { get; set; }
        public virtual DbSet<HealthProfile> HealthProfile { get; set; }
        public virtual DbSet<MedicalIncident> MedicalIncident { get; set; }
        public virtual DbSet<MedicalRequest> MedicalRequest { get; set; }
        public virtual DbSet<MedicationRequestAdministration> MedicationRequestAdministration { get; set; }
        public virtual DbSet<MedicalStock> MedicalStock { get; set; }
        public virtual DbSet<MedicalUsage> MedicalUsage { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<VaccinationCampaign> VaccinationCampaign { get; set; }
        public virtual DbSet<VaccinationRecord> VaccinationRecord { get; set; }
		public virtual DbSet<HealthActivityClass> HealthActivityClasses { get; set; }
		public virtual DbSet<VaccinationCampaignClass> VaccinationCampaignClasses { get; set; }

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
			modelBuilder.Entity<Student>()
				.Property(s => s.StudentNumber)
                .UseIdentityColumn() // Sử dụng UseIdentityColumn để tạo cột tự tăng
				.ValueGeneratedOnAdd(); // Đặt StudentNumber là cột tự tăng (identity)

			modelBuilder.Entity<Student>()
				.Property(s => s.StudentCode)
				.HasComputedColumnSql("'STD' + CAST([StudentNumber] AS VARCHAR(10))");

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
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false); // Explicitly nullable
				AC.HasOne(ur => ur.User)
                    .WithMany(r => r.ActivityConsents)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                AC.HasOne(ur => ur.HealthActivity)
                    .WithMany(r => r.ActivityConsents)
                    .HasForeignKey(ur => ur.HealthActivityId)
                    .OnDelete(DeleteBehavior.Restrict)
					.IsRequired(false); // Explicitly nullable
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

            // MedicationRequestAdministration - MedicalRequest, User (N-1)
            modelBuilder.Entity<MedicationRequestAdministration>(mra =>
            {
                mra.HasOne(ur => ur.MedicalRequest)
                    .WithMany(u => u.MedicationRequestAdministrations)
                    .HasForeignKey(ur => ur.MedicalRequestId)
                    .OnDelete(DeleteBehavior.Restrict);
                mra.HasOne(ur => ur.Administrator)
                    .WithMany()
                    .HasForeignKey(ur => ur.AdministeredBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

			modelBuilder.Entity<HealthActivityClass>()
			.HasOne(hac => hac.HealthActivity)
			.WithMany(ha => ha.HealthActivityClasses)
			.HasForeignKey(hac => hac.HealthActivityId)
			.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<HealthActivityClass>()
				.HasOne(hac => hac.SchoolClass)
				.WithMany()
				.HasForeignKey(hac => hac.SchoolClassId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<VaccinationCampaignClass>()
				.HasOne(vcc => vcc.VaccinationCampaign)
				.WithMany(vc => vc.VaccinationCampaignClasses)
				.HasForeignKey(vcc => vcc.VaccinationCampaignId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<VaccinationCampaignClass>()
				.HasOne(vcc => vcc.SchoolClass)
				.WithMany()
				.HasForeignKey(vcc => vcc.SchoolClassId)
				.OnDelete(DeleteBehavior.Restrict);
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
			// SchoolClass
			var classId1 = Guid.NewGuid().ToString();
			var classId2 = Guid.NewGuid().ToString();
			modelBuilder.Entity<SchoolClass>().HasData(
				new SchoolClass
				{
					Id = classId1,
					ClassName = "Class 10A",
					ClassRoom = "Room 101",
					Quantity = 30,
					CreatedBy = "System",
					CreatedTime = DateTimeOffset.UtcNow
				},
				new SchoolClass
				{
					Id = classId2,
					ClassName = "Class 10B",
					ClassRoom = "Room 102",
					Quantity = 28,
					CreatedBy = "System",
					CreatedTime = DateTimeOffset.UtcNow
				});

			// Student
			var studentId1 = Guid.NewGuid().ToString();
			var studentId2 = Guid.NewGuid().ToString();
			modelBuilder.Entity<Student>().HasData(
				new Student
				{
					Id = studentId1,
					ParentId = parentId,
					ClassId = classId1,
					FullName = "Nguyen Van A",
					Gender = "Male",
					DateOfBirth = new DateTime(2010, 5, 15),
					CreatedBy = "System",
					CreatedTime = DateTimeOffset.UtcNow
				},
				new Student
				{
					Id = studentId2,
					ParentId = parentId,
					ClassId = classId2,
					FullName = "Tran Thi B",
					Gender = "Female",
					DateOfBirth = new DateTime(2010, 8, 20),
					CreatedBy = "System",
					CreatedTime = DateTimeOffset.UtcNow
				});
			modelBuilder.Entity<HealthProfile>().HasData(
				new HealthProfile
				{
					Id = Guid.NewGuid().ToString(),
					StudentId = studentId1,
					Vision = "20/20",
					Hearing = "Normal",
					Dental = "No cavities",
					BMI = 20.5,
					AbnormalNote = "None",
					VaccinationHistory = "Fully using Rocket1h",
					CreatedBy = "System",
					CreatedTime = DateTimeOffset.UtcNow
				},
				new HealthProfile
				{
					Id = Guid.NewGuid().ToString(),
					StudentId = studentId2,
					Vision = "20/25",
					Hearing = "Normal",
					Dental = "Minor cavities",
					BMI = 19.8,
					AbnormalNote = "Monitor dental health",
					VaccinationHistory = "Fully using Rocket24/7",
					CreatedBy = "System",
					CreatedTime = DateTimeOffset.UtcNow
				});
			var healthActivityId = Guid.NewGuid().ToString();
			modelBuilder.Entity<HealthActivity>().HasData(
				new HealthActivity
				{
					Id = healthActivityId,
					UserId = nurseId,
					Name = "Bet88",
					Description = "Nha Cai Hang Dau So 1 Dong Nam A",
					ScheduledDate = new DateTime(2024, 12, 1),
					Status = ApprovalStatus.Pending,
					CreatedBy = nurseId,
					CreatedTime = DateTimeOffset.UtcNow
				}
			);

			modelBuilder.Entity<HealthActivityClass>().HasData(
				new HealthActivityClass
				{
					Id = Guid.NewGuid().ToString(),
					HealthActivityId = healthActivityId,
					SchoolClassId = classId1,
					CreatedBy = "System",
					CreatedTime = DateTimeOffset.UtcNow
				}
			);

			// VaccinationCampaign
			var vaccinationCampaignId = Guid.NewGuid().ToString();
			modelBuilder.Entity<VaccinationCampaign>().HasData(
				new VaccinationCampaign
				{
					Id = vaccinationCampaignId,
					UserId = nurseId,
					Name = "KT88",
					VaccineName = "Nha Cai Hang Dau So 1 Chau Au",
					EXP = new DateTime(2025, 12, 1),
					MFG = new DateTime(2024, 1, 1),
					VaccineType = "Flu",
					StartDate = new DateTime(2024, 11, 15),
					Status = ApprovalStatus.Pending,
					CreatedBy = nurseId,
					CreatedTime = DateTimeOffset.UtcNow
				}
			);

			modelBuilder.Entity<VaccinationCampaignClass>().HasData(
				new VaccinationCampaignClass
				{
					Id = Guid.NewGuid().ToString(),
					VaccinationCampaignId = vaccinationCampaignId,
					SchoolClassId = classId2,
					CreatedBy = "System",
					CreatedTime = DateTimeOffset.UtcNow
				}
			);
            modelBuilder.Entity<MedicalStock>().HasData(
					new MedicalStock
					{
						Id = Guid.NewGuid().ToString(),
						Name = "Rocket1s",
						Quantity = 100,
                        DetailInformation = "A supplement for enhancing health and vitality",
						ExpiryDate = new DateTime(2025, 12, 31),
						Status = MedicalStockStatus.Available,
						CreatedBy = "System",
						CreatedTime = DateTimeOffset.UtcNow
					},
					new MedicalStock
					{
						Id = Guid.NewGuid().ToString(),
						Name = "Rocket1m",
						Quantity = 50,
						DetailInformation = "A supplement for enhancing health and vitality",
						ExpiryDate = new DateTime(2026, 6, 30),
						Status = MedicalStockStatus.Available,
						CreatedBy = "System",
						CreatedTime = DateTimeOffset.UtcNow
					},

					new MedicalStock
					{
						Id = Guid.NewGuid().ToString(),
						Name = "Rocket1h",
						Quantity = 50,
						DetailInformation = "A supplement for enhancing health and vitality",
						ExpiryDate = new DateTime(2026, 6, 30),
						Status = MedicalStockStatus.Available,
						CreatedBy = "System",
						CreatedTime = DateTimeOffset.UtcNow
					},

					new MedicalStock
					{
						Id = Guid.NewGuid().ToString(),
						Name = "Rocket12h",
						Quantity = 50,
						DetailInformation = "A supplement for enhancing health and vitality",
						ExpiryDate = new DateTime(2026, 6, 30),
						Status = MedicalStockStatus.Available,
						CreatedBy = "System",
						CreatedTime = DateTimeOffset.UtcNow
					},

					new MedicalStock
					{
						Id = Guid.NewGuid().ToString(),
						Name = "Rocket-24/7",
						Quantity = 50,
						DetailInformation = "A supplement for enhancing health and vitality",
						ExpiryDate = new DateTime(2026, 6, 30),
						Status = MedicalStockStatus.Available,
						CreatedBy = "System",
						CreatedTime = DateTimeOffset.UtcNow
					}
				);
		}
    }
}

