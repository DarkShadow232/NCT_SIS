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
    public double? CourseWork { get; set; }  // CW - Added based on PDF requirements
    
    [Range(0, 100)]
    public double? FinalExam { get; set; }
    
    public double? TotalScore { get; set; }
    
    [MaxLength(20)]
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
    
    // Computed properties based on PDF requirements
    [NotMapped]
    public bool IsComplete
    {
        get
        {
            if (Course == null) return false;
            
            // Theoretical courses don't have Final Exam
            if (Course.CourseType == CourseType.Theoretical100)
                return Assignment1.HasValue && Assignment2.HasValue && CourseWork.HasValue;
            
            // Practical courses have all components
            return Assignment1.HasValue && Assignment2.HasValue && CourseWork.HasValue && FinalExam.HasValue;
        }
    }
    
    [NotMapped]
    public string StatusDisplay => IsComplete ? "Complete" : "Incomplete";
}


