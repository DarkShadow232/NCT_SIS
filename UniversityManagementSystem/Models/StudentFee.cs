using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityManagementSystem.Models;

public class StudentFee
{
    public int Id { get; set; }
    
    public int StudentId { get; set; }
    
    /// <summary>
    /// Academic year (1-4)
    /// </summary>
    [Range(1, 4)]
    public int AcademicYear { get; set; }
    
    /// <summary>
    /// The fees amount for this year based on department
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Amount paid by the student
    /// </summary>
    public decimal AmountPaid { get; set; } = 0;
    
    /// <summary>
    /// Payment status
    /// </summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    /// <summary>
    /// Date when fees were assigned
    /// </summary>
    public DateTime AssignedDate { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Last payment date
    /// </summary>
    public DateTime? LastPaymentDate { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(StudentId))]
    public virtual Student? Student { get; set; }
    
    // Computed properties
    [NotMapped]
    public decimal Balance => Amount - AmountPaid;
    
    [NotMapped]
    public string StatusDisplay => Status switch
    {
        PaymentStatus.Paid => "Paid",
        PaymentStatus.Partial => "Partial",
        PaymentStatus.Pending => "Pending",
        PaymentStatus.Overdue => "Overdue",
        _ => "Unknown"
    };
    
    [NotMapped]
    public string AmountDisplay => $"{Amount:N0} EGP";
    
    [NotMapped]
    public string BalanceDisplay => $"{Balance:N0} EGP";
}

public enum PaymentStatus
{
    Pending,
    Partial,
    Paid,
    Overdue
}

