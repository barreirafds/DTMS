namespace BusinessLogicLayer.DTOs;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string? FieldName { get; set; }

    public static ValidationResult Success() => new ValidationResult { IsValid = true };
    
    public static ValidationResult Failure(string errorMessage, string? fieldName = null) => 
        new ValidationResult { IsValid = false, ErrorMessage = errorMessage, FieldName = fieldName };
}

