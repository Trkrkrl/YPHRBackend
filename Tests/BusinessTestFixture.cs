using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Tests
{

    public class BusinessTestFixture : IDisposable
    {
        public AppDbContext DbContext { get; }

        public IDepartmentService DepartmentService { get; }
        public ITitleService TitleService { get; }
        public IEmployeeService EmployeeService { get; }

        public BusinessTestFixture()
        {
            DbContext = new AppDbContext();

            DbContext.Database.EnsureDeleted();
            DbContext.Database.EnsureCreated();

            IDepartmentDal departmentDal = new EfDepartmentDal(DbContext);
            ITitleDal titleDal = new EfTitleDal(DbContext);
            IEmployeeDal personnelDal = new EfEmployeeDal(DbContext);

            DepartmentService = new DepartmentManager(departmentDal);
            TitleService = new TitleManager(titleDal);
            EmployeeService = new EmployeeManager(personnelDal);

            SeedInitialData();
        }

        private void SeedInitialData()
        {
            var itDeptId = Guid.NewGuid();
            var hrDeptId = Guid.NewGuid();
            var juniorId = Guid.NewGuid();
            var seniorId = Guid.NewGuid();

            DbContext.Departments.AddRange(
                new Department
                {
                    Id = itDeptId,
                    Name = "IT",
                    Description = "Bilgi İşlem"
                },
                new Department
                {
                    Id = hrDeptId,
                    Name = "HR",
                    Description = "İnsan Kaynakları"
                }
            );

            DbContext.Titles.AddRange(
                new Title
                {
                    Id = juniorId,
                    Name = "Junior Developer",
                    Description = "Entry level"
                },
                new Title
                {
                    Id = seniorId,
                    Name = "Senior Developer",
                    Description = "Experienced"
                }
            );

            var seedEmployee = new Employee
            {
                Id = Guid.NewGuid(),
                RegistryNumber = "EMP-0001",
                FirstName = "Seed",
                LastName = "Employee",
                DepartmentId = itDeptId,
                TitleId = juniorId,
                HireDate = DateTime.UtcNow.Date,
                PhotoPath = null,
                ImagePath = string.Empty, 
                IsActive = true
            };

            DbContext.Employees.Add(seedEmployee);

            DbContext.SaveChanges();
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
        }
    }
}