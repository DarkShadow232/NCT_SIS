using System.Text.RegularExpressions;

namespace UniversityManagementSystem.Services;

public static class ValidationService
{
    /// <summary>
    /// Validates an email address format
    /// </summary>
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, emailPattern);
    }
    
    /// <summary>
    /// Validates a phone number (Egyptian format)
    /// </summary>
    public static bool IsValidPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return true; // Phone is optional
        
        // Accept formats like: +20-1012345678, 01012345678, +201012345678
        var phonePattern = @"^(\+?20[-\s]?)?0?1[0125]\d{8}$";
        return Regex.IsMatch(phone.Replace(" ", "").Replace("-", ""), phonePattern);
    }
    
    /// <summary>
    /// Validates student ID format (NCT followed by digits)
    /// </summary>
    public static bool IsValidStudentId(string? studentId)
    {
        if (string.IsNullOrWhiteSpace(studentId))
            return true; // Student ID can be auto-generated
        
        var idPattern = @"^NCT\d{5,}$";
        return Regex.IsMatch(studentId, idPattern);
    }
    
    /// <summary>
    /// Validates course code format
    /// </summary>
    public static bool IsValidCourseCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;
        
        // Format: 2-4 letters followed by 3 digits (e.g., CS101, BUS201)
        var codePattern = @"^[A-Z]{2,4}\d{3}$";
        return Regex.IsMatch(code.ToUpper(), codePattern);
    }
    
    /// <summary>
    /// Validates a required string field
    /// </summary>
    public static bool IsNotEmpty(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
    
    /// <summary>
    /// Validates a score (0-100)
    /// </summary>
    public static bool IsValidScore(double? score)
    {
        if (!score.HasValue)
            return true; // Score can be null
        return score >= 0 && score <= 100;
    }
    
    /// <summary>
    /// Validates capacity (positive number)
    /// </summary>
    public static bool IsValidCapacity(int capacity)
    {
        return capacity > 0 && capacity <= 100;
    }
    
    /// <summary>
    /// Validates credits (1-6)
    /// </summary>
    public static bool IsValidCredits(int credits)
    {
        return credits >= 1 && credits <= 6;
    }
    
    /// <summary>
    /// Validates year level (1-4)
    /// </summary>
    public static bool IsValidYearLevel(int year)
    {
        return year >= 1 && year <= 4;
    }
    
    /// <summary>
    /// Validates a fee amount (positive decimal)
    /// </summary>
    public static bool IsValidFeeAmount(decimal amount)
    {
        return amount >= 0;
    }
    
    /// <summary>
    /// Gets validation error message for email
    /// </summary>
    public static string GetEmailError(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Email is required.";
        if (!IsValidEmail(email))
            return "Please enter a valid email address (e.g., name@nct.edu.eg).";
        return string.Empty;
    }
    
    /// <summary>
    /// Gets validation error message for phone
    /// </summary>
    public static string GetPhoneError(string? phone)
    {
        if (!IsValidPhone(phone))
            return "Please enter a valid Egyptian phone number.";
        return string.Empty;
    }
    
    /// <summary>
    /// Gets validation error message for student ID
    /// </summary>
    public static string GetStudentIdError(string? studentId)
    {
        if (!IsValidStudentId(studentId))
            return "Student ID must start with NCT followed by at least 5 digits.";
        return string.Empty;
    }
    
    /// <summary>
    /// Gets validation error message for course code
    /// </summary>
    public static string GetCourseCodeError(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return "Course code is required.";
        if (!IsValidCourseCode(code))
            return "Course code must be 2-4 letters followed by 3 digits (e.g., CS101).";
        return string.Empty;
    }
    
    /// <summary>
    /// Gets validation error message for score
    /// </summary>
    public static string GetScoreError(double? score, string fieldName)
    {
        if (score.HasValue && !IsValidScore(score))
            return $"{fieldName} must be between 0 and 100.";
        return string.Empty;
    }
}

