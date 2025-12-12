using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;

namespace UniversityManagementSystem.ViewModels;

public class FeesViewModel : BaseViewModel
{
    private ObservableCollection<StudentFee> _studentFees = new();
    private ObservableCollection<Department> _departments = new();
    private ObservableCollection<Student> _students = new();
    private StudentFee? _selectedFee;
    private int? _filterDepartmentId;
    private string? _filterStatus;
    private string _searchText = string.Empty;
    
    // Summary stats
    private decimal _totalFees;
    private decimal _totalCollected;
    private decimal _totalPending;
    private int _paidCount;
    private int _pendingCount;
    
    // Payment form
    private decimal _paymentAmount;
    private bool _isPaymentDialogOpen;
    
    public ObservableCollection<StudentFee> StudentFees
    {
        get => _studentFees;
        set => SetProperty(ref _studentFees, value);
    }
    
    public ObservableCollection<Department> Departments
    {
        get => _departments;
        set => SetProperty(ref _departments, value);
    }
    
    public ObservableCollection<Student> Students
    {
        get => _students;
        set => SetProperty(ref _students, value);
    }
    
    public StudentFee? SelectedFee
    {
        get => _selectedFee;
        set => SetProperty(ref _selectedFee, value);
    }
    
    public int? FilterDepartmentId
    {
        get => _filterDepartmentId;
        set
        {
            if (SetProperty(ref _filterDepartmentId, value))
                LoadFees();
        }
    }
    
    public string? FilterStatus
    {
        get => _filterStatus;
        set
        {
            if (SetProperty(ref _filterStatus, value))
                LoadFees();
        }
    }
    
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                LoadFees();
        }
    }
    
    public decimal TotalFees
    {
        get => _totalFees;
        set => SetProperty(ref _totalFees, value);
    }
    
    public decimal TotalCollected
    {
        get => _totalCollected;
        set => SetProperty(ref _totalCollected, value);
    }
    
    public decimal TotalPending
    {
        get => _totalPending;
        set => SetProperty(ref _totalPending, value);
    }
    
    public int PaidCount
    {
        get => _paidCount;
        set => SetProperty(ref _paidCount, value);
    }
    
    public int PendingCount
    {
        get => _pendingCount;
        set => SetProperty(ref _pendingCount, value);
    }
    
    public decimal PaymentAmount
    {
        get => _paymentAmount;
        set => SetProperty(ref _paymentAmount, value);
    }
    
    public bool IsPaymentDialogOpen
    {
        get => _isPaymentDialogOpen;
        set => SetProperty(ref _isPaymentDialogOpen, value);
    }
    
    public string[] StatusOptions => new[] { "All", "Paid", "Partial", "Pending", "Overdue" };
    
    // Commands
    public ICommand RecordPaymentCommand { get; }
    public ICommand MarkAsPaidCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand GenerateFeesCommand { get; }
    
    public FeesViewModel()
    {
        RecordPaymentCommand = new RelayCommand(RecordPayment, () => SelectedFee != null);
        MarkAsPaidCommand = new RelayCommand(MarkAsPaid, () => SelectedFee != null && SelectedFee.Status != PaymentStatus.Paid);
        ClearFilterCommand = new RelayCommand(ClearFilter);
        RefreshCommand = new RelayCommand(Refresh);
        GenerateFeesCommand = new RelayCommand(GenerateFeesForNewYear);
        
        LoadDepartments();
        LoadFees();
        LoadSummary();
    }
    
    private void LoadFees()
    {
        using var context = new UniversityDbContext();
        var query = context.StudentFees
            .Include(f => f.Student)
            .ThenInclude(s => s!.Department)
            .AsQueryable();
        
        if (FilterDepartmentId.HasValue)
        {
            query = query.Where(f => f.Student != null && f.Student.DepartmentId == FilterDepartmentId.Value);
        }
        
        if (!string.IsNullOrEmpty(FilterStatus) && FilterStatus != "All")
        {
            var status = FilterStatus switch
            {
                "Paid" => PaymentStatus.Paid,
                "Partial" => PaymentStatus.Partial,
                "Pending" => PaymentStatus.Pending,
                "Overdue" => PaymentStatus.Overdue,
                _ => PaymentStatus.Pending
            };
            query = query.Where(f => f.Status == status);
        }
        
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.ToLower();
            query = query.Where(f => f.Student != null && 
                (f.Student.Name.ToLower().Contains(search) || 
                 (f.Student.StudentId != null && f.Student.StudentId.ToLower().Contains(search))));
        }
        
        StudentFees = new ObservableCollection<StudentFee>(
            query.OrderByDescending(f => f.AssignedDate).ToList()
        );
        
        LoadSummary();
    }
    
    private void LoadDepartments()
    {
        using var context = new UniversityDbContext();
        Departments = new ObservableCollection<Department>(context.Departments.OrderBy(d => d.Name).ToList());
    }
    
    private void LoadSummary()
    {
        using var context = new UniversityDbContext();
        var fees = context.StudentFees.ToList();
        
        TotalFees = fees.Sum(f => f.Amount);
        TotalCollected = fees.Sum(f => f.AmountPaid);
        TotalPending = TotalFees - TotalCollected;
        PaidCount = fees.Count(f => f.Status == PaymentStatus.Paid);
        PendingCount = fees.Count(f => f.Status == PaymentStatus.Pending || f.Status == PaymentStatus.Partial);
    }
    
    private void RecordPayment()
    {
        if (SelectedFee == null) return;
        
        // For simplicity, record full balance payment
        var result = MessageBox.Show(
            $"Record payment for {SelectedFee.Student?.Name}?\n\nBalance: {SelectedFee.Balance:N0} EGP\n\nClick Yes to pay full balance, No to pay half.",
            "Record Payment", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Cancel) return;
        
        var amount = result == MessageBoxResult.Yes ? SelectedFee.Balance : SelectedFee.Balance / 2;
        
        using var context = new UniversityDbContext();
        var fee = context.StudentFees.Find(SelectedFee.Id);
        if (fee != null)
        {
            fee.AmountPaid += amount;
            fee.LastPaymentDate = DateTime.Now;
            fee.Status = fee.AmountPaid >= fee.Amount ? PaymentStatus.Paid : PaymentStatus.Partial;
            context.SaveChanges();
        }
        
        LoadFees();
        MessageBox.Show($"Payment of {amount:N0} EGP recorded successfully!", "Payment Recorded", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    private void MarkAsPaid()
    {
        if (SelectedFee == null) return;
        
        var result = MessageBox.Show(
            $"Mark fees for {SelectedFee.Student?.Name} (Year {SelectedFee.AcademicYear}) as fully paid?",
            "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            using var context = new UniversityDbContext();
            var fee = context.StudentFees.Find(SelectedFee.Id);
            if (fee != null)
            {
                fee.AmountPaid = fee.Amount;
                fee.Status = PaymentStatus.Paid;
                fee.LastPaymentDate = DateTime.Now;
                context.SaveChanges();
            }
            LoadFees();
        }
    }
    
    private void ClearFilter()
    {
        SearchText = string.Empty;
        FilterDepartmentId = null;
        FilterStatus = null;
    }
    
    private void Refresh()
    {
        LoadFees();
        LoadSummary();
    }
    
    private void GenerateFeesForNewYear()
    {
        var result = MessageBox.Show(
            "This will generate fee records for students who don't have fees assigned for their current academic year.\n\nContinue?",
            "Generate Fees", MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result != MessageBoxResult.Yes) return;
        
        using var context = new UniversityDbContext();
        var students = context.Students
            .Include(s => s.Department)
            .Include(s => s.Fees)
            .Where(s => s.IsActive && s.DepartmentId != null)
            .ToList();
        
        int generated = 0;
        foreach (var student in students)
        {
            if (student.Department == null) continue;
            
            // Check if fee exists for current year
            if (!student.Fees.Any(f => f.AcademicYear == student.YearLevel))
            {
                context.StudentFees.Add(new StudentFee
                {
                    StudentId = student.Id,
                    AcademicYear = student.YearLevel,
                    Amount = student.Department.AnnualFees,
                    AmountPaid = 0,
                    Status = PaymentStatus.Pending,
                    AssignedDate = DateTime.Now
                });
                generated++;
            }
        }
        
        context.SaveChanges();
        LoadFees();
        
        MessageBox.Show($"Generated {generated} fee records.", "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}

