namespace UniversityManagementSystem.Models;

public class CourseInstructor
{
    public int Id { get; set; }
    public int InstructorId { get; set; }
    public Instructor? Instructor { get; set; }
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public string AcademicYear { get; set; } = "2024-2025";
    public string Semester { get; set; } = "Fall";
    
    // Computed properties for display
    public string CourseName => Course?.Name ?? "";
    public string CourseCode => Course?.Code ?? "";
    public int YearLevel => Course?.YearLevel ?? 1;
}

