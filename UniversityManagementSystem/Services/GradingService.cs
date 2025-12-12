using UniversityManagementSystem.Models;

namespace UniversityManagementSystem.Services;

public class GradingService
{
    // Grade thresholds
    private const double DistinctionThreshold = 85.0;
    private const double MeritThreshold = 70.0;
    private const double PassThreshold = 50.0;
    private const double LeniencyMargin = 2.0;
    
    // Weight percentages
    private const double Assignment1Weight = 0.20;
    private const double Assignment2Weight = 0.20;
    private const double FinalExamWeight = 0.60;
    
    /// <summary>
    /// Calculates the total score based on weighted components
    /// </summary>
    public double CalculateTotalScore(double? assignment1, double? assignment2, double? finalExam)
    {
        if (!assignment1.HasValue || !assignment2.HasValue || !finalExam.HasValue)
            return 0;
        
        return (assignment1.Value * Assignment1Weight) +
               (assignment2.Value * Assignment2Weight) +
               (finalExam.Value * FinalExamWeight);
    }
    
    /// <summary>
    /// Determines the symbolic grade based on total score
    /// </summary>
    public string GetSymbolicGrade(double totalScore)
    {
        return totalScore switch
        {
            >= DistinctionThreshold => "D",  // Distinction
            >= MeritThreshold => "M",         // Merit
            >= PassThreshold => "P",          // Pass
            _ => "NA"                         // Not Achieved
        };
    }
    
    /// <summary>
    /// Checks if leniency can be applied based on improvement trend
    /// </summary>
    public bool CanApplyLeniency(double totalScore, double? assignment1, double? assignment2)
    {
        if (!assignment1.HasValue || !assignment2.HasValue)
            return false;
        
        // Check if student shows improvement (Assignment2 > Assignment1)
        bool showsImprovement = assignment2.Value > assignment1.Value;
        
        if (!showsImprovement)
            return false;
        
        // Check if within leniency margin of next grade boundary
        bool nearDistinction = totalScore >= (DistinctionThreshold - LeniencyMargin) && totalScore < DistinctionThreshold;
        bool nearMerit = totalScore >= (MeritThreshold - LeniencyMargin) && totalScore < MeritThreshold;
        bool nearPass = totalScore >= (PassThreshold - LeniencyMargin) && totalScore < PassThreshold;
        
        return nearDistinction || nearMerit || nearPass;
    }
    
    /// <summary>
    /// Applies leniency and returns the upgraded grade
    /// </summary>
    public string ApplyLeniency(double totalScore)
    {
        if (totalScore >= (DistinctionThreshold - LeniencyMargin) && totalScore < DistinctionThreshold)
            return "D";
        if (totalScore >= (MeritThreshold - LeniencyMargin) && totalScore < MeritThreshold)
            return "M";
        if (totalScore >= (PassThreshold - LeniencyMargin) && totalScore < PassThreshold)
            return "P";
        
        return GetSymbolicGrade(totalScore);
    }
    
    /// <summary>
    /// Calculates and updates a grade record with all computed values
    /// </summary>
    public void CalculateGrade(Grade grade)
    {
        if (!grade.Assignment1.HasValue || !grade.Assignment2.HasValue || !grade.FinalExam.HasValue)
        {
            grade.TotalScore = null;
            grade.SymbolicGrade = null;
            grade.LeniencyApplied = false;
            return;
        }
        
        grade.TotalScore = CalculateTotalScore(grade.Assignment1, grade.Assignment2, grade.FinalExam);
        
        // Check for leniency
        if (CanApplyLeniency(grade.TotalScore.Value, grade.Assignment1, grade.Assignment2))
        {
            grade.SymbolicGrade = ApplyLeniency(grade.TotalScore.Value);
            grade.LeniencyApplied = true;
        }
        else
        {
            grade.SymbolicGrade = GetSymbolicGrade(grade.TotalScore.Value);
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
            "D" => "#4ADE80",  // Green - Distinction
            "M" => "#60A5FA",  // Blue - Merit
            "P" => "#FBBF24",  // Yellow - Pass
            "NA" => "#F87171", // Red - Not Achieved
            _ => "#A0A0A0"     // Gray - Unknown
        };
    }
    
    /// <summary>
    /// Gets the full grade name
    /// </summary>
    public static string GetGradeName(string? symbolicGrade)
    {
        return symbolicGrade switch
        {
            "D" => "Distinction",
            "M" => "Merit",
            "P" => "Pass",
            "NA" => "Not Achieved",
            _ => "Not Graded"
        };
    }
}


