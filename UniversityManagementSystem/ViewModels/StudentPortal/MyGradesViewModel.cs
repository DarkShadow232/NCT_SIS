using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels.StudentPortal;

public class MyGradesViewModel : BaseViewModel
{
    private ObservableCollection<Grade> _grades = new();
    private string _searchText = string.Empty;
    private int? _filterYearLevel;
    private string? _filterSymbolicGrade;
    private int _totalCourses;
    private int _completedCourses;
    private decimal _averageScore;
    
    public ObservableCollection<Grade> Grades
    {
        get => _grades;
        set => SetProperty(ref _grades, value);
    }
    
    public string SearchText
    {
        get => _searchText;
        set { SetProperty(ref _searchText, value); FilterGrades(); }
    }
    
    public int? FilterYearLevel
    {
        get => _filterYearLevel;
        set { SetProperty(ref _filterYearLevel, value); FilterGrades(); }
    }
    
    public string? FilterSymbolicGrade
    {
        get => _filterSymbolicGrade;
        set { SetProperty(ref _filterSymbolicGrade, value); FilterGrades(); }
    }
    
    public int TotalCourses
    {
        get => _totalCourses;
        set => SetProperty(ref _totalCourses, value);
    }
    
    public int CompletedCourses
    {
        get => _completedCourses;
        set => SetProperty(ref _completedCourses, value);
    }
    
    public decimal AverageScore
    {
        get => _averageScore;
        set => SetProperty(ref _averageScore, value);
    }
    
    public List<int?> YearLevelOptions { get; } = new() { null, 1, 2, 3, 4 };
    public List<string?> SymbolicGradeOptions { get; } = new() { null, "D", "M", "P", "NA" };
    
    private List<Grade> _allGrades = new();
    
    public MyGradesViewModel()
    {
        LoadGrades();
    }
    
    private void LoadGrades()
    {
        var user = AuthenticationService.Instance.CurrentUser;
        if (user?.StudentId == null) return;
        
        using var context = new UniversityDbContext();
        _allGrades = context.Grades
            .Include(g => g.Course)
            .Where(g => g.StudentId == user.StudentId)
            .OrderBy(g => g.Course!.YearLevel)
            .ThenBy(g => g.Course!.Name)
            .ToList();
        
        TotalCourses = _allGrades.Count;
        CompletedCourses = _allGrades.Count(g => g.SymbolicGrade != "NA");
        
        var completed = _allGrades.Where(g => g.SymbolicGrade != "NA" && g.TotalScore.HasValue).ToList();
        AverageScore = completed.Any() ? (decimal)completed.Average(g => g.TotalScore ?? 0) : 0;
        
        FilterGrades();
    }
    
    private void FilterGrades()
    {
        var filtered = _allGrades.AsEnumerable();
        
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(g => 
                g.Course!.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                g.Course.Code.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }
        
        if (FilterYearLevel.HasValue)
        {
            filtered = filtered.Where(g => g.Course!.YearLevel == FilterYearLevel.Value);
        }
        
        if (!string.IsNullOrEmpty(FilterSymbolicGrade))
        {
            filtered = filtered.Where(g => g.SymbolicGrade == FilterSymbolicGrade);
        }
        
        Grades = new ObservableCollection<Grade>(filtered);
    }
}

