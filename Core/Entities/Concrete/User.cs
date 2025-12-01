using System;
using Core.Entities;

namespace Core.Entities.Concrete
{
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public string Status { get; set; } = "active";
        public int FailedRecentLoginAttempts { get; set; }
        public DateTime EndOfSuspension { get; set; }
    }
}
