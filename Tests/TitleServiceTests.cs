using Business.ValidationRules.FluentValidation;
using Entities.Concrete;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class TitleServiceTests : TestBase
    {
        private readonly TitleValidator _validator;

        public TitleServiceTests(BusinessTestFixture fixture) : base(fixture)
        {
            _validator = new TitleValidator();
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Titles()
        {
            var result = await Fixture.TitleService.GetAllAsync(1, 10);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Count.Should().BeGreaterOrEqualTo(2); 
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Title_By_Id()
        {
            var all = await Fixture.TitleService.GetAllAsync(1,10);
            var anyTitle = all.Data.First();

            var result = await Fixture.TitleService.GetByIdAsync(anyTitle.Id);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(anyTitle.Id);
            result.Data.Name.Should().Be(anyTitle.Name);
        }

        [Fact]
        public async Task AddAsync_Should_Add_New_Title_When_Valid()
        {
            var newTitle = new Title
            {
                Id = Guid.NewGuid(),
                Name = "Team Lead",
                Description = "Takım lideri"
            };

            var validationResult = _validator.Validate(newTitle);
            validationResult.IsValid.Should().BeTrue();

            var addResult = await Fixture.TitleService.AddAsync(newTitle);

            addResult.Success.Should().BeTrue();

            var all = await Fixture.TitleService.GetAllAsync(1,10);
            all.Data.Should().Contain(t => t.Name == "Team Lead");
        }

        [Fact]
        public void AddAsync_Should_Fail_Validation_When_Name_Is_Invalid()
        {
            var invalidTitle = new Title
            {
                Id = Guid.NewGuid(),
                Name = "",         
                Description = "Kısa"
            };

            var result = _validator.Validate(invalidTitle);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Title()
        {
            var all = await Fixture.TitleService.GetAllAsync(1, 10);
            var title = all.Data.First();

            title.Description = "Güncellenmiş açıklama";

            var validationResult = _validator.Validate(title);
            validationResult.IsValid.Should().BeTrue();

            var result = await Fixture.TitleService.UpdateAsync(title);

            result.Success.Should().BeTrue();

            var updated = await Fixture.TitleService.GetByIdAsync(title.Id);
            updated.Data.Description.Should().Be("Güncellenmiş açıklama");
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Title()
        {
            var all = await Fixture.TitleService.GetAllAsync(1, 10);
            var title = all.Data.First();

            var result = await Fixture.TitleService.DeleteAsync(title);

            result.Success.Should().BeTrue();

            var after = await Fixture.TitleService.GetAllAsync(1, 10);
            after.Data.Should().NotContain(t => t.Id == title.Id);
        }
    }
}