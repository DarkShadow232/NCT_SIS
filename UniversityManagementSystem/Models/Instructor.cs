namespace UniversityManagementSystem.Models;

public class Instructor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public DateTime HireDate { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;
    
    public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
}

