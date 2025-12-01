using Business.ValidationRules.FluentValidation;
using Entities.Concrete;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class EmployeeServiceTests : TestBase
    {
        private readonly EmployeeValidator _validator;

        public EmployeeServiceTests(BusinessTestFixture fixture) : base(fixture)
        {
            _validator = new EmployeeValidator();
        }

        [Fact]
        public async Task AddAsync_Should_Add_New_Employee_With_Valid_Department_And_Title()
        {
            // Arrange
            var departments = await Fixture.DepartmentService.GetAllAsync(1, 10);
            var titles = await Fixture.TitleService.GetAllAsync(1,10);

            var department = departments.Data.First();
            var title = titles.Data.First();

            var employeeId = Guid.NewGuid();

            var newEmployee = new Employee
            {
                Id = employeeId,
                RegistryNumber = "EMP1001",
                FirstName = "Ali",
                LastName = "Yılmaz",
                DepartmentId = department.Id,
                Department = department,
                TitleId = title.Id,
                Title = title,
                HireDate = DateTime.UtcNow.Date,
                PhotoPath = null,
                ImagePath = string.Empty,
                IsActive = true
            };

            // VALIDATION
            var validationResult = _validator.Validate(newEmployee);
            validationResult.IsValid.Should().BeTrue();

            // Act
            var addResult = await Fixture.EmployeeService.AddAsync(newEmployee);

            // Assert
            addResult.Success.Should().BeTrue();

            var all = await Fixture.EmployeeService.GetAllAsync(1,10);
            all.Data.Should().Contain(p => p.Id == employeeId);
        }

        [Fact]
        public void AddAsync_Should_Fail_Validation_When_Employee_Is_Invalid()
        {
            var invalidEmployee = new Employee
            {
                Id = Guid.Empty,          
                RegistryNumber = "12",    
                FirstName = "",          
                LastName = "A",          
                DepartmentId = Guid.Empty,
                TitleId = Guid.Empty,
                Department = null,
                Title = null,
                HireDate = DateTime.UtcNow.Date,
                PhotoPath = null,
                ImagePath = null,
                IsActive = true
            };

            // Act
            var result = _validator.Validate(invalidEmployee);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Id");
            result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
            result.Errors.Should().Contain(e => e.PropertyName == "LastName");
            result.Errors.Should().Contain(e => e.PropertyName == "RegistryNumber");
            result.Errors.Should().Contain(e => e.PropertyName == "DepartmentId");
            result.Errors.Should().Contain(e => e.PropertyName == "TitleId");
            result.Errors.Should().Contain(e => e.PropertyName == "Department");
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Employee_By_Id()
        {
            var departments = await Fixture.DepartmentService.GetAllAsync(1, 10);
            var titles = await Fixture.TitleService.GetAllAsync(1, 10);

            var department = departments.Data.First();
            var title = titles.Data.First();

            var employeeId = Guid.NewGuid();

            var employee = new Employee
            {
                Id = employeeId,
                RegistryNumber = "EMP1002",
                FirstName = "Mehmet",
                LastName = "Kaya",
                DepartmentId = department.Id,
                Department = department,
                TitleId = title.Id,
                Title = title,
                HireDate = DateTime.UtcNow.Date,
                PhotoPath = null,
                ImagePath = string.Empty,
                IsActive = true
            };

            var validationResult = _validator.Validate(employee);
            validationResult.IsValid.Should().BeTrue();

            await Fixture.EmployeeService.AddAsync(employee);

            // Act
            var result = await Fixture.EmployeeService.GetByIdAsync(employeeId);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(employeeId);
        }
        [Fact]
        public async Task GetAllAsync_Should_Return_All_Employees()
        {
            var departments = await Fixture.DepartmentService.GetAllAsync(1, 10);
            var titles = await Fixture.TitleService.GetAllAsync(1, 10);

            var department = departments.Data.First();
            var title = titles.Data.First();

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                RegistryNumber = "EMP1003",
                FirstName = "Zeynep",
                LastName = "Arslan",
                DepartmentId = department.Id,
                Department = department,
                TitleId = title.Id,
                Title = title,
                HireDate = DateTime.UtcNow.Date,
                PhotoPath = null,
                ImagePath = string.Empty,
                IsActive = true
            };

            var validationResult = _validator.Validate(employee);
            validationResult.IsValid.Should().BeTrue();

            await Fixture.EmployeeService.AddAsync(employee);

            // Act
            var result = await Fixture.EmployeeService.GetAllAsync(1, 10);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Count.Should().BeGreaterOrEqualTo(2); 
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Employee_Basic_Info()
        {
            // Arrange
            var departments = await Fixture.DepartmentService.GetAllAsync(1, 10);
            var titles = await Fixture.TitleService.GetAllAsync(1, 10);

            var department = departments.Data.First();
            var title = titles.Data.First();

            var employeeId = Guid.NewGuid();

            var employee = new Employee
            {
                Id = employeeId,
                RegistryNumber = "EMP1004",
                FirstName = "Deniz",
                LastName = "Çelik",
                DepartmentId = department.Id,
                Department = department,
                TitleId = title.Id,
                Title = title,
                HireDate = DateTime.UtcNow.Date,
                PhotoPath = null,
                ImagePath = string.Empty,
                IsActive = true
            };

            var validationResult = _validator.Validate(employee);
            validationResult.IsValid.Should().BeTrue();

            await Fixture.EmployeeService.AddAsync(employee);

            var all = await Fixture.EmployeeService.GetAllAsync(1, 10);
            var existing = all.Data.First(e => e.Id == employeeId);

            existing.FirstName = "Deniz Güncel";
            existing.LastName = "Çelik Güncel";

            // Act
            var updateResult = await Fixture.EmployeeService.UpdateAsync(existing);

            // Assert
            updateResult.Success.Should().BeTrue();

            var updated = await Fixture.EmployeeService.GetByIdAsync(employeeId);
            updated.Data.FirstName.Should().Be("Deniz Güncel");
            updated.Data.LastName.Should().Be("Çelik Güncel");
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Employee_Status_And_Photo()
        {
            // Arrange
            var departments = await Fixture.DepartmentService.GetAllAsync(1, 10);
            var titles = await Fixture.TitleService.GetAllAsync(1, 10);

            var department = departments.Data.First();
            var title = titles.Data.First();

            var employeeId = Guid.NewGuid();

            var employee = new Employee
            {
                Id = employeeId,
                RegistryNumber = "EMP1005",
                FirstName = "Ayşe",
                LastName = "Demir",
                DepartmentId = department.Id,
                Department = department,
                TitleId = title.Id,
                Title = title,
                HireDate = DateTime.UtcNow.Date,
                PhotoPath = null,
                ImagePath = string.Empty,
                IsActive = true
            };

            var validationResult = _validator.Validate(employee);
            validationResult.IsValid.Should().BeTrue();

            await Fixture.EmployeeService.AddAsync(employee);

            var all = await Fixture.EmployeeService.GetAllAsync(1, 10);
            var added = all.Data.First(p => p.Id == employeeId);

            added.IsActive = false;
            added.PhotoPath = "/uploads/ayse.png";
            added.ImagePath = "/uploads/ayse-thumb.png";

            // Act
            var updateResult = await Fixture.EmployeeService.UpdateAsync(added);

            // Assert
            updateResult.Success.Should().BeTrue();

            var updated = await Fixture.EmployeeService.GetByIdAsync(employeeId);
            updated.Data.IsActive.Should().BeFalse();
            updated.Data.PhotoPath.Should().Be("/uploads/ayse.png");
            updated.Data.ImagePath.Should().Be("/uploads/ayse-thumb.png");
        }
    }
}