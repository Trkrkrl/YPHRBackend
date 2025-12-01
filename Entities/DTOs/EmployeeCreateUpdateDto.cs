using Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs
{
    public class EmployeeCreateUpdateDto:IDto
    {
        public Guid? Id { get; set; }

        [Required, MinLength(5), MaxLength(20)]
        [RegularExpression("^[a-zA-Z0-9]+$")]
        public string RegistryNumber { get; set; } = null!;

        [Required, MinLength(2), MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required, MinLength(2), MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        public Guid DepartmentId { get; set; }

        [Required]
        public Guid TitleId { get; set; }

        [Required]
        public string HireDate { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
