using System;
using Core.Entities;

namespace Core.Entities.Concrete
{
    public class OperationClaim : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
