using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityManagementSystem.Models;

public class Student
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? StudentId { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [Range(1, 4)]
    public int YearLevel { get; set; }
    
    public DateTime EnrollmentDate { get; set; } = DateTime.Now;
    
    public bool IsActive { get; set; } = true;
    
    // Foreign keys
    public int? SectionId { get; set; }
    public int? DepartmentId { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(SectionId))]
    public virtual Section? Section { get; set; }
    
    [ForeignKey(nameof(DepartmentId))]
    public virtual Department? Department { get; set; }
    
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public virtual ICollection<StudentFee> Fees { get; set; } = new List<StudentFee>();
    
    // Computed properties
    [NotMapped]
    public string YearLevelDisplay => $"Year {YearLevel}";
    
    [NotMapped]
    public decimal TotalFees => Fees?.Sum(f => f.Amount) ?? 0;
    
    [NotMapped]
    public decimal TotalPaid => Fees?.Sum(f => f.AmountPaid) ?? 0;
    
    [NotMapped]
    public decimal TotalBalance => TotalFees - TotalPaid;
}


