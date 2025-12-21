using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.Data;
using UniversityManagementSystem.Models;

namespace UniversityManagementSystem.Services;

public class ReportService
{
    private readonly UniversityDbContext _context;
    
    public ReportService(UniversityDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Gets overall statistics for the dashboard
    /// </summary>
    public DashboardStats GetDashboardStats()
    {
        return new DashboardStats
        {
            TotalStudents = _context.Students.Count(),
            ActiveStudents = _context.Students.Count(s => s.IsActive),
            TotalCourses = _context.Courses.Count(),
            TotalSections = _context.Sections.Count(),
            TotalDepartments = _context.Departments.Count(),
            GradedStudents = _context.Grades.Where(g => g.TotalScore.HasValue).Select(g => g.StudentId).Distinct().Count()
        };
    }
    
    /// <summary>
    /// Gets grade distribution statistics
    /// Maps new PDF grade names (Excellent, Very Good, Good, Pass, Fail) to display categories
    /// </summary>
    public GradeDistribution GetGradeDistribution()
    {
        var grades = _context.Grades.Where(g => g.SymbolicGrade != null).ToList();
        
        return new GradeDistribution
        {
            // Map new PDF grades to display categories:
            // Excellent (≥85%) = Distinction
            // Very Good (≥75%) = Merit
            // Good (≥65%) = Pass
            // Pass (≥60%) = Pass
            // Fail (<60%) = Not Achieved
            DistinctionCount = grades.Count(g => g.SymbolicGrade == "Excellent" || g.SymbolicGrade == "D"),
            MeritCount = grades.Count(g => g.SymbolicGrade == "Very Good" || g.SymbolicGrade == "M"),
            PassCount = grades.Count(g => g.SymbolicGrade == "Good" || g.SymbolicGrade == "Pass" || g.SymbolicGrade == "P"),
            FailCount = grades.Count(g => g.SymbolicGrade == "Fail" || g.SymbolicGrade == "NA"),
            TotalGraded = grades.Count
        };
    }
    
    /// <summary>
    /// Gets student count by year level
    /// </summary>
    public Dictionary<int, int> GetStudentsByYear()
    {
        return _context.Students
            .GroupBy(s => s.YearLevel)
            .ToDictionary(g => g.Key, g => g.Count());
    }
    
    /// <summary>
    /// Gets average scores by course
    /// </summary>
    public List<CoursePerformance> GetCoursePerformance()
    {
        return _context.Grades
            .Include(g => g.Course)
            .Where(g => g.TotalScore.HasValue && g.Course != null)
            .GroupBy(g => new { g.CourseId, g.Course!.Name, g.Course.Code })
            .Select(g => new CoursePerformance
            {
                CourseId = g.Key.CourseId,
                CourseName = g.Key.Name,
                CourseCode = g.Key.Code,
                AverageScore = g.Average(x => x.TotalScore!.Value),
                StudentCount = g.Count(),
                PassRate = (double)g.Count(x => x.SymbolicGrade != "NA" && x.SymbolicGrade != "Fail") / g.Count() * 100
            })
            .OrderByDescending(c => c.AverageScore)
            .ToList();
    }
    
    /// <summary>
    /// Gets section enrollment statistics
    /// </summary>
    public List<SectionEnrollment> GetSectionEnrollments()
    {
        return _context.Sections
            .Include(s => s.Course)
            .Include(s => s.Students)
            .Select(s => new SectionEnrollment
            {
                SectionId = s.Id,
                SectionName = s.Name,
                CourseName = s.Course != null ? s.Course.Name : "Unknown",
                Capacity = s.Capacity,
                Enrolled = s.Students.Count,
                UtilizationRate = s.Capacity > 0 ? (double)s.Students.Count / s.Capacity * 100 : 0
            })
            .OrderByDescending(s => s.UtilizationRate)
            .ToList();
    }
    
    /// <summary>
    /// Gets top performing students
    /// </summary>
    public List<StudentPerformance> GetTopStudents(int count = 10)
    {
        return _context.Students
            .Include(s => s.Grades)
            .Where(s => s.Grades.Any(g => g.TotalScore.HasValue))
            .Select(s => new StudentPerformance
            {
                StudentId = s.Id,
                StudentName = s.Name,
                YearLevel = s.YearLevel,
                AverageScore = s.Grades.Where(g => g.TotalScore.HasValue).Average(g => g.TotalScore!.Value),
                CoursesCompleted = s.Grades.Count(g => g.TotalScore.HasValue)
            })
            .OrderByDescending(s => s.AverageScore)
            .Take(count)
            .ToList();
    }
    
    /// <summary>
    /// Gets leniency application statistics
    /// </summary>
    public LeniencyStats GetLeniencyStats()
    {
        var grades = _context.Grades.Where(g => g.TotalScore.HasValue).ToList();
        
        return new LeniencyStats
        {
            TotalGraded = grades.Count,
            LeniencyApplied = grades.Count(g => g.LeniencyApplied),
            LeniencyRate = grades.Count > 0 ? (double)grades.Count(g => g.LeniencyApplied) / grades.Count * 100 : 0
        };
    }
}

// DTOs for reports
public class DashboardStats
{
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int TotalCourses { get; set; }
    public int TotalSections { get; set; }
    public int TotalDepartments { get; set; }
    public int GradedStudents { get; set; }
}

public class GradeDistribution
{
    public int DistinctionCount { get; set; }
    public int MeritCount { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public int TotalGraded { get; set; }
    
    public double DistinctionPercentage => TotalGraded > 0 ? (double)DistinctionCount / TotalGraded * 100 : 0;
    public double MeritPercentage => TotalGraded > 0 ? (double)MeritCount / TotalGraded * 100 : 0;
    public double PassPercentage => TotalGraded > 0 ? (double)PassCount / TotalGraded * 100 : 0;
    public double FailPercentage => TotalGraded > 0 ? (double)FailCount / TotalGraded * 100 : 0;
}

public class CoursePerformance
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public double AverageScore { get; set; }
    public int StudentCount { get; set; }
    public double PassRate { get; set; }
}

public class SectionEnrollment
{
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int Enrolled { get; set; }
    public double UtilizationRate { get; set; }
}

public class StudentPerformance
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int YearLevel { get; set; }
    public double AverageScore { get; set; }
    public int CoursesCompleted { get; set; }
}

public class LeniencyStats
{
    public int TotalGraded { get; set; }
    public int LeniencyApplied { get; set; }
    public double LeniencyRate { get; set; }
}


