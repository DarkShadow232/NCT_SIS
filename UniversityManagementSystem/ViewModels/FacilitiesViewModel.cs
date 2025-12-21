using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels;

public class FacilitiesViewModel : BaseViewModel
{
    private readonly FileExportService _fileExportService = new();
    
    private ObservableCollection<LectureHall> _lectureHalls = new();
    private ObservableCollection<Laboratory> _laboratories = new();
    private ObservableCollection<Department> _departments = new();
    private LectureHall? _selectedLectureHall;
    private Laboratory? _selectedLaboratory;
    private string _selectedFacilityType = "LectureHall";
    private bool _isEditing;
    
    // Form fields
    private string _code = string.Empty;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private int _capacity = 30;
    private int _numberOfSeats = 0;
    private int _numberOfAirConditioners = 0;
    private int _numberOfFans = 0;
    private int _numberOfLights = 0;
    private int _numberOfComputers = 0;
    private int? _departmentId;
    
    public ObservableCollection<LectureHall> LectureHalls
    {
        get => _lectureHalls;
        set => SetProperty(ref _lectureHalls, value);
    }
    
    public ObservableCollection<Laboratory> Laboratories
    {
        get => _laboratories;
        set => SetProperty(ref _laboratories, value);
    }
    
    public ObservableCollection<Department> Departments
    {
        get => _departments;
        set => SetProperty(ref _departments, value);
    }
    
    public LectureHall? SelectedLectureHall
    {
        get => _selectedLectureHall;
        set
        {
            if (SetProperty(ref _selectedLectureHall, value) && value != null)
            {
                LoadLectureHallToForm(value);
            }
        }
    }
    
    public Laboratory? SelectedLaboratory
    {
        get => _selectedLaboratory;
        set
        {
            if (SetProperty(ref _selectedLaboratory, value) && value != null)
            {
                LoadLaboratoryToForm(value);
            }
        }
    }
    
    public string SelectedFacilityType
    {
        get => _selectedFacilityType;
        set => SetProperty(ref _selectedFacilityType, value);
    }
    
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }
    
    public string Code
    {
        get => _code;
        set => SetProperty(ref _code, value);
    }
    
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }
    
    public int Capacity
    {
        get => _capacity;
        set => SetProperty(ref _capacity, value);
    }
    
    public int NumberOfSeats
    {
        get => _numberOfSeats;
        set => SetProperty(ref _numberOfSeats, value);
    }
    
    public int NumberOfAirConditioners
    {
        get => _numberOfAirConditioners;
        set => SetProperty(ref _numberOfAirConditioners, value);
    }
    
    public int NumberOfFans
    {
        get => _numberOfFans;
        set => SetProperty(ref _numberOfFans, value);
    }
    
    public int NumberOfLights
    {
        get => _numberOfLights;
        set => SetProperty(ref _numberOfLights, value);
    }
    
    public int NumberOfComputers
    {
        get => _numberOfComputers;
        set => SetProperty(ref _numberOfComputers, value);
    }
    
    public int? DepartmentId
    {
        get => _departmentId;
        set => SetProperty(ref _departmentId, value);
    }
    
    public ICommand AddCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteLectureHallCommand { get; }
    public ICommand DeleteLaboratoryCommand { get; }
    public ICommand ExportLectureHallCommand { get; }
    public ICommand ExportLaboratoryCommand { get; }
    public ICommand RefreshCommand { get; }
    
    public FacilitiesViewModel()
    {
        AddCommand = new RelayCommand(StartAdd);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteLectureHallCommand = new RelayCommand(DeleteLectureHall, () => SelectedLectureHall != null);
        DeleteLaboratoryCommand = new RelayCommand(DeleteLaboratory, () => SelectedLaboratory != null);
        ExportLectureHallCommand = new RelayCommand(ExportLectureHall, () => SelectedLectureHall != null);
        ExportLaboratoryCommand = new RelayCommand(ExportLaboratory, () => SelectedLaboratory != null);
        RefreshCommand = new RelayCommand(LoadData);
        
        LoadData();
    }
    
    private void LoadData()
    {
        LoadDepartments();
        LoadLectureHalls();
        LoadLaboratories();
    }
    
    private void LoadDepartments()
    {
        using var context = new UniversityDbContext();
        var departments = context.Departments.OrderBy(d => d.Name).ToList();
        Departments = new ObservableCollection<Department>(departments);
    }
    
    private void LoadLectureHalls()
    {
        using var context = new UniversityDbContext();
        var halls = context.LectureHalls
            .Include(h => h.Department)
            .OrderBy(h => h.Code)
            .ToList();
        LectureHalls = new ObservableCollection<LectureHall>(halls);
    }
    
    private void LoadLaboratories()
    {
        using var context = new UniversityDbContext();
        var labs = context.Laboratories
            .Include(l => l.Department)
            .OrderBy(l => l.Code)
            .ToList();
        Laboratories = new ObservableCollection<Laboratory>(labs);
    }
    
    private void StartAdd()
    {
        IsEditing = true;
        ClearForm();
    }
    
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(Code) || string.IsNullOrWhiteSpace(Name))
        {
            MessageBox.Show("Please enter code and name.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        using var context = new UniversityDbContext();
        
        if (SelectedFacilityType == "LectureHall")
        {
            var hall = new LectureHall
            {
                Code = Code,
                Name = Name,
                Description = Description,
                Capacity = Capacity,
                NumberOfSeats = NumberOfSeats,
                NumberOfAirConditioners = NumberOfAirConditioners,
                NumberOfFans = NumberOfFans,
                NumberOfLights = NumberOfLights,
                DepartmentId = DepartmentId
            };
            
            context.LectureHalls.Add(hall);
            context.SaveChanges();
            LoadLectureHalls();
            MessageBox.Show("Lecture Hall saved successfully!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            var lab = new Laboratory
            {
                Code = Code,
                Name = Name,
                Description = Description,
                Capacity = Capacity,
                NumberOfComputers = NumberOfComputers,
                NumberOfSeats = NumberOfSeats,
                NumberOfAirConditioners = NumberOfAirConditioners,
                NumberOfFans = NumberOfFans,
                NumberOfLights = NumberOfLights,
                DepartmentId = DepartmentId
            };
            
            context.Laboratories.Add(lab);
            context.SaveChanges();
            LoadLaboratories();
            MessageBox.Show("Laboratory saved successfully!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        IsEditing = false;
        ClearForm();
    }
    
    private void Cancel()
    {
        IsEditing = false;
        ClearForm();
    }
    
    private void DeleteLectureHall()
    {
        if (SelectedLectureHall == null) return;
        
        var result = MessageBox.Show(
            $"Are you sure you want to delete lecture hall '{SelectedLectureHall.Name}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        
        if (result != MessageBoxResult.Yes) return;
        
        using var context = new UniversityDbContext();
        var hall = context.LectureHalls.Find(SelectedLectureHall.Id);
        if (hall != null)
        {
            context.LectureHalls.Remove(hall);
            context.SaveChanges();
            LoadLectureHalls();
            MessageBox.Show("Lecture Hall deleted successfully!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    
    private void DeleteLaboratory()
    {
        if (SelectedLaboratory == null) return;
        
        var result = MessageBox.Show(
            $"Are you sure you want to delete laboratory '{SelectedLaboratory.Name}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        
        if (result != MessageBoxResult.Yes) return;
        
        using var context = new UniversityDbContext();
        var lab = context.Laboratories.Find(SelectedLaboratory.Id);
        if (lab != null)
        {
            context.Laboratories.Remove(lab);
            context.SaveChanges();
            LoadLaboratories();
            MessageBox.Show("Laboratory deleted successfully!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    
    private void ExportLectureHall()
    {
        if (SelectedLectureHall == null) return;
        
        try
        {
            var fileName = _fileExportService.ExportLectureHall(SelectedLectureHall);
            MessageBox.Show($"Lecture Hall exported successfully!\n\nFile: {fileName}", 
                "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting: {ex.Message}", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void ExportLaboratory()
    {
        if (SelectedLaboratory == null) return;
        
        try
        {
            var fileName = _fileExportService.ExportLaboratory(SelectedLaboratory);
            MessageBox.Show($"Laboratory exported successfully!\n\nFile: {fileName}", 
                "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting: {ex.Message}", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void LoadLectureHallToForm(LectureHall hall)
    {
        Code = hall.Code;
        Name = hall.Name;
        Description = hall.Description ?? string.Empty;
        Capacity = hall.Capacity;
        NumberOfSeats = hall.NumberOfSeats;
        NumberOfAirConditioners = hall.NumberOfAirConditioners;
        NumberOfFans = hall.NumberOfFans;
        NumberOfLights = hall.NumberOfLights;
        DepartmentId = hall.DepartmentId;
        SelectedFacilityType = "LectureHall";
    }
    
    private void LoadLaboratoryToForm(Laboratory lab)
    {
        Code = lab.Code;
        Name = lab.Name;
        Description = lab.Description ?? string.Empty;
        Capacity = lab.Capacity;
        NumberOfComputers = lab.NumberOfComputers;
        NumberOfSeats = lab.NumberOfSeats;
        NumberOfAirConditioners = lab.NumberOfAirConditioners;
        NumberOfFans = lab.NumberOfFans;
        NumberOfLights = lab.NumberOfLights;
        DepartmentId = lab.DepartmentId;
        SelectedFacilityType = "Laboratory";
    }
    
    private void ClearForm()
    {
        Code = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        Capacity = 30;
        NumberOfSeats = 0;
        NumberOfAirConditioners = 0;
        NumberOfFans = 0;
        NumberOfLights = 0;
        NumberOfComputers = 0;
        DepartmentId = null;
    }
}

