using UniversityManagementSystem.Models;

namespace UniversityManagementSystem.Services;

public class GradingService
{
    // Grade thresholds based on PDF requirements (percentage-based)
    private const double ExcellentThreshold = 85.0;     // ≥85%
    private const double VeryGoodThreshold = 75.0;      // ≥75%
    private const double GoodThreshold = 65.0;          // ≥65%
    private const double PassThreshold = 60.0;          // ≥60%
    private const double LeniencyMargin = 2.0;
    
    /// <summary>
    /// Calculates the total score based on weighted components and course type
    /// Based on PDF Grade Distribution table
    /// </summary>
    public double CalculateTotalScore(Grade grade)
    {
        if (grade.Course == null)
            return 0;
        
        var courseType = grade.Course.CourseType;
        
        // Theoretical courses (out of 100): Ass1 20%, Ass2 20%, CW 60%, NO Final
        if (courseType == CourseType.Theoretical100)
        {
            if (!grade.Assignment1.HasValue || !grade.Assignment2.HasValue || !grade.CourseWork.HasValue)
                return 0;
            
            return (grade.Assignment1.Value * 0.20) +
                   (grade.Assignment2.Value * 0.20) +
                   (grade.CourseWork.Value * 0.60);
        }
        
        // Practical courses (out of 150 or 100): Ass1 20%, Ass2 30%, CW 20%, Final 30%
        if (!grade.Assignment1.HasValue || !grade.Assignment2.HasValue || 
            !grade.CourseWork.HasValue || !grade.FinalExam.HasValue)
            return 0;
        
        return (grade.Assignment1.Value * 0.20) +
               (grade.Assignment2.Value * 0.30) +
               (grade.CourseWork.Value * 0.20) +
               (grade.FinalExam.Value * 0.30);
    }
    
    /// <summary>
    /// Determines the symbolic grade based on total score and course max degree
    /// Based on PDF Grade Evaluation: ≥85% Excellent, ≥75% Very Good, ≥65% Good, ≥60% Pass
    /// </summary>
    public string GetSymbolicGrade(double totalScore, int maxDegree)
    {
        double percentage = (totalScore / maxDegree) * 100.0;
        
        return percentage switch
        {
            >= ExcellentThreshold => "Excellent",
            >= VeryGoodThreshold => "Very Good",
            >= GoodThreshold => "Good",
            >= PassThreshold => "Pass",
            _ => "Fail"
        };
    }
    
    /// <summary>
    /// Checks if leniency can be applied based on improvement trend
    /// </summary>
    public bool CanApplyLeniency(double percentage, double? assignment1, double? assignment2)
    {
        if (!assignment1.HasValue || !assignment2.HasValue)
            return false;
        
        // Check if student shows improvement (Assignment2 > Assignment1)
        bool showsImprovement = assignment2.Value > assignment1.Value;
        
        if (!showsImprovement)
            return false;
        
        // Check if within leniency margin of next grade boundary
        bool nearExcellent = percentage >= (ExcellentThreshold - LeniencyMargin) && percentage < ExcellentThreshold;
        bool nearVeryGood = percentage >= (VeryGoodThreshold - LeniencyMargin) && percentage < VeryGoodThreshold;
        bool nearGood = percentage >= (GoodThreshold - LeniencyMargin) && percentage < GoodThreshold;
        bool nearPass = percentage >= (PassThreshold - LeniencyMargin) && percentage < PassThreshold;
        
        return nearExcellent || nearVeryGood || nearGood || nearPass;
    }
    
    /// <summary>
    /// Applies leniency and returns the upgraded grade
    /// </summary>
    public string ApplyLeniency(double percentage)
    {
        if (percentage >= (ExcellentThreshold - LeniencyMargin) && percentage < ExcellentThreshold)
            return "Excellent";
        if (percentage >= (VeryGoodThreshold - LeniencyMargin) && percentage < VeryGoodThreshold)
            return "Very Good";
        if (percentage >= (GoodThreshold - LeniencyMargin) && percentage < GoodThreshold)
            return "Good";
        if (percentage >= (PassThreshold - LeniencyMargin) && percentage < PassThreshold)
            return "Pass";
        
        return "Fail";
    }
    
    /// <summary>
    /// Calculates and updates a grade record with all computed values
    /// </summary>
    public void CalculateGrade(Grade grade)
    {
        if (grade.Course == null || !grade.IsComplete)
        {
            grade.TotalScore = null;
            grade.SymbolicGrade = null;
            grade.LeniencyApplied = false;
            return;
        }
        
        grade.TotalScore = CalculateTotalScore(grade);
        
        int maxDegree = grade.Course.MaxDegree;
        double percentage = (grade.TotalScore.Value / maxDegree) * 100.0;
        
        // Check for leniency
        if (CanApplyLeniency(percentage, grade.Assignment1, grade.Assignment2))
        {
            grade.SymbolicGrade = ApplyLeniency(percentage);
            grade.LeniencyApplied = true;
        }
        else
        {
            grade.SymbolicGrade = GetSymbolicGrade(grade.TotalScore.Value, maxDegree);
            grade.LeniencyApplied = false;
        }
        
        grade.GradedDate = DateTime.Now;
    }
    
    /// <summary>
    /// Gets the grade color based on symbolic grade
    /// </summary>
    public static string GetGradeColor(string? symbolicGrade)
    {
        return symbolicGrade switch
        {
            "Excellent" => "#10B981",   // Green - Excellent
            "Very Good" => "#3B82F6",   // Blue - Very Good
            "Good" => "#F59E0B",        // Orange - Good
            "Pass" => "#FBBF24",        // Yellow - Pass
            "Fail" => "#EF4444",        // Red - Fail
            _ => "#6B7280"              // Gray - Unknown
        };
    }
    
    /// <summary>
    /// Gets the full grade name (already using full names)
    /// </summary>
    public static string GetGradeName(string? symbolicGrade)
    {
        return symbolicGrade ?? "Not Graded";
    }
    
    /// <summary>
    /// Calculates GPA based on grades (4.0 scale)
    /// </summary>
    public static double CalculateGPA(IEnumerable<Grade> grades)
    {
        var completedGrades = grades.Where(g => g.IsComplete && g.SymbolicGrade != null).ToList();
        
        if (!completedGrades.Any())
            return 0.0;
        
        double totalPoints = 0;
        int totalCourses = completedGrades.Count;
        
        foreach (var grade in completedGrades)
        {
            totalPoints += grade.SymbolicGrade switch
            {
                "Excellent" => 4.0,
                "Very Good" => 3.5,
                "Good" => 3.0,
                "Pass" => 2.5,
                _ => 0.0
            };
        }
        
        return totalPoints / totalCourses;
    }
}


