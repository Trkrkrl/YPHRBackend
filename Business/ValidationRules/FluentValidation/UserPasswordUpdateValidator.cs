using Entities.DTOs;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class UserPasswordUpdateValidator : AbstractValidator<UserPasswordUpdateDto>
    {
        public UserPasswordUpdateValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Eski şifre boş bırakılamaz!");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Yeni şifre boş bırakılamaz!")
                .MinimumLength(8).WithMessage("Yeni şifre en az 8 karakter olmalıdır!");

            RuleFor(x => x.ConfirmedNewPassword)
                .Equal(x => x.NewPassword).WithMessage("Yeni şifre ile şifre tekrarı aynı olmalıdır!");
        }
    }
}
