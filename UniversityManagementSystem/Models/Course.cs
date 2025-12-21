using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityManagementSystem.Models;

public enum CourseType
{
    Practical150 = 0,      // Practical courses graded out of 150
    Practical100 = 1,      // Practical courses like Big Data, Database, MS Office graded out of 100
    Theoretical100 = 2     // Theoretical courses graded out of 100
}

public enum CurriculumStructure
{
    Practical = 0,
    Theoretical = 1
}

public class Course
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;
    
    public int Credits { get; set; }
    
    [Range(1, 4)]
    public int YearLevel { get; set; }
    
    public string? Description { get; set; }
    
    // New fields based on PDF requirements
    public CourseType CourseType { get; set; } = CourseType.Theoretical100;
    
    public CurriculumStructure CurriculumStructure { get; set; } = CurriculumStructure.Theoretical;
    
    [NotMapped]
    public int MaxDegree => CourseType switch
    {
        CourseType.Practical150 => 150,
        CourseType.Practical100 => 100,
        CourseType.Theoretical100 => 100,
        _ => 100
    };
    
    // Foreign keys
    public int DepartmentId { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(DepartmentId))]
    public virtual Department? Department { get; set; }
    
    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}


