namespace UniversityManagementSystem.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Student"; // Admin, Student, Instructor
    public bool IsActive { get; set; } = true;
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // Link to Student or Instructor
    public int? StudentId { get; set; }
    public Student? Student { get; set; }
    
    public int? InstructorId { get; set; }
    public Instructor? Instructor { get; set; }
    
    public string DisplayName => Student?.Name ?? Instructor?.Name ?? Username;
    public string RoleDisplay => Role switch
    {
        "Admin" => "Administrator",
        "Student" => "Student",
        "Instructor" => "Instructor",
        _ => Role
    };
}

