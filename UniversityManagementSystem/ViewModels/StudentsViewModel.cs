using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels;

public class StudentsViewModel : BaseViewModel
{
    private ObservableCollection<Student> _students = new();
    private ObservableCollection<Section> _sections = new();
    private ObservableCollection<Department> _departments = new();
    private Student? _selectedStudent;
    private string _searchText = string.Empty;
    private int? _filterYearLevel;
    private int? _filterDepartmentId;
    private bool _isEditing;
    private bool _isAdding;
    
    // Form fields
    private string _formName = string.Empty;
    private string _formEmail = string.Empty;
    private string _formStudentId = string.Empty;
    private string _formPhone = string.Empty;
    private int _formYearLevel = 1;
    private int? _formSectionId;
    private int? _formDepartmentId;
    private bool _formIsActive = true;
    
    // Validation errors
    private string _nameError = string.Empty;
    private string _emailError = string.Empty;
    private string _studentIdError = string.Empty;
    private string _phoneError = string.Empty;
    private string _departmentError = string.Empty;
    
    public ObservableCollection<Student> Students
    {
        get => _students;
        set => SetProperty(ref _students, value);
    }
    
    public ObservableCollection<Section> Sections
    {
        get => _sections;
        set => SetProperty(ref _sections, value);
    }
    
    public ObservableCollection<Department> Departments
    {
        get => _departments;
        set => SetProperty(ref _departments, value);
    }
    
    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set
        {
            if (SetProperty(ref _selectedStudent, value) && value != null && !_isAdding)
            {
                LoadStudentToForm(value);
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
                LoadStudents();
        }
    }
    
    public int? FilterYearLevel
    {
        get => _filterYearLevel;
        set
        {
            if (SetProperty(ref _filterYearLevel, value))
                LoadStudents();
        }
    }
    
    public int? FilterDepartmentId
    {
        get => _filterDepartmentId;
        set
        {
            if (SetProperty(ref _filterDepartmentId, value))
                LoadStudents();
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
    
    public string FormEmail
    {
        get => _formEmail;
        set
        {
            if (SetProperty(ref _formEmail, value))
                ValidateEmail();
        }
    }
    
    public string FormStudentId
    {
        get => _formStudentId;
        set
        {
            if (SetProperty(ref _formStudentId, value))
                ValidateStudentId();
        }
    }
    
    public string FormPhone
    {
        get => _formPhone;
        set
        {
            if (SetProperty(ref _formPhone, value))
                ValidatePhone();
        }
    }
    
    public int FormYearLevel
    {
        get => _formYearLevel;
        set => SetProperty(ref _formYearLevel, value);
    }
    
    public int? FormSectionId
    {
        get => _formSectionId;
        set => SetProperty(ref _formSectionId, value);
    }
    
    public int? FormDepartmentId
    {
        get => _formDepartmentId;
        set
        {
            if (SetProperty(ref _formDepartmentId, value))
                ValidateDepartment();
        }
    }
    
    public bool FormIsActive
    {
        get => _formIsActive;
        set => SetProperty(ref _formIsActive, value);
    }
    
    // Validation error properties
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
    
    public string StudentIdError
    {
        get => _studentIdError;
        set => SetProperty(ref _studentIdError, value);
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
    
    public bool HasErrors => !string.IsNullOrEmpty(NameError) || 
                             !string.IsNullOrEmpty(EmailError) || 
                             !string.IsNullOrEmpty(StudentIdError) ||
                             !string.IsNullOrEmpty(PhoneError) ||
                             !string.IsNullOrEmpty(DepartmentError);
    
    public int[] YearLevels => new[] { 1, 2, 3, 4 };
    
    // Commands
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearFilterCommand { get; }
    
    public StudentsViewModel()
    {
        AddCommand = new RelayCommand(StartAdd);
        EditCommand = new RelayCommand(StartEdit, () => SelectedStudent != null);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete, () => SelectedStudent != null);
        ClearFilterCommand = new RelayCommand(ClearFilter);
        
        LoadDepartments();
        LoadStudents();
        LoadSections();
    }
    
    private void ValidateName()
    {
        NameError = ValidationService.IsNotEmpty(FormName) ? string.Empty : "Name is required.";
    }
    
    private void ValidateEmail()
    {
        EmailError = ValidationService.GetEmailError(FormEmail);
    }
    
    private void ValidateStudentId()
    {
        StudentIdError = ValidationService.GetStudentIdError(FormStudentId);
    }
    
    private void ValidatePhone()
    {
        PhoneError = ValidationService.GetPhoneError(FormPhone);
    }
    
    private void ValidateDepartment()
    {
        DepartmentError = FormDepartmentId.HasValue ? string.Empty : "Please select a department.";
    }
    
    private bool ValidateAll()
    {
        ValidateName();
        ValidateEmail();
        ValidateStudentId();
        ValidatePhone();
        ValidateDepartment();
        
        OnPropertyChanged(nameof(HasErrors));
        return !HasErrors;
    }
    
    private void ClearValidationErrors()
    {
        NameError = string.Empty;
        EmailError = string.Empty;
        StudentIdError = string.Empty;
        PhoneError = string.Empty;
        DepartmentError = string.Empty;
    }
    
    private void LoadStudents()
    {
        try
        {
            using var context = new UniversityDbContext();
            var query = context.Students
                .Include(s => s.Section)
                .Include(s => s.Department)
                .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(search) || 
                                     s.Email.ToLower().Contains(search) ||
                                     (s.StudentId != null && s.StudentId.ToLower().Contains(search)));
        }
        
        if (FilterYearLevel.HasValue)
        {
            query = query.Where(s => s.YearLevel == FilterYearLevel.Value);
        }
        
        if (FilterDepartmentId.HasValue)
        {
            query = query.Where(s => s.DepartmentId == FilterDepartmentId.Value);
        }
        
            Students = new ObservableCollection<Student>(query.OrderBy(s => s.Name).ToList());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading students: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void LoadSections()
    {
        using var context = new UniversityDbContext();
        Sections = new ObservableCollection<Section>(
            context.Sections.Include(s => s.Course).OrderBy(s => s.Name).ToList()
        );
    }
    
    private void LoadDepartments()
    {
        using var context = new UniversityDbContext();
        Departments = new ObservableCollection<Department>(
            context.Departments.OrderBy(d => d.Name).ToList()
        );
    }
    
    private void LoadStudentToForm(Student student)
    {
        FormName = student.Name;
        FormEmail = student.Email;
        FormStudentId = student.StudentId ?? string.Empty;
        FormPhone = student.Phone ?? string.Empty;
        FormYearLevel = student.YearLevel;
        FormSectionId = student.SectionId;
        FormDepartmentId = student.DepartmentId;
        FormIsActive = student.IsActive;
    }
    
    private void ClearForm()
    {
        FormName = string.Empty;
        FormEmail = string.Empty;
        FormStudentId = string.Empty;
        FormPhone = string.Empty;
        FormYearLevel = 1;
        FormSectionId = null;
        FormDepartmentId = Departments.FirstOrDefault()?.Id;
        FormIsActive = true;
        ClearValidationErrors();
    }
    
    private void StartAdd()
    {
        ClearForm();
        IsAdding = true;
        IsEditing = true;
        SelectedStudent = null;
    }
    
    private void StartEdit()
    {
        if (SelectedStudent != null)
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
        
        // Check for duplicate email
        var existingEmail = context.Students.FirstOrDefault(s => 
            s.Email.ToLower() == FormEmail.ToLower() && 
            (IsAdding || s.Id != SelectedStudent!.Id));
        
        if (existingEmail != null)
        {
            EmailError = "This email is already registered.";
            MessageBox.Show("This email is already registered to another student.", "Duplicate Email", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        if (IsAdding)
        {
            var student = new Student
            {
                Name = FormName.Trim(),
                Email = FormEmail.Trim().ToLower(),
                StudentId = string.IsNullOrWhiteSpace(FormStudentId) ? GenerateStudentId() : FormStudentId.Trim().ToUpper(),
                Phone = string.IsNullOrWhiteSpace(FormPhone) ? null : FormPhone.Trim(),
                YearLevel = FormYearLevel,
                SectionId = FormSectionId,
                DepartmentId = FormDepartmentId,
                IsActive = FormIsActive,
                EnrollmentDate = DateTime.Now
            };
            context.Students.Add(student);
            context.SaveChanges();
            
            // Auto-generate fees for the student based on department
            var department = context.Departments.Find(FormDepartmentId);
            if (department != null)
            {
                for (int year = 1; year <= FormYearLevel; year++)
                {
                    context.StudentFees.Add(new StudentFee
                    {
                        StudentId = student.Id,
                        AcademicYear = year,
                        Amount = department.AnnualFees,
                        AmountPaid = 0,
                        Status = PaymentStatus.Pending,
                        AssignedDate = DateTime.Now
                    });
                }
                context.SaveChanges();
            }
            
            MessageBox.Show($"Student '{FormName}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else if (SelectedStudent != null)
        {
            var student = context.Students.Find(SelectedStudent.Id);
            if (student != null)
            {
                var oldDepartmentId = student.DepartmentId;
                
                student.Name = FormName.Trim();
                student.Email = FormEmail.Trim().ToLower();
                student.StudentId = string.IsNullOrWhiteSpace(FormStudentId) ? student.StudentId : FormStudentId.Trim().ToUpper();
                student.Phone = string.IsNullOrWhiteSpace(FormPhone) ? null : FormPhone.Trim();
                student.YearLevel = FormYearLevel;
                student.SectionId = FormSectionId;
                student.DepartmentId = FormDepartmentId;
                student.IsActive = FormIsActive;
                
                // If department changed, update pending fees
                if (oldDepartmentId != FormDepartmentId && FormDepartmentId.HasValue)
                {
                    var newDept = context.Departments.Find(FormDepartmentId);
                    if (newDept != null)
                    {
                        var pendingFees = context.StudentFees
                            .Where(f => f.StudentId == student.Id && f.Status != PaymentStatus.Paid)
                            .ToList();
                        foreach (var fee in pendingFees)
                        {
                            fee.Amount = newDept.AnnualFees;
                        }
                    }
                }
                
                context.SaveChanges();
                MessageBox.Show($"Student '{FormName}' updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        IsEditing = false;
        IsAdding = false;
        LoadStudents();
    }
    
    private string GenerateStudentId()
    {
        using var context = new UniversityDbContext();
        var maxId = context.Students
            .Where(s => s.StudentId != null && s.StudentId.StartsWith("NCT"))
            .Select(s => s.StudentId)
            .ToList()
            .Select(id => int.TryParse(id?.Substring(3), out var num) ? num : 0)
            .DefaultIfEmpty(2024000)
            .Max();
        return $"NCT{maxId + 1:D5}";
    }
    
    private void Cancel()
    {
        IsEditing = false;
        IsAdding = false;
        ClearValidationErrors();
        if (SelectedStudent != null)
            LoadStudentToForm(SelectedStudent);
        else
            ClearForm();
    }
    
    private void Delete()
    {
        if (SelectedStudent == null) return;
        
        var result = MessageBox.Show($"Are you sure you want to delete {SelectedStudent.Name}?\n\nThis will also delete all their fees and grades.", 
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        
        if (result == MessageBoxResult.Yes)
        {
            using var context = new UniversityDbContext();
            var student = context.Students.Find(SelectedStudent.Id);
            if (student != null)
            {
                context.Students.Remove(student);
                context.SaveChanges();
                MessageBox.Show("Student deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadStudents();
            ClearForm();
        }
    }
    
    private void ClearFilter()
    {
        SearchText = string.Empty;
        FilterYearLevel = null;
        FilterDepartmentId = null;
    }
}
