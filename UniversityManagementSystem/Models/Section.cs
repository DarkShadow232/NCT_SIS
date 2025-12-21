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
    public int? LectureHallId { get; set; }
    public int? LaboratoryId { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(CourseId))]
    public virtual Course? Course { get; set; }
    
    [ForeignKey(nameof(LectureHallId))]
    public virtual LectureHall? LectureHall { get; set; }
    
    [ForeignKey(nameof(LaboratoryId))]
    public virtual Laboratory? Laboratory { get; set; }
    
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    
    // Computed property
    [NotMapped]
    public int EnrolledCount => Students?.Count ?? 0;
    
    [NotMapped]
    public bool IsFull => EnrolledCount >= Capacity;
}


