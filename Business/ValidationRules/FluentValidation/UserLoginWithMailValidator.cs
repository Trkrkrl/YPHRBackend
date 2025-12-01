using Entities.DTOs;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class UserLoginWithMailValidator : AbstractValidator<UserMailLoginDto>
    {
        public UserLoginWithMailValidator()
        {
            // Boş geçilemez alanlar
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email adresiniz kısmı boş  bırakılamaz!");
            RuleFor(u => u.Password).NotEmpty().WithMessage("Şifre kısmı boş  bırakılamaz!");

            // Email formatı
            RuleFor(c => c.Email)
                .EmailAddress().WithMessage("Geçerli bir Email Adresi giriniz");
        }
    }
}
