using Core.Entities;

namespace Entities.DTOs
{
    public class UserForRegisterDto : IDto
    {
        public string Password { get; set; }
        public string PasswordAgain { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
