using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityManagementSystem.Models;

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
    
    // Foreign keys
    public int DepartmentId { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(DepartmentId))]
    public virtual Department? Department { get; set; }
    
    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}


