using Core.Entities;

namespace Entities.DTOs
{
    public class EmployeeDeleteDto:IDto
    {
        public Guid Id { get; set; }
    }
}
