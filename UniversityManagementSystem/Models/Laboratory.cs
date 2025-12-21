using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityManagementSystem.Models;

public class Laboratory
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Range(1, 100)]
    public int Capacity { get; set; } = 20;
    
    // Lab-specific specifications based on PDF requirements
    public int NumberOfComputers { get; set; } = 0;
    public int NumberOfSeats { get; set; } = 0;
    public int NumberOfAirConditioners { get; set; } = 0;
    public int NumberOfFans { get; set; } = 0;
    public int NumberOfLights { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    // Specification references (list of product IDs)
    [MaxLength(500)]
    public string? ComputerSpecIds { get; set; }  // Comma-separated IDs
    
    [MaxLength(500)]
    public string? SeatingSpecIds { get; set; }
    
    [MaxLength(500)]
    public string? AirConditioningSpecIds { get; set; }
    
    [MaxLength(500)]
    public string? FanSpecIds { get; set; }
    
    [MaxLength(500)]
    public string? LightingSpecIds { get; set; }
    
    // Foreign keys
    public int? DepartmentId { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(DepartmentId))]
    public virtual Department? Department { get; set; }
    
    public virtual ICollection<Section> LabSections { get; set; } = new List<Section>();
    
    [NotMapped]
    public string CapacityDisplay => $"{Capacity} students, {NumberOfComputers} computers";
}


