using Core.Entities;

namespace Entities.DTOs
{
    public class UserPasswordUpdateDto : IDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmedNewPassword { get; set; }
    }
}
