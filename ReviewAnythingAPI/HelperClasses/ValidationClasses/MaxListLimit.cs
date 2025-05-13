using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.HelperClasses.ValidationClasses;

public class MaxListLimit : ValidationAttribute
{
    private readonly int _maxListLimit;

    public MaxListLimit(int maxListLimit)
    {
        _maxListLimit = maxListLimit;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is ICollection collection)
        {
            if (collection.Count > _maxListLimit)
            {
                return new ValidationResult(ErrorMessage ??
                                            $"The collection cannot contain more than {_maxListLimit} items.");
            }
        }

        return ValidationResult.Success;
    }
    
}