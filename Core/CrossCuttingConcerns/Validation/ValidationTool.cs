using FluentValidation;

namespace Core.CrossCuttingConcerns.Validation
{
    public static class ValidationTool
    {
        /// <summary>
        /// Non-generic validator kullanımı için yardımcı metot.
        /// </summary>
        public static void Validate(IValidator validator, object entity)
        {
            var context = new ValidationContext<object>(entity);
            var result = validator.Validate(context);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }

        /// <summary>
        /// Generic tip güvenli validator kullanımı için yardımcı metot.
        /// </summary>
        public static void Validate<T>(IValidator<T> validator, T entity)
        {
            var context = new ValidationContext<T>(entity);
            var result = validator.Validate(context);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}
