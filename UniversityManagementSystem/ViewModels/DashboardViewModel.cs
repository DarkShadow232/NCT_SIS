using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private int _totalStudents;
    private int _activeStudents;
    private int _totalCourses;
    private int _totalSections;
    private int _totalDepartments;
    private int _gradedStudents;
    private int _distinctionCount;
    private int _meritCount;
    private int _passCount;
    private int _failCount;
    private ObservableCollection<ISeries> _gradeSeries = new();
    
    public int TotalStudents
    {
        get => _totalStudents;
        set => SetProperty(ref _totalStudents, value);
    }
    
    public int ActiveStudents
    {
        get => _activeStudents;
        set => SetProperty(ref _activeStudents, value);
    }
    
    public int TotalCourses
    {
        get => _totalCourses;
        set => SetProperty(ref _totalCourses, value);
    }
    
    public int TotalSections
    {
        get => _totalSections;
        set => SetProperty(ref _totalSections, value);
    }
    
    public int TotalDepartments
    {
        get => _totalDepartments;
        set => SetProperty(ref _totalDepartments, value);
    }
    
    public int GradedStudents
    {
        get => _gradedStudents;
        set => SetProperty(ref _gradedStudents, value);
    }
    
    public int DistinctionCount
    {
        get => _distinctionCount;
        set => SetProperty(ref _distinctionCount, value);
    }
    
    public int MeritCount
    {
        get => _meritCount;
        set => SetProperty(ref _meritCount, value);
    }
    
    public int PassCount
    {
        get => _passCount;
        set => SetProperty(ref _passCount, value);
    }
    
    public int FailCount
    {
        get => _failCount;
        set => SetProperty(ref _failCount, value);
    }
    
    public ObservableCollection<ISeries> GradeSeries
    {
        get => _gradeSeries;
        set => SetProperty(ref _gradeSeries, value);
    }
    
    public DashboardViewModel()
    {
        LoadStatistics();
        CreatePieChart();
    }
    
    private void LoadStatistics()
    {
        using var context = new UniversityDbContext();
        var reportService = new ReportService(context);
        
        var stats = reportService.GetDashboardStats();
        TotalStudents = stats.TotalStudents;
        ActiveStudents = stats.ActiveStudents;
        TotalCourses = stats.TotalCourses;
        TotalSections = stats.TotalSections;
        TotalDepartments = stats.TotalDepartments;
        GradedStudents = stats.GradedStudents;
        
        var gradeDistribution = reportService.GetGradeDistribution();
        DistinctionCount = gradeDistribution.DistinctionCount;
        MeritCount = gradeDistribution.MeritCount;
        PassCount = gradeDistribution.PassCount;
        FailCount = gradeDistribution.FailCount;
    }
    
    private void CreatePieChart()
    {
        GradeSeries = new ObservableCollection<ISeries>
        {
            new PieSeries<int>
            {
                Values = new[] { DistinctionCount },
                Name = "Distinction (D)",
                Fill = new SolidColorPaint(SKColor.Parse("#10B981")),
                Stroke = null,
                Pushout = 5,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#112250")),
                DataLabelsSize = 14,
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue}"
            },
            new PieSeries<int>
            {
                Values = new[] { MeritCount },
                Name = "Merit (M)",
                Fill = new SolidColorPaint(SKColor.Parse("#3B82F6")),
                Stroke = null,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#112250")),
                DataLabelsSize = 14,
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue}"
            },
            new PieSeries<int>
            {
                Values = new[] { PassCount },
                Name = "Pass (P)",
                Fill = new SolidColorPaint(SKColor.Parse("#F59E0B")),
                Stroke = null,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#112250")),
                DataLabelsSize = 14,
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue}"
            },
            new PieSeries<int>
            {
                Values = new[] { FailCount },
                Name = "Not Achieved (NA)",
                Fill = new SolidColorPaint(SKColor.Parse("#EF4444")),
                Stroke = null,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#112250")),
                DataLabelsSize = 14,
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue}"
            }
        };
    }
}
