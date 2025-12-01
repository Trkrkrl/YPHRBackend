using System;
using System.Text.RegularExpressions;
using Entities.DTOs;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class UserResigterValidator : AbstractValidator<UserForRegisterDto>
    {
        public UserResigterValidator()
        {
            // Zorunlu alanlar
            RuleFor(u => u.FirstName).NotEmpty().WithMessage("Adınız kısmı boş bırakılamaz!");
            RuleFor(u => u.LastName).NotEmpty().WithMessage("Soyadınız kısmı boş bırakılamaz!");
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email adresiniz kısmı boş  bırakılamaz!");
            RuleFor(u => u.PhoneNumber).NotEmpty().WithMessage("Telefon numaranız  kısmı boş  bırakılamaz!");
            RuleFor(u => u.Password).NotEmpty().WithMessage("Şifre kısmı boş  bırakılamaz!");
            RuleFor(u => u.PasswordAgain).NotEmpty().WithMessage("Şifre tekrar kısmı boş  bırakılamaz!");

            // Şifre uzunluğu
            RuleFor(u => u.Password.Length).GreaterThanOrEqualTo(8).WithMessage("Şifreniz 8 karakterden kısa olamaz!");
            RuleFor(u => u.Password.Length).LessThanOrEqualTo(20).WithMessage("Şifreniz 20 karakterden uzun olamaz!");

            // Şifre karmaşıklığı
            RuleFor(u => u.Password)
                .Matches("[A-Z]").WithMessage("Şifreniz en az 1 adet büyük karakter içermelidir!")
                .Matches("[a-z]").WithMessage("Şifreniz en az 1 adet küçük karakter içermelidir!")
                .Matches(@"\d").WithMessage("Şifreniz en az 1 adet rakam içermelidir!")
                .Matches(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]")
                .WithMessage("Şifreniz en az 1 adet özel karakter içermelidir!");

            // Parolalar aynı olmalı
            RuleFor(u => u.PasswordAgain)
                .Equal(u => u.Password).WithMessage("Girmiş olduğunuz şifreler aynı olmalıdır");


            // Email formatı
            RuleFor(c => c.Email)
                .EmailAddress().WithMessage("Geçerli bir Email Adresi giriniz");

            // Telefon numarası formatı
            RuleFor(u => u.PhoneNumber)
                .MinimumLength(10).WithMessage("Telefon numaranız en az 10 karakterden oluşmalıdır.")
                .MaximumLength(20).WithMessage("Telefon numaranız 20 karakterden uzun olamaz.") // +90 ve boşluklar dahil maksimum uzunluğu artırdık.
                .Matches(new Regex(@"^(\+90[\s-]?)?[\s-]?(\(\d{3}\)|\d{3})[\s-]?\d{3}[\s-]?\d{4}$"))
                .WithMessage("Telefon numaranız geçerli formatta değildir. Örnekler: +905551234567, 555 123 4567, +90 555 123 4567");

        }
    }
}
