using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels;

public class InstructorsViewModel : BaseViewModel
{
    private readonly UniversityDbContext _context;
    
    private ObservableCollection<Instructor> _instructors = new();
    private ObservableCollection<Department> _departments = new();
    private Instructor? _selectedInstructor;
    private bool _isAdding;
    private bool _isEditing;
    
    // Form fields
    private string _formName = string.Empty;
    private string _formEmail = string.Empty;
    private string _formPhoneNumber = string.Empty;
    private int? _formDepartmentId;
    
    // Validation errors
    private string _nameError = string.Empty;
    private string _emailError = string.Empty;
    private string _phoneError = string.Empty;
    private string _departmentError = string.Empty;
    
    // Statistics
    private int _totalInstructors;
    private Dictionary<string, int> _instructorsByDepartment = new();

    public ObservableCollection<Instructor> Instructors
    {
        get => _instructors;
        set => SetProperty(ref _instructors, value);
    }

    public ObservableCollection<Department> Departments
    {
        get => _departments;
        set => SetProperty(ref _departments, value);
    }

    public Instructor? SelectedInstructor
    {
        get => _selectedInstructor;
        set => SetProperty(ref _selectedInstructor, value);
    }

    public bool IsAdding
    {
        get => _isAdding;
        set => SetProperty(ref _isAdding, value);
    }

    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public string FormName
    {
        get => _formName;
        set => SetProperty(ref _formName, value);
    }

    public string FormEmail
    {
        get => _formEmail;
        set => SetProperty(ref _formEmail, value);
    }

    public string FormPhoneNumber
    {
        get => _formPhoneNumber;
        set => SetProperty(ref _formPhoneNumber, value);
    }

    public int? FormDepartmentId
    {
        get => _formDepartmentId;
        set => SetProperty(ref _formDepartmentId, value);
    }

    public string NameError
    {
        get => _nameError;
        set => SetProperty(ref _nameError, value);
    }

    public string EmailError
    {
        get => _emailError;
        set => SetProperty(ref _emailError, value);
    }

    public string PhoneError
    {
        get => _phoneError;
        set => SetProperty(ref _phoneError, value);
    }

    public string DepartmentError
    {
        get => _departmentError;
        set => SetProperty(ref _departmentError, value);
    }

    public int TotalInstructors
    {
        get => _totalInstructors;
        set => SetProperty(ref _totalInstructors, value);
    }

    public Dictionary<string, int> InstructorsByDepartment
    {
        get => _instructorsByDepartment;
        set => SetProperty(ref _instructorsByDepartment, value);
    }

    // Commands
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }

    public InstructorsViewModel(UniversityDbContext context)
    {
        _context = context;
        
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand(Edit, () => SelectedInstructor != null);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete, () => SelectedInstructor != null);
        
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            
            // Load departments
            Departments = new ObservableCollection<Department>(_context.Departments.ToList());
            
            // Load instructors with their departments
            var instructors = _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.CourseInstructors)
                    .ThenInclude(ci => ci.Course)
                .OrderBy(i => i.Name)
                .ToList();
            
            Instructors = new ObservableCollection<Instructor>(instructors);
            
            // Calculate statistics
            TotalInstructors = instructors.Count;
            InstructorsByDepartment = instructors
                .Where(i => i.Department != null)
                .GroupBy(i => i.Department!.Name)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load instructors: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void Add()
    {
        IsAdding = true;
        IsEditing = true;
        ClearForm();
    }

    private void Edit()
    {
        if (SelectedInstructor == null) return;
        
        IsAdding = false;
        IsEditing = true;
        
        FormName = SelectedInstructor.Name;
        FormEmail = SelectedInstructor.Email ?? string.Empty;
        FormPhoneNumber = SelectedInstructor.PhoneNumber ?? string.Empty;
        FormDepartmentId = SelectedInstructor.DepartmentId;
        
        ClearValidationErrors();
    }

    private void Save()
    {
        if (!ValidateForm()) return;
        
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            
            if (IsAdding)
            {
                var instructor = new Instructor
                {
                    Name = FormName.Trim(),
                    Email = FormEmail.Trim(),
                    PhoneNumber = FormPhoneNumber.Trim(),
                    DepartmentId = FormDepartmentId
                };
                
                _context.Instructors.Add(instructor);
                _context.SaveChanges();
                
                SuccessMessage = $"Instructor '{instructor.Name}' added successfully!";
            }
            else if (SelectedInstructor != null)
            {
                SelectedInstructor.Name = FormName.Trim();
                SelectedInstructor.Email = FormEmail.Trim();
                SelectedInstructor.PhoneNumber = FormPhoneNumber.Trim();
                SelectedInstructor.DepartmentId = FormDepartmentId;
                
                _context.SaveChanges();
                
                SuccessMessage = $"Instructor '{SelectedInstructor.Name}' updated successfully!";
            }
            
            LoadData();
            Cancel();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save instructor: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void Delete()
    {
        if (SelectedInstructor == null) return;
        
        var result = MessageBox.Show($"Are you sure you want to delete {SelectedInstructor.Name}?", 
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                
                var instructorName = SelectedInstructor.Name;
                _context.Instructors.Remove(SelectedInstructor);
                _context.SaveChanges();
                
                SuccessMessage = $"Instructor '{instructorName}' deleted successfully!";
                LoadData();
                Cancel();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to delete instructor: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private void Cancel()
    {
        IsAdding = false;
        IsEditing = false;
        ClearForm();
        ClearValidationErrors();
        SelectedInstructor = null;
    }

    private bool ValidateForm()
    {
        ClearValidationErrors();
        bool isValid = true;
        
        if (string.IsNullOrWhiteSpace(FormName))
        {
            NameError = "Name is required";
            isValid = false;
        }
        else if (FormName.Length < 3)
        {
            NameError = "Name must be at least 3 characters";
            isValid = false;
        }
        
        if (string.IsNullOrWhiteSpace(FormEmail))
        {
            EmailError = "Email is required";
            isValid = false;
        }
        else if (!FormEmail.Contains("@") || !FormEmail.Contains("."))
        {
            EmailError = "Invalid email format";
            isValid = false;
        }
        
        if (string.IsNullOrWhiteSpace(FormPhoneNumber))
        {
            PhoneError = "Phone number is required";
            isValid = false;
        }
        else if (FormPhoneNumber.Length < 10)
        {
            PhoneError = "Phone number must be at least 10 digits";
            isValid = false;
        }
        
        if (FormDepartmentId == null)
        {
            DepartmentError = "Department is required";
            isValid = false;
        }
        
        return isValid;
    }

    private void ClearForm()
    {
        FormName = string.Empty;
        FormEmail = string.Empty;
        FormPhoneNumber = string.Empty;
        FormDepartmentId = null;
    }

    private void ClearValidationErrors()
    {
        NameError = string.Empty;
        EmailError = string.Empty;
        PhoneError = string.Empty;
        DepartmentError = string.Empty;
    }
}
