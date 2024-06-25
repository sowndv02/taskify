using System.ComponentModel.DataAnnotations;

namespace taskify_api.Validation
{

    public class DateRangeAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateRangeAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

            if (currentValue < comparisonValue)
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be earlier than {_comparisonProperty}.");
        }
    }
}
