using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class DepartmentValidator : AbstractValidator<Department>
    {
        public DepartmentValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} alanı boş bırakılamaz.")
                .Must(id => id != Guid.Empty).WithMessage("{PropertyName} alanı geçerli bir GUID olmalıdır.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} alanı boş bırakılamaz.")
                .Length(3, 150).WithMessage("{PropertyName} alanı en az {MinLength}, en fazla {MaxLength} karakter uzunluğunda olmalıdır.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("{PropertyName} alanı boş bırakılamaz.")
                .MaximumLength(1000).WithMessage("{PropertyName} alanı en fazla {MaxLength} karakter uzunluğunda olabilir.");
        }
    }
}
