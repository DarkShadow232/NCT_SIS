using System.IO;
using System.Text;
using UniversityManagementSystem.Models;

namespace UniversityManagementSystem.Services;

/// <summary>
/// Service for exporting data to text files on D:\ as per PDF requirements
/// </summary>
public class FileExportService
{
    private readonly string _exportPath = @"D:\UniversityData";
    
    public FileExportService()
    {
        // Ensure export directory exists
        try
        {
            if (!Directory.Exists(_exportPath))
            {
                Directory.CreateDirectory(_exportPath);
            }
        }
        catch
        {
            // If D:\ is not available, use application directory
            _exportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExportedData");
            if (!Directory.Exists(_exportPath))
            {
                Directory.CreateDirectory(_exportPath);
            }
        }
    }
    
    /// <summary>
    /// Export student data to text file
    /// </summary>
    public string ExportStudent(Student student)
    {
        var fileName = Path.Combine(_exportPath, $"Student_{student.StudentId}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        var sb = new StringBuilder();
        
        sb.AppendLine("========================================");
        sb.AppendLine("          STUDENT INFORMATION");
        sb.AppendLine("========================================");
        sb.AppendLine($"Student ID: {student.StudentId}");
        sb.AppendLine($"Name: {student.Name}");
        sb.AppendLine($"Email: {student.Email}");
        sb.AppendLine($"Phone: {student.Phone}");
        sb.AppendLine($"Year Level: {student.YearLevel}");
        sb.AppendLine($"Department: {student.Department?.Name ?? "N/A"}");
        sb.AppendLine($"Section: {student.Section?.Name ?? "N/A"}");
        sb.AppendLine($"Enrollment Date: {student.EnrollmentDate:yyyy-MM-dd}");
        sb.AppendLine($"Status: {(student.IsActive ? "Active" : "Inactive")}");
        sb.AppendLine();
        
        sb.AppendLine("--- Financial Information ---");
        sb.AppendLine($"Total Fees: {student.TotalFees:C}");
        sb.AppendLine($"Total Paid: {student.TotalPaid:C}");
        sb.AppendLine($"Balance: {student.TotalBalance:C}");
        sb.AppendLine();
        
        if (student.Grades?.Any() == true)
        {
            sb.AppendLine("--- Grades ---");
            foreach (var grade in student.Grades)
            {
                sb.AppendLine($"Course: {grade.Course?.Name ?? "Unknown"}");
                sb.AppendLine($"  Assignment 1: {grade.Assignment1?.ToString() ?? "N/A"}");
                sb.AppendLine($"  Assignment 2: {grade.Assignment2?.ToString() ?? "N/A"}");
                sb.AppendLine($"  Course Work: {grade.CourseWork?.ToString() ?? "N/A"}");
                sb.AppendLine($"  Final Exam: {grade.FinalExam?.ToString() ?? "N/A"}");
                sb.AppendLine($"  Total Score: {grade.TotalScore?.ToString("F2") ?? "N/A"}");
                sb.AppendLine($"  Grade: {grade.SymbolicGrade ?? "Not Graded"}");
                sb.AppendLine();
            }
        }
        
        sb.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("========================================");
        
        File.WriteAllText(fileName, sb.ToString());
        return fileName;
    }
    
    /// <summary>
    /// Export course grades to text file
    /// </summary>
    public string ExportCourseGrades(Course course, IEnumerable<Grade> grades)
    {
        var fileName = Path.Combine(_exportPath, $"Course_{course.Code}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        var sb = new StringBuilder();
        
        sb.AppendLine("========================================");
        sb.AppendLine("        COURSE GRADES REPORT");
        sb.AppendLine("========================================");
        sb.AppendLine($"Course Code: {course.Code}");
        sb.AppendLine($"Course Name: {course.Name}");
        sb.AppendLine($"Course Type: {course.CourseType}");
        sb.AppendLine($"Max Degree: {course.MaxDegree}");
        sb.AppendLine($"Department: {course.Department?.Name ?? "N/A"}");
        sb.AppendLine();
        
        sb.AppendLine("--- Grade Distribution ---");
        var courseType = course.CourseType;
        if (courseType == CourseType.Theoretical100)
        {
            sb.AppendLine("Assignment 1: 20%");
            sb.AppendLine("Assignment 2: 20%");
            sb.AppendLine("Course Work: 60%");
            sb.AppendLine("Final Exam: N/A");
        }
        else
        {
            sb.AppendLine("Assignment 1: 20%");
            sb.AppendLine("Assignment 2: 30%");
            sb.AppendLine("Course Work: 20%");
            sb.AppendLine("Final Exam: 30%");
        }
        sb.AppendLine();
        
        sb.AppendLine("--- Student Grades ---");
        sb.AppendLine(new string('-', 120));
        sb.AppendLine($"{"Student ID",-15} {"Name",-25} {"Ass1",-8} {"Ass2",-8} {"CW",-8} {"Final",-8} {"Total",-10} {"Grade",-12}");
        sb.AppendLine(new string('-', 120));
        
        foreach (var grade in grades.OrderBy(g => g.Student?.StudentId))
        {
            sb.AppendLine($"{grade.Student?.StudentId ?? "N/A",-15} " +
                         $"{grade.Student?.Name ?? "Unknown",-25} " +
                         $"{grade.Assignment1?.ToString("F1") ?? "N/A",-8} " +
                         $"{grade.Assignment2?.ToString("F1") ?? "N/A",-8} " +
                         $"{grade.CourseWork?.ToString("F1") ?? "N/A",-8} " +
                         $"{grade.FinalExam?.ToString("F1") ?? "N/A",-8} " +
                         $"{grade.TotalScore?.ToString("F2") ?? "N/A",-10} " +
                         $"{grade.SymbolicGrade ?? "Not Graded",-12}");
        }
        
        sb.AppendLine(new string('-', 120));
        sb.AppendLine($"Total Students: {grades.Count()}");
        sb.AppendLine();
        sb.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("========================================");
        
        File.WriteAllText(fileName, sb.ToString());
        return fileName;
    }
    
    /// <summary>
    /// Export attendance records to text file
    /// </summary>
    public string ExportAttendance(IEnumerable<Attendance> attendances, string title = "Attendance Report")
    {
        var fileName = Path.Combine(_exportPath, $"Attendance_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        var sb = new StringBuilder();
        
        sb.AppendLine("========================================");
        sb.AppendLine($"     {title.ToUpper()}");
        sb.AppendLine("========================================");
        sb.AppendLine();
        
        var groupedByStudent = attendances.GroupBy(a => a.Student);
        
        foreach (var group in groupedByStudent)
        {
            var student = group.Key;
            var records = group.ToList();
            
            sb.AppendLine($"Student: {student?.Name ?? "Unknown"} ({student?.StudentId ?? "N/A"})");
            sb.AppendLine(new string('-', 80));
            
            foreach (var record in records.OrderBy(r => r.Date))
            {
                sb.AppendLine($"{record.Date:yyyy-MM-dd} | " +
                             $"{(record.IsLecture ? "Lecture" : "Lab"),-8} | " +
                             $"{record.Course?.Name ?? "Unknown",-30} | " +
                             $"{record.StatusDisplay,-8}");
            }
            
            int totalPresent = records.Count(r => r.IsPresent);
            int totalAbsent = records.Count(r => !r.IsPresent);
            double attendanceRate = records.Any() ? (totalPresent * 100.0 / records.Count) : 0;
            
            sb.AppendLine();
            sb.AppendLine($"Summary: Present: {totalPresent}, Absent: {totalAbsent}, Rate: {attendanceRate:F1}%");
            sb.AppendLine();
        }
        
        sb.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("========================================");
        
        File.WriteAllText(fileName, sb.ToString());
        return fileName;
    }
    
    /// <summary>
    /// Export department information to text file
    /// </summary>
    public string ExportDepartment(Department department)
    {
        var fileName = Path.Combine(_exportPath, $"Department_{department.Code}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        var sb = new StringBuilder();
        
        sb.AppendLine("========================================");
        sb.AppendLine("      DEPARTMENT INFORMATION");
        sb.AppendLine("========================================");
        sb.AppendLine($"Code: {department.Code}");
        sb.AppendLine($"Name: {department.Name}");
        sb.AppendLine($"Head of Department: {department.HeadOfDepartment ?? "N/A"}");
        sb.AppendLine($"Annual Fees: {department.AnnualFees:C}");
        sb.AppendLine();
        
        if (department.Courses?.Any() == true)
        {
            sb.AppendLine($"--- Courses ({department.Courses.Count}) ---");
            foreach (var course in department.Courses)
            {
                sb.AppendLine($"  {course.Code} - {course.Name} ({course.Credits} credits)");
            }
            sb.AppendLine();
        }
        
        if (department.Students?.Any() == true)
        {
            sb.AppendLine($"--- Students ({department.Students.Count}) ---");
            var activeStudents = department.Students.Count(s => s.IsActive);
            sb.AppendLine($"  Active: {activeStudents}");
            sb.AppendLine($"  Inactive: {department.Students.Count - activeStudents}");
            sb.AppendLine();
        }
        
        sb.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("========================================");
        
        File.WriteAllText(fileName, sb.ToString());
        return fileName;
    }
    
    /// <summary>
    /// Export lecture hall information to text file
    /// </summary>
    public string ExportLectureHall(LectureHall hall)
    {
        var fileName = Path.Combine(_exportPath, $"LectureHall_{hall.Code}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        var sb = new StringBuilder();
        
        sb.AppendLine("========================================");
        sb.AppendLine("     LECTURE HALL INFORMATION");
        sb.AppendLine("========================================");
        sb.AppendLine($"Code: {hall.Code}");
        sb.AppendLine($"Name: {hall.Name}");
        sb.AppendLine($"Description: {hall.Description ?? "N/A"}");
        sb.AppendLine($"Capacity: {hall.Capacity} students");
        sb.AppendLine();
        
        sb.AppendLine("--- Equipment Specifications ---");
        sb.AppendLine($"Seats: {hall.NumberOfSeats}");
        sb.AppendLine($"Air Conditioners: {hall.NumberOfAirConditioners}");
        sb.AppendLine($"Fans: {hall.NumberOfFans}");
        sb.AppendLine($"Lights: {hall.NumberOfLights}");
        sb.AppendLine();
        
        sb.AppendLine($"Status: {(hall.IsActive ? "Active" : "Inactive")}");
        sb.AppendLine($"Department: {hall.Department?.Name ?? "N/A"}");
        sb.AppendLine();
        
        sb.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("========================================");
        
        File.WriteAllText(fileName, sb.ToString());
        return fileName;
    }
    
    /// <summary>
    /// Export laboratory information to text file
    /// </summary>
    public string ExportLaboratory(Laboratory lab)
    {
        var fileName = Path.Combine(_exportPath, $"Laboratory_{lab.Code}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        var sb = new StringBuilder();
        
        sb.AppendLine("========================================");
        sb.AppendLine("      LABORATORY INFORMATION");
        sb.AppendLine("========================================");
        sb.AppendLine($"Code: {lab.Code}");
        sb.AppendLine($"Name: {lab.Name}");
        sb.AppendLine($"Description: {lab.Description ?? "N/A"}");
        sb.AppendLine($"Capacity: {lab.Capacity} students");
        sb.AppendLine();
        
        sb.AppendLine("--- Equipment Specifications ---");
        sb.AppendLine($"Computers: {lab.NumberOfComputers}");
        sb.AppendLine($"Seats: {lab.NumberOfSeats}");
        sb.AppendLine($"Air Conditioners: {lab.NumberOfAirConditioners}");
        sb.AppendLine($"Fans: {lab.NumberOfFans}");
        sb.AppendLine($"Lights: {lab.NumberOfLights}");
        sb.AppendLine();
        
        sb.AppendLine($"Status: {(lab.IsActive ? "Active" : "Inactive")}");
        sb.AppendLine($"Department: {lab.Department?.Name ?? "N/A"}");
        sb.AppendLine();
        
        sb.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("========================================");
        
        File.WriteAllText(fileName, sb.ToString());
        return fileName;
    }
    
    public string GetExportPath() => _exportPath;
}

