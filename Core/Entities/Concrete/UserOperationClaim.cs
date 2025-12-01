using System;
using Core.Entities;

namespace Core.Entities.Concrete
{
    public class UserOperationClaim : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OperationClaimId { get; set; }
    }
}
