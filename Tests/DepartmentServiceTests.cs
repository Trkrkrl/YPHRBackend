using Business.ValidationRules.FluentValidation;
using Entities.Concrete;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class DepartmentServiceTests : TestBase
    {
        private readonly DepartmentValidator _validator;

        public DepartmentServiceTests(BusinessTestFixture fixture) : base(fixture)
        {
            _validator = new DepartmentValidator();
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Departments()
        {
            // Act
            var result = await Fixture.DepartmentService.GetAllAsync(1,10);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Count.Should().BeGreaterOrEqualTo(2); 
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Department_By_Id()
        {
            // Arrange
            var all = await Fixture.DepartmentService.GetAllAsync(1,10);
            var anyDept = all.Data.First();

            // Act
            var result = await Fixture.DepartmentService.GetByIdAsync(anyDept.Id);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(anyDept.Id);
            result.Data.Name.Should().Be(anyDept.Name);
        }

   
        [Fact]
        public async Task AddAsync_Should_Add_New_Department_When_Valid()
        {
            // Arrange
            var newDepartment = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Finance",
                Description = "Finans Departmanı"
            };

            var validationResult = _validator.Validate(newDepartment);
            validationResult.IsValid.Should().BeTrue("valid bir department ekliyoruz");

            // Act
            var addResult = await Fixture.DepartmentService.AddAsync(newDepartment);

            // Assert
            addResult.Success.Should().BeTrue();

            var all = await Fixture.DepartmentService.GetAllAsync(1, 10);
            all.Data.Should().Contain(d => d.Name == "Finance");
        }

        [Fact]
        public void AddAsync_Should_Fail_Validation_When_Name_Is_Invalid()
        {
            // Arrange
            var invalidDepartment = new Department
            {
                Id = Guid.NewGuid(),
                Name = "IT",              
                Description = ""         
            };

            // Act
            var result = _validator.Validate(invalidDepartment);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Description");

            // Not: Burada bilerek servis çağırmıyoruz, çünkü gerçek senaryoda
            // AOP ValidationAspect burada devreye girip AddAsync'e gidişi engelleyecek.
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Existing_Department()
        {
            // Arrange
            var all = await Fixture.DepartmentService.GetAllAsync(1,10  );
            var dept = all.Data.First();

            dept.Name = "Updated Department";

            var validationResult = _validator.Validate(dept);
            validationResult.IsValid.Should().BeTrue();

            // Act
            var updateResult = await Fixture.DepartmentService.UpdateAsync(dept);

            // Assert
            updateResult.Success.Should().BeTrue();

            var updated = await Fixture.DepartmentService.GetByIdAsync(dept.Id);
            updated.Data.Name.Should().Be("Updated Department");
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Department()
        {
            // Arrange
            var all = await Fixture.DepartmentService.GetAllAsync(1, 10);
            var dept = all.Data.First();

            // Act
            var deleteResult = await Fixture.DepartmentService.DeleteAsync(dept);

            // Assert
            deleteResult.Success.Should().BeTrue();

            var allAfter = await Fixture.DepartmentService.GetAllAsync(1, 10);
            allAfter.Data.Should().NotContain(d => d.Id == dept.Id);
        }
    }
}