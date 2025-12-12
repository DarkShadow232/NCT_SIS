using System.Collections.ObjectModel;
using System.Windows.Input;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Services;

namespace UniversityManagementSystem.ViewModels;

public class ReportsViewModel : BaseViewModel
{
    private GradeDistribution? _gradeDistribution;
    private ObservableCollection<CoursePerformance> _coursePerformances = new();
    private ObservableCollection<StudentPerformance> _topStudents = new();
    private ObservableCollection<SectionEnrollment> _sectionEnrollments = new();
    private LeniencyStats? _leniencyStats;
    private Dictionary<int, int> _studentsByYear = new();
    
    public GradeDistribution? GradeDistribution
    {
        get => _gradeDistribution;
        set => SetProperty(ref _gradeDistribution, value);
    }
    
    public ObservableCollection<CoursePerformance> CoursePerformances
    {
        get => _coursePerformances;
        set => SetProperty(ref _coursePerformances, value);
    }
    
    public ObservableCollection<StudentPerformance> TopStudents
    {
        get => _topStudents;
        set => SetProperty(ref _topStudents, value);
    }
    
    public ObservableCollection<SectionEnrollment> SectionEnrollments
    {
        get => _sectionEnrollments;
        set => SetProperty(ref _sectionEnrollments, value);
    }
    
    public LeniencyStats? LeniencyStats
    {
        get => _leniencyStats;
        set => SetProperty(ref _leniencyStats, value);
    }
    
    public Dictionary<int, int> StudentsByYear
    {
        get => _studentsByYear;
        set => SetProperty(ref _studentsByYear, value);
    }
    
    // Computed properties for chart data
    public int Year1Students => StudentsByYear.GetValueOrDefault(1, 0);
    public int Year2Students => StudentsByYear.GetValueOrDefault(2, 0);
    public int Year3Students => StudentsByYear.GetValueOrDefault(3, 0);
    public int Year4Students => StudentsByYear.GetValueOrDefault(4, 0);
    
    public ICommand RefreshCommand { get; }
    
    public ReportsViewModel()
    {
        RefreshCommand = new RelayCommand(LoadAllReports);
        LoadAllReports();
    }
    
    private void LoadAllReports()
    {
        using var context = new UniversityDbContext();
        var reportService = new ReportService(context);
        
        GradeDistribution = reportService.GetGradeDistribution();
        CoursePerformances = new ObservableCollection<CoursePerformance>(reportService.GetCoursePerformance());
        TopStudents = new ObservableCollection<StudentPerformance>(reportService.GetTopStudents(10));
        SectionEnrollments = new ObservableCollection<SectionEnrollment>(reportService.GetSectionEnrollments());
        LeniencyStats = reportService.GetLeniencyStats();
        StudentsByYear = reportService.GetStudentsByYear();
        
        // Notify computed properties changed
        OnPropertyChanged(nameof(Year1Students));
        OnPropertyChanged(nameof(Year2Students));
        OnPropertyChanged(nameof(Year3Students));
        OnPropertyChanged(nameof(Year4Students));
    }
}


