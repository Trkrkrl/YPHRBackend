using Core.Entities;

namespace Entities.Concrete
{
    public class Employee: IEntity
    {
        public Guid Id { get; set; }

        public  string RegistryNumber { get; set; } // Unique

        public  string FirstName { get; set; }
        public  string LastName { get; set; }

        // Relations
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public Guid TitleId { get; set; }
        public Title Title { get; set; } = null!;

        
        public DateTime HireDate { get; set; }

        // Photo file path or URL
        public string? PhotoPath { get; set; }

        public bool IsActive { get; set; } = true;
        public string ImagePath { get; set; }
    }
}
