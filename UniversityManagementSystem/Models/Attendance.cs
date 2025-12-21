using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityManagementSystem.Models;

public class Attendance
{
    public int Id { get; set; }
    
    [Required]
    public DateTime Date { get; set; } = DateTime.Now;
    
    public bool IsPresent { get; set; } = false;
    
    public bool IsLecture { get; set; } = true; // true for lecture, false for lab/section
    
    [MaxLength(200)]
    public string? Notes { get; set; }
    
    // Foreign keys
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int? SectionId { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(StudentId))]
    public virtual Student? Student { get; set; }
    
    [ForeignKey(nameof(CourseId))]
    public virtual Course? Course { get; set; }
    
    [ForeignKey(nameof(SectionId))]
    public virtual Section? Section { get; set; }
    
    [NotMapped]
    public string StatusDisplay => IsPresent ? "Present" : "Absent";
}

