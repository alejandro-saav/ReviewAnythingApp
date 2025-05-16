using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.HelperClasses.ValidationClasses;

public class AllowedValuesAttribute : ValidationAttribute
{
    private readonly int[] _allowed;
    
    public AllowedValuesAttribute(params int[] allowed) { _allowed = allowed; }

    public override bool IsValid(object? value)
    {
        if (value == null) return false;
        return _allowed.Contains((int)value);
    }
}