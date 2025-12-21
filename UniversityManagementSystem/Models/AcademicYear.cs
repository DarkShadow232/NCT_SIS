using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Models;

public class AcademicYear
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Year { get; set; } = string.Empty; // e.g., "2024-2025"
    
    [MaxLength(50)]
    public string? Semester { get; set; } // e.g., "First Semester", "Second Semester"
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public bool IsActive { get; set; } = false;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public virtual ICollection<AcademicCalendar> CalendarEvents { get; set; } = new List<AcademicCalendar>();
}

public class AcademicCalendar
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public DateTime EventDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    [MaxLength(50)]
    public string? EventType { get; set; } // e.g., "Exam", "Holiday", "Registration", "Lecture"
    
    public bool IsAllDay { get; set; } = true;
    
    // Foreign keys
    public int AcademicYearId { get; set; }
    
    // Navigation properties
    public virtual AcademicYear? AcademicYear { get; set; }
}


