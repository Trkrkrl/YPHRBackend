using Core.Entities.Concrete;
using Core.Utilities.Security.Hashing;
using DataAccess.Concrete.EntityFramework.Context;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.StartupExtensions
{
    public static class DatabaseInitializerExtensions
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var configuration = services.GetRequiredService<IConfiguration>();
            var dbContext = services.GetRequiredService<AppDbContext>();

            // 1) DB ve tabloları oluştur (migration yerine EnsureCreated)
            try
            {
                await dbContext.Database.EnsureCreatedAsync();
                Console.WriteLine("[DB INIT] EnsureCreated tamamlandı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB INIT] EnsureCreated sırasında hata: {ex.Message}");
                return; // buradan sonrası zaten anlamsız
            }

            // 2) Seed ayarını kontrol et
            var seedSection = configuration.GetSection("SeedAdminUser");
            bool enabled = seedSection.GetValue<bool>("Enabled");
            if (!enabled)
            {
                Console.WriteLine("[DB SEED] SeedAdminUser.Enabled = false, seed atlandı.");
                return;
            }

            var email = seedSection["Email"];
            var password = seedSection["Password"];
            var firstName = seedSection["FirstName"] ?? "Admin";
            var lastName = seedSection["LastName"] ?? "User";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("[DB SEED] Email veya Password boş, seed atlandı.");
                return;
            }

            // 3) Users tablosu gerçekten var mı? (DbSet üzerinden TRY)
            try
            {

                var exists = await dbContext.Users.AnyAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB SEED] Users tablosuna erişilirken hata: {ex.Message}");
                return;
            }

            // 4) Bu email ile kullanıcı zaten var mı kontrolü yapılır
            var existingUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                Console.WriteLine("[DB SEED] Admin user zaten mevcut, seed atlandı.");
                return;
            }

            Console.WriteLine("[DB SEED] Admin user oluşturuluyor...");

            // 5) Hashli şifre ile admin user eklenir
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);


            var user = new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = "active"
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            // 6) ADMIN rol claim'ini ekle / bağla
            const string adminRoleName = "admin"; // SecuredOperation("admin") ile aynı olmalı!

            // OperationClaim var mı?
            var adminClaim = await dbContext.OperationClaims
                .FirstOrDefaultAsync(c => c.Name == adminRoleName);

            if (adminClaim == null)
            {
                adminClaim = new OperationClaim
                {
                    Id = Guid.NewGuid(),
                    Name = adminRoleName
                };

                dbContext.OperationClaims.Add(adminClaim);
                await dbContext.SaveChangesAsync();

                Console.WriteLine("[DB SEED] 'admin' operation claim oluşturuldu.");
            }

            // UserOperationClaim var mı?
            var existingUserOpClaim = await dbContext.UserOperationClaims
                .FirstOrDefaultAsync(uoc => uoc.UserId == user.Id && uoc.OperationClaimId == adminClaim.Id);

            if (existingUserOpClaim == null)
            {
                var userOpClaim = new UserOperationClaim
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    OperationClaimId = adminClaim.Id
                };

                dbContext.UserOperationClaims.Add(userOpClaim);
                await dbContext.SaveChangesAsync();

                Console.WriteLine("[DB SEED] Admin user'a 'admin' rolü bağlandı.");
            }

            Console.WriteLine("[DB SEED] Admin user başarıyla oluşturuldu ve yetkilendirildi.");

            // 7) Department / Title / Employee seed
            await SeedOrganizationAsync(dbContext);
        }
        private static async Task SeedOrganizationAsync(AppDbContext dbContext)
        {
            // Zaten veri varsa tekrar seed etme
            var hasDepartments = await dbContext.Departments.AnyAsync();
            var hasTitles = await dbContext.Titles.AnyAsync();
            var hasEmployees = await dbContext.Employees.AnyAsync();

            if (hasDepartments || hasTitles || hasEmployees)
            {
                Console.WriteLine("[DB SEED] Departments/Titles/Employees zaten var, seed atlandı.");
                return;
            }

            Console.WriteLine("[DB SEED] Departments/Titles/Employees seed ediliyor...");

            // --- GUID'leri önce üretelim ki PK–FK tutarlı olsun ---

            // Departments
            var deptItId = Guid.NewGuid();
            var deptHrId = Guid.NewGuid();
            var deptFinanceId = Guid.NewGuid();

            // Titles
            var titleDevId = Guid.NewGuid();
            var titleHrSpecialistId = Guid.NewGuid();
            var titleFinanceMgrId = Guid.NewGuid();

            // --- Departments ---

            var departments = new List<Department>
    {
        new Department
        {
            Id = deptItId,
            Name = "Information Technology",
            Description = "Yazılım geliştirme, altyapı ve sistem yönetimi."
        },
        new Department
        {
            Id = deptHrId,
            Name = "Human Resources",
            Description = "İK süreçleri, işe alım ve çalışan ilişkileri."
        },
        new Department
        {
            Id = deptFinanceId,
            Name = "Finance",
            Description = "Bütçe, muhasebe ve finansal raporlama."
        }
    };

            // --- Titles ---

            var titles = new List<Title>
    {
        new Title
        {
            Id = titleDevId,
            Name = "Software Developer",
            Description = "Uygulama ve servis geliştiren yazılım mühendisi."
        },
        new Title
        {
            Id = titleHrSpecialistId,
            Name = "HR Specialist",
            Description = "İnsan kaynakları operasyon ve süreç uzmanı."
        },
        new Title
        {
            Id = titleFinanceMgrId,
            Name = "Finance Manager",
            Description = "Finans departmanı yöneticisi."
        }
    };

            // --- Employees ---

            var now = DateTime.UtcNow;

            var employees = new List<Employee>
{
    // --- IT Departmanı (deptItId) ---
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0001", FirstName = "Ahmet", LastName = "Yılmaz", DepartmentId = deptItId, TitleId = titleDevId, HireDate = now.AddYears(-3), IsActive = true, ImagePath = string.Empty },
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0004", FirstName = "Elif", LastName = "Çelik", DepartmentId = deptItId, TitleId = titleDevId, HireDate = now.AddMonths(-20), IsActive = true, ImagePath = string.Empty },
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0005", FirstName = "Onur", LastName = "Kara", DepartmentId = deptItId, TitleId = titleDevId, HireDate = now.AddMonths(-8), IsActive = true, ImagePath = string.Empty },
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0006", FirstName = "Fatma", LastName = "Güneş", DepartmentId = deptItId, TitleId = titleDevId, HireDate = now.AddMonths(-16), IsActive = true, ImagePath = string.Empty },
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0007", FirstName = "Emre", LastName = "Aslan", DepartmentId = deptItId, TitleId = titleDevId, HireDate = now.AddYears(-1), IsActive = true, ImagePath = string.Empty },

    // --- HR Departmanı (deptHrId) ---
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0002", FirstName = "Ayşe", LastName = "Demir", DepartmentId = deptHrId, TitleId = titleHrSpecialistId, HireDate = now.AddYears(-2), IsActive = true, ImagePath = string.Empty },
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0008", FirstName = "Betül", LastName = "Sezer", DepartmentId = deptHrId, TitleId = titleHrSpecialistId, HireDate = now.AddMonths(-14), IsActive = true, ImagePath = string.Empty },
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0009", FirstName = "Tuğba", LastName = "Aydın", DepartmentId = deptHrId, TitleId = titleHrSpecialistId, HireDate = now.AddMonths(-9), IsActive = true, ImagePath = string.Empty },
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0010", FirstName = "Kerem", LastName = "Eroğlu", DepartmentId = deptHrId, TitleId = titleHrSpecialistId, HireDate = now.AddMonths(-22), IsActive = true, ImagePath = string.Empty },

    // --- Finance Departmanı (deptFinanceId) ---
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0003", FirstName = "Mehmet", LastName = "Kaya", DepartmentId = deptFinanceId, TitleId = titleFinanceMgrId, HireDate = now.AddYears(-5), IsActive = true, ImagePath = string.Empty },
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0011", FirstName = "Deniz", LastName = "Tuna", DepartmentId = deptFinanceId, TitleId = titleFinanceMgrId, HireDate = now.AddMonths(-19), IsActive = true, ImagePath = string.Empty },
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0012", FirstName = "Seda", LastName = "Işık", DepartmentId = deptFinanceId, TitleId = titleFinanceMgrId, HireDate = now.AddMonths(-4), IsActive = true, ImagePath = string.Empty },
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0013", FirstName = "Levent", LastName = "Bulut", DepartmentId = deptFinanceId, TitleId = titleFinanceMgrId, HireDate = now.AddYears(-2), IsActive = true, ImagePath = string.Empty },
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0014", FirstName = "Gizem", LastName = "Polat", DepartmentId = deptFinanceId, TitleId = titleFinanceMgrId, HireDate = now.AddMonths(-11), IsActive = true, ImagePath = string.Empty },

    // --- Departman dışı örnek (opsiyonel) ---
    new Employee { Id = Guid.NewGuid(), RegistryNumber = "EMP0015", FirstName = "Selim", LastName = "Doğan", DepartmentId = deptItId, TitleId = titleDevId, HireDate = now.AddYears(-4), IsActive = false, ImagePath = string.Empty },
};


            // --- DB'ye ekle ---

            await dbContext.Departments.AddRangeAsync(departments);
            await dbContext.Titles.AddRangeAsync(titles);
            await dbContext.Employees.AddRangeAsync(employees);

            await dbContext.SaveChangesAsync();

            Console.WriteLine("[DB SEED] Departments/Titles/Employees seed işlemi tamamlandı.");
        }

    }
}