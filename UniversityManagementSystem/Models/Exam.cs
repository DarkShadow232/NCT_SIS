namespace UniversityManagementSystem.Models;

public enum ExamType
{
    Quiz,
    Midterm,
    Final,
    Practical,
    Assignment
}

public class Exam
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public string Name { get; set; } = string.Empty;
    public ExamType Type { get; set; } = ExamType.Quiz;
    public decimal MaxScore { get; set; } = 100;
    public decimal Weight { get; set; } = 10; // Percentage weight
    public DateTime ExamDate { get; set; }
    public string Room { get; set; } = string.Empty;
    public int DurationMinutes { get; set; } = 60;
    public bool IsActive { get; set; } = true;
    
    public string TypeDisplay => Type.ToString();
    public string DurationDisplay => $"{DurationMinutes} min";
}

