using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityManagementSystem.Models;

public class Grade
{
    public int Id { get; set; }
    
    [Range(0, 100)]
    public double? Assignment1 { get; set; }
    
    [Range(0, 100)]
    public double? Assignment2 { get; set; }
    
    [Range(0, 100)]
    public double? FinalExam { get; set; }
    
    public double? TotalScore { get; set; }
    
    [MaxLength(10)]
    public string? SymbolicGrade { get; set; }
    
    public bool LeniencyApplied { get; set; } = false;
    
    public DateTime? GradedDate { get; set; }
    
    // Foreign keys
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(StudentId))]
    public virtual Student? Student { get; set; }
    
    [ForeignKey(nameof(CourseId))]
    public virtual Course? Course { get; set; }
    
    // Computed properties
    [NotMapped]
    public bool IsComplete => Assignment1.HasValue && Assignment2.HasValue && FinalExam.HasValue;
    
    [NotMapped]
    public string StatusDisplay => IsComplete ? "Complete" : "Incomplete";
}


