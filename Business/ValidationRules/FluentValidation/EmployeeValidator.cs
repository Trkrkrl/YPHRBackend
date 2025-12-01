using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} alanı boş bırakılamaz.")
                .Must(id => id != Guid.Empty).WithMessage("{PropertyName} alanı geçerli bir GUID olmalıdır.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad alanı boş bırakılamaz.")
                .Length(2, 50).WithMessage("Ad en az {MinLength}, en fazla {MaxLength} karakter uzunluğunda olmalıdır.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad alanı boş bırakılamaz.")
                .Length(2, 50).WithMessage("Soyad en az {MinLength}, en fazla {MaxLength} karakter uzunluğunda olmalıdır.");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Departman ID alanı boş bırakılamaz.")
                .Must(id => id != Guid.Empty).WithMessage("Departman ID geçerli bir GUID olmalıdır.");

            RuleFor(x => x.TitleId)
                .NotEmpty().WithMessage("Unvan ID alanı boş bırakılamaz.")
                .Must(id => id != Guid.Empty).WithMessage("Unvan ID geçerli bir GUID olmalıdır.");

            RuleFor(x => x.RegistryNumber)
                .NotEmpty().WithMessage("Sicil Numarası alanı boş bırakılamaz.")
                .Length(5, 20).WithMessage("Sicil Numarası en az {MinLength}, en fazla {MaxLength} karakter olmalıdır.")
                .Matches("^[a-zA-Z0-9]*$").WithMessage("Sicil Numarası sadece harf ve rakam içerebilir.");

        }

       
    }
}
