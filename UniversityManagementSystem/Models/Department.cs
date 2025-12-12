using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityManagementSystem.Models;

public class Department
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? HeadOfDepartment { get; set; }
    
    public string? Description { get; set; }
    
    /// <summary>
    /// Annual tuition fees for this department in EGP
    /// </summary>
    public decimal AnnualFees { get; set; } = 0;
    
    // Navigation properties
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    
    // Computed property
    [NotMapped]
    public string FeesDisplay => $"{AnnualFees:N0} EGP";
}


