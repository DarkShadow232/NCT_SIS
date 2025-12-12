using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityManagementSystem.Models;

public class Section
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    public int Capacity { get; set; } = 40;
    
    public string? Schedule { get; set; }
    
    public string? Room { get; set; }
    
    // Foreign keys
    public int CourseId { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(CourseId))]
    public virtual Course? Course { get; set; }
    
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    
    // Computed property
    [NotMapped]
    public int EnrolledCount => Students?.Count ?? 0;
    
    [NotMapped]
    public bool IsFull => EnrolledCount >= Capacity;
}


