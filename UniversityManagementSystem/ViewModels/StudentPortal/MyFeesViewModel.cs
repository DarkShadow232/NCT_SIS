using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels.StudentPortal;

public class MyFeesViewModel : BaseViewModel
{
    private ObservableCollection<StudentFee> _fees = new();
    private decimal _totalFees;
    private decimal _totalPaid;
    private decimal _balanceDue;
    private double _paymentProgress;
    private string _paymentStatus = "Pending";
    
    public ObservableCollection<StudentFee> Fees
    {
        get => _fees;
        set => SetProperty(ref _fees, value);
    }
    
    public decimal TotalFees
    {
        get => _totalFees;
        set => SetProperty(ref _totalFees, value);
    }
    
    public decimal TotalPaid
    {
        get => _totalPaid;
        set => SetProperty(ref _totalPaid, value);
    }
    
    public decimal BalanceDue
    {
        get => _balanceDue;
        set => SetProperty(ref _balanceDue, value);
    }
    
    public double PaymentProgress
    {
        get => _paymentProgress;
        set => SetProperty(ref _paymentProgress, value);
    }
    
    public string PaymentStatus
    {
        get => _paymentStatus;
        set => SetProperty(ref _paymentStatus, value);
    }
    
    public MyFeesViewModel()
    {
        LoadFees();
    }
    
    private void LoadFees()
    {
        var user = AuthenticationService.Instance.CurrentUser;
        if (user?.StudentId == null) return;
        
        using var context = new UniversityDbContext();
        
        var fees = context.StudentFees
            .Where(f => f.StudentId == user.StudentId)
            .OrderByDescending(f => f.AcademicYear)
            .ToList();
        
        Fees = new ObservableCollection<StudentFee>(fees);
        
        TotalFees = fees.Sum(f => f.Amount);
        TotalPaid = fees.Sum(f => f.AmountPaid);
        BalanceDue = TotalFees - TotalPaid;
        
        PaymentProgress = TotalFees > 0 ? (double)(TotalPaid / TotalFees) * 100 : 0;
        
        PaymentStatus = BalanceDue <= 0 ? "Paid" : 
                        TotalPaid > 0 ? "Partial" : "Pending";
    }
}

