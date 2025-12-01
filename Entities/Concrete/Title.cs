using Core.Entities;

namespace Entities.Concrete
{
    public class Title : IEntity
    {

        public Guid Id { get; set; }

        public  string Name { get; set; }
        public string? Description { get; set; }

        // Navigation - One Title has many Employees
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
       
    }
}
