using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels;

public class DepartmentsViewModel : BaseViewModel
{
    private ObservableCollection<Department> _departments = new();
    private Department? _selectedDepartment;
    private string _searchText = string.Empty;
    private bool _isEditing;
    private bool _isAdding;
    
    // Form fields
    private string _formName = string.Empty;
    private string _formCode = string.Empty;
    private string _formDescription = string.Empty;
    private string _formAnnualFees = string.Empty;
    
    // Validation errors
    private string _nameError = string.Empty;
    private string _codeError = string.Empty;
    private string _feesError = string.Empty;
    
    // Statistics
    private int _totalStudents;
    private int _totalCourses;
    
    public ObservableCollection<Department> Departments
    {
        get => _departments;
        set => SetProperty(ref _departments, value);
    }
    
    public Department? SelectedDepartment
    {
        get => _selectedDepartment;
        set
        {
            if (SetProperty(ref _selectedDepartment, value) && value != null && !_isAdding)
            {
                LoadDepartmentToForm(value);
                LoadStatistics(value.Id);
                ClearValidationErrors();
            }
        }
    }
    
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                LoadDepartments();
        }
    }
    
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }
    
    public bool IsAdding
    {
        get => _isAdding;
        set => SetProperty(ref _isAdding, value);
    }
    
    // Form properties with validation
    public string FormName
    {
        get => _formName;
        set
        {
            if (SetProperty(ref _formName, value))
                ValidateName();
        }
    }
    
    public string FormCode
    {
        get => _formCode;
        set
        {
            if (SetProperty(ref _formCode, value))
                ValidateCode();
        }
    }
    
    public string FormDescription
    {
        get => _formDescription;
        set => SetProperty(ref _formDescription, value);
    }
    
    public string FormAnnualFees
    {
        get => _formAnnualFees;
        set
        {
            if (SetProperty(ref _formAnnualFees, value))
                ValidateFees();
        }
    }
    
    // Validation errors
    public string NameError
    {
        get => _nameError;
        set => SetProperty(ref _nameError, value);
    }
    
    public string CodeError
    {
        get => _codeError;
        set => SetProperty(ref _codeError, value);
    }
    
    public string FeesError
    {
        get => _feesError;
        set => SetProperty(ref _feesError, value);
    }
    
    public bool HasErrors => !string.IsNullOrEmpty(NameError) || 
                             !string.IsNullOrEmpty(CodeError) || 
                             !string.IsNullOrEmpty(FeesError);
    
    // Statistics
    public int TotalStudents
    {
        get => _totalStudents;
        set => SetProperty(ref _totalStudents, value);
    }
    
    public int TotalCourses
    {
        get => _totalCourses;
        set => SetProperty(ref _totalCourses, value);
    }
    
    // Commands
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearSearchCommand { get; }
    
    public DepartmentsViewModel()
    {
        AddCommand = new RelayCommand(StartAdd);
        EditCommand = new RelayCommand(StartEdit, () => SelectedDepartment != null);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete, () => SelectedDepartment != null);
        ClearSearchCommand = new RelayCommand(ClearSearch);
        
        LoadDepartments();
    }
    
    private void ValidateName()
    {
        if (string.IsNullOrWhiteSpace(FormName))
            NameError = "Department name is required.";
        else if (FormName.Trim().Length < 3)
            NameError = "Name must be at least 3 characters.";
        else
            NameError = string.Empty;
    }
    
    private void ValidateCode()
    {
        if (string.IsNullOrWhiteSpace(FormCode))
            CodeError = "Department code is required.";
        else if (FormCode.Trim().Length < 2 || FormCode.Trim().Length > 10)
            CodeError = "Code must be 2-10 characters.";
        else if (!System.Text.RegularExpressions.Regex.IsMatch(FormCode, @"^[A-Za-z0-9]+$"))
            CodeError = "Code can only contain letters and numbers.";
        else
            CodeError = string.Empty;
    }
    
    private void ValidateFees()
    {
        if (string.IsNullOrWhiteSpace(FormAnnualFees))
        {
            FeesError = "Annual fees are required.";
        }
        else if (!decimal.TryParse(FormAnnualFees, out var fees))
        {
            FeesError = "Please enter a valid amount.";
        }
        else if (fees < 0)
        {
            FeesError = "Fees cannot be negative.";
        }
        else if (fees > 1000000)
        {
            FeesError = "Fees amount seems too high.";
        }
        else
        {
            FeesError = string.Empty;
        }
    }
    
    private void ClearValidationErrors()
    {
        NameError = string.Empty;
        CodeError = string.Empty;
        FeesError = string.Empty;
    }
    
    private bool ValidateAll()
    {
        ValidateName();
        ValidateCode();
        ValidateFees();
        OnPropertyChanged(nameof(HasErrors));
        return !HasErrors;
    }
    
    private void LoadDepartments()
    {
        using var context = new UniversityDbContext();
        var query = context.Departments.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.ToLower();
            query = query.Where(d => d.Name.ToLower().Contains(search) || 
                                     d.Code.ToLower().Contains(search));
        }
        
        Departments = new ObservableCollection<Department>(query.OrderBy(d => d.Name).ToList());
    }
    
    private void LoadStatistics(int departmentId)
    {
        using var context = new UniversityDbContext();
        TotalStudents = context.Students.Count(s => s.DepartmentId == departmentId);
        TotalCourses = context.Courses.Count(c => c.DepartmentId == departmentId);
    }
    
    private void LoadDepartmentToForm(Department dept)
    {
        FormName = dept.Name;
        FormCode = dept.Code;
        FormDescription = dept.Description ?? string.Empty;
        FormAnnualFees = dept.AnnualFees.ToString("F2");
    }
    
    private void ClearForm()
    {
        FormName = string.Empty;
        FormCode = string.Empty;
        FormDescription = string.Empty;
        FormAnnualFees = string.Empty;
        TotalStudents = 0;
        TotalCourses = 0;
        ClearValidationErrors();
    }
    
    private void StartAdd()
    {
        ClearForm();
        IsAdding = true;
        IsEditing = true;
        SelectedDepartment = null;
    }
    
    private void StartEdit()
    {
        if (SelectedDepartment != null)
        {
            IsAdding = false;
            IsEditing = true;
            ClearValidationErrors();
        }
    }
    
    private void Save()
    {
        if (!ValidateAll())
        {
            MessageBox.Show("Please fix the validation errors before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        using var context = new UniversityDbContext();
        
        // Check for duplicate code
        var existingCode = context.Departments.FirstOrDefault(d => 
            d.Code.ToLower() == FormCode.Trim().ToLower() && 
            (IsAdding || d.Id != SelectedDepartment!.Id));
        
        if (existingCode != null)
        {
            CodeError = "This code is already used by another department.";
            MessageBox.Show("A department with this code already exists.", "Duplicate Code", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        var fees = decimal.Parse(FormAnnualFees);
        
        if (IsAdding)
        {
            var department = new Department
            {
                Name = FormName.Trim(),
                Code = FormCode.Trim().ToUpper(),
                Description = string.IsNullOrWhiteSpace(FormDescription) ? null : FormDescription.Trim(),
                AnnualFees = fees
            };
            context.Departments.Add(department);
            context.SaveChanges();
            
            MessageBox.Show($"Department '{FormName}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else if (SelectedDepartment != null)
        {
            var department = context.Departments.Find(SelectedDepartment.Id);
            if (department != null)
            {
                var oldFees = department.AnnualFees;
                
                department.Name = FormName.Trim();
                department.Code = FormCode.Trim().ToUpper();
                department.Description = string.IsNullOrWhiteSpace(FormDescription) ? null : FormDescription.Trim();
                department.AnnualFees = fees;
                
                // Update pending fees if annual fees changed
                if (oldFees != fees)
                {
                    var pendingFees = context.StudentFees
                        .Include(f => f.Student)
                        .Where(f => f.Student.DepartmentId == department.Id && f.Status != PaymentStatus.Paid)
                        .ToList();
                    
                    foreach (var fee in pendingFees)
                    {
                        fee.Amount = fees;
                    }
                }
                
                context.SaveChanges();
                MessageBox.Show($"Department '{FormName}' updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        IsEditing = false;
        IsAdding = false;
        LoadDepartments();
    }
    
    private void Cancel()
    {
        IsEditing = false;
        IsAdding = false;
        ClearValidationErrors();
        if (SelectedDepartment != null)
            LoadDepartmentToForm(SelectedDepartment);
        else
            ClearForm();
    }
    
    private void Delete()
    {
        if (SelectedDepartment == null) return;
        
        using var context = new UniversityDbContext();
        var studentCount = context.Students.Count(s => s.DepartmentId == SelectedDepartment.Id);
        var courseCount = context.Courses.Count(c => c.DepartmentId == SelectedDepartment.Id);
        
        if (studentCount > 0 || courseCount > 0)
        {
            MessageBox.Show($"Cannot delete this department.\n\nIt has {studentCount} students and {courseCount} courses assigned to it.\n\nPlease reassign them first.", 
                "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        var result = MessageBox.Show($"Are you sure you want to delete '{SelectedDepartment.Name}'?", 
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            var department = context.Departments.Find(SelectedDepartment.Id);
            if (department != null)
            {
                context.Departments.Remove(department);
                context.SaveChanges();
                MessageBox.Show("Department deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadDepartments();
            ClearForm();
        }
    }
    
    private void ClearSearch()
    {
        SearchText = string.Empty;
    }
}

