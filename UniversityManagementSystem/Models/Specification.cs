using System.ComponentModel.DataAnnotations;

namespace UniversityManagementSystem.Models;

public enum SpecificationType
{
    Seating = 0,
    AirConditioning = 1,
    Fan = 2,
    Lighting = 3,
    Computer = 4,
    Projector = 5,
    Whiteboard = 6,
    Other = 99
}

public class Specification
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ProductName { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? ProductDescription { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string ProductId { get; set; } = string.Empty;
    
    public SpecificationType Type { get; set; }
    
    public int Quantity { get; set; } = 1;
    
    public bool IsActive { get; set; } = true;
}


