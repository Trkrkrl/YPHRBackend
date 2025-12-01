using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class TitleValidator : AbstractValidator<Title>
    {
        public TitleValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} alanı boş bırakılamaz.")
                .Must(id => id != Guid.Empty).WithMessage("{PropertyName} alanı geçerli bir GUID olmalıdır.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} alanı boş bırakılamaz.")
                .Length(3, 100).WithMessage("{PropertyName} alanı en az {MinLength}, en fazla {MaxLength} karakter uzunluğunda olmalıdır.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("{PropertyName} alanı boş bırakılamaz.")
                .MaximumLength(500).WithMessage("{PropertyName} alanı en fazla {MaxLength} karakter uzunluğunda olabilir.");
        }
    }
}
