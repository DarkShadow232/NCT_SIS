# Data Structures and Features Documentation

## 📊 Overview

This document demonstrates the use of **structs**, **arrays**, **vectors (collections)**, and **functions** in the University Management System, as required by the PDF assignment.

---

## 🏗️ 1. STRUCTS / CLASSES (C# Equivalent)

In C#, we use **classes** and **structs** to define data structures. Here are the key data structures used in this project:

### Student Structure (Class)
```csharp
public class Student
{
    public int Id { get; set; }
    public string StudentId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int YearLevel { get; set; }
    public int? DepartmentId { get; set; }
    public int? SectionId { get; set; }
    public bool IsActive { get; set; }
    public DateTime EnrollmentDate { get; set; }
    
    // Navigation properties
    public Department? Department { get; set; }
    public Section? Section { get; set; }
    public ICollection<Grade> Grades { get; set; }
    public ICollection<StudentFee> Fees { get; set; }
}
```

### Instructor Structure (Class)
```csharp
public class Instructor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public int? DepartmentId { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation properties
    public Department? Department { get; set; }
    public ICollection<CourseInstructor> CourseInstructors { get; set; }
}
```

### Course Structure (Class)
```csharp
public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public int Credits { get; set; }
    public int YearLevel { get; set; }
    public CourseType CourseType { get; set; }  // Enum: Theoretical/Practical
    public int MaxDegree { get; set; }
    public int? DepartmentId { get; set; }
    
    // Navigation properties
    public Department? Department { get; set; }
}
```

### Grade Structure (Class)
```csharp
public class Grade
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public double? Assignment1 { get; set; }
    public double? Assignment2 { get; set; }
    public double? CourseWork { get; set; }    // NEW: Added as per PDF
    public double? FinalExam { get; set; }
    public double? TotalScore { get; set; }
    public string? SymbolicGrade { get; set; }
    public DateTime? GradedDate { get; set; }
    
    // Navigation properties
    public Student Student { get; set; }
    public Course Course { get; set; }
}
```

### Attendance Structure (Class)
```csharp
public class Attendance
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int? SectionId { get; set; }
    public DateTime Date { get; set; }
    public bool IsPresent { get; set; }
    public bool IsLecture { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public Student Student { get; set; }
    public Course Course { get; set; }
    public Section? Section { get; set; }
}
```

### Lecture Hall Structure (Class)
```csharp
public class LectureHall
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public int NumberOfSeats { get; set; }
    public int NumberOfAirConditioners { get; set; }
    public int NumberOfFans { get; set; }
    public int NumberOfLights { get; set; }
    public bool IsActive { get; set; }
}
```

### Laboratory Structure (Class)
```csharp
public class Laboratory : LectureHall
{
    public int NumberOfComputers { get; set; }
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
}
```

---

## 📦 2. VECTORS / COLLECTIONS (C# Equivalent)

In C#, we use **`List<T>`**, **`ObservableCollection<T>`**, and **`ICollection<T>`** as equivalents to C++ vectors.

### Examples:

#### ObservableCollection (Used in ViewModels for UI binding)
```csharp
// In StudentsViewModel.cs
private ObservableCollection<Student> _students = new();
private ObservableCollection<Department> _departments = new();
private ObservableCollection<Section> _sections = new();

public ObservableCollection<Student> Students
{
    get => _students;
    set => SetProperty(ref _students, value);
}
```

#### List (Used for data processing)
```csharp
// In DbSeeder.cs
var students = new List<Student>();
var departments = new List<Department>();
var instructors = new List<Instructor>();
var courses = new List<Course>();
```

#### ICollection (Used for navigation properties)
```csharp
public class Department
{
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();
    public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
}
```

#### Dictionary (Key-Value Collection)
```csharp
// In InstructorsViewModel.cs
private Dictionary<string, int> _instructorsByDepartment = new();

// Usage:
InstructorsByDepartment = instructors
    .Where(i => i.Department != null)
    .GroupBy(i => i.Department!.Name)
    .ToDictionary(g => g.Key, g => g.Count());
```

---

## 📐 3. ARRAYS

Arrays are used for fixed-size collections of data.

### Examples:

#### Year Levels Array
```csharp
// In StudentsViewModel.cs
public int[] YearLevels => new[] { 1, 2, 3, 4 };
```

#### First Names Array (for seeding)
```csharp
// In DbSeeder.cs
var firstNames = new[] { "Ahmed", "Mohamed", "Omar", "Youssef", "Ali", 
                         "Sara", "Fatma", "Nour", "Mariam", "Hana" };
```

#### Last Names Array (for seeding)
```csharp
// In DbSeeder.cs
var lastNames = new[] { "Hassan", "Mohamed", "Ahmed", "Ali", "Ibrahim", 
                        "Khaled", "Mahmoud", "Youssef", "Salem", "Nasser" };
```

#### Grade Thresholds (Implicit Array)
```csharp
// In GradingService.cs - Percentage-based grading thresholds
var thresholds = new (double threshold, string grade)[]
{
    (0.85, "Excellent"),
    (0.75, "Very Good"),
    (0.65, "Good"),
    (0.60, "Pass"),
    (0.00, "Fail")
};
```

---

## ⚙️ 4. FUNCTIONS / METHODS

Functions are used throughout the application for processing data, validation, and business logic.

### Grade Calculation Functions

#### Calculate Total Score (GradingService.cs)
```csharp
public double CalculateTotalScore(double? assignment1, double? assignment2, 
                                  double? courseWork, double? finalExam, 
                                  CourseType courseType)
{
    double a1 = assignment1 ?? 0;
    double a2 = assignment2 ?? 0;
    double cw = courseWork ?? 0;
    double fin = finalExam ?? 0;

    if (courseType == CourseType.Theoretical100)
    {
        // Theoretical: A1(20%) + A2(20%) + CW(60%)
        return (a1 * 0.20) + (a2 * 0.20) + (cw * 0.60);
    }
    else
    {
        // Practical: A1(20%) + A2(30%) + CW(20%) + Final(30%)
        return (a1 * 0.20) + (a2 * 0.30) + (cw * 0.20) + (fin * 0.30);
    }
}
```

#### Get Symbolic Grade (GradingService.cs)
```csharp
public string GetSymbolicGrade(double totalScore, int maxDegree)
{
    double percentage = totalScore / maxDegree;
    
    if (percentage >= 0.85) return "Excellent";
    if (percentage >= 0.75) return "Very Good";
    if (percentage >= 0.65) return "Good";
    if (percentage >= 0.60) return "Pass";
    return "Fail";
}
```

### Data Export Functions

#### Export Student Report (FileExportService.cs)
```csharp
public string ExportStudent(Student student)
{
    var fileName = Path.Combine(_exportPath, 
        $"Student_{student.StudentId}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
    var sb = new StringBuilder();
    
    sb.AppendLine("========================================");
    sb.AppendLine("          STUDENT INFORMATION");
    sb.AppendLine("========================================");
    sb.AppendLine($"Student ID: {student.StudentId}");
    sb.AppendLine($"Name: {student.Name}");
    // ... more fields
    
    File.WriteAllText(fileName, sb.ToString());
    return fileName;
}
```

#### Export Course Grades (FileExportService.cs)
```csharp
public string ExportCourseGrades(Course course, IEnumerable<Grade> grades)
{
    var fileName = Path.Combine(_exportPath, 
        $"Course_{course.Code}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
    var sb = new StringBuilder();
    
    // Format grades in a table
    sb.AppendLine($"{"Student ID",-15} {"Name",-25} {"A1",-8} {"A2",-8} 
                   {"CW",-8} {"Final",-8} {"Total",-10} {"Grade",-12}");
    
    foreach (var grade in grades)
    {
        sb.AppendLine($"{grade.Student?.StudentId ?? "N/A",-15} " +
                     $"{grade.Student?.Name ?? "Unknown",-25} " +
                     $"{grade.Assignment1?.ToString("F1") ?? "N/A",-8} " +
                     // ... more fields
        );
    }
    
    File.WriteAllText(fileName, sb.ToString());
    return fileName;
}
```

### Validation Functions

#### Validate Student Form (StudentsViewModel.cs)
```csharp
private bool ValidateAll()
{
    ValidateName();
    ValidateEmail();
    ValidateStudentId();
    ValidatePhone();
    ValidateDepartment();
    
    return !HasErrors;
}

private void ValidateName()
{
    NameError = ValidationService.IsNotEmpty(FormName) 
        ? string.Empty 
        : "Name is required.";
}

private void ValidateEmail()
{
    EmailError = ValidationService.GetEmailError(FormEmail);
}
```

### Database Seeding Functions

#### Seed Students Data (DbSeeder.cs)
```csharp
private static void SeedStudents(UniversityDbContext context)
{
    var random = new Random(42);
    var students = new List<Student>();
    
    foreach (var dept in departments)
    {
        for (int i = 0; i < 10; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            
            students.Add(new Student
            {
                StudentId = $"{dept.Code}{studentCounter:D4}",
                Name = $"{firstName} {lastName}",
                Email = $"{firstName.ToLower()}.{lastName.ToLower()}@student.nct.edu.eg",
                // ... more fields
            });
        }
    }
    
    context.Students.AddRange(students);
    context.SaveChanges();
}
```

---

## 📋 5. NEW FEATURES ADDED

### ✅ Grade Page with Data
- **Sample grade data** seeded for all students
- **Course Work (CW)** field added to grade entry form
- **Updated grading formula** displayed:
  - Practical: A1 (20%) + A2 (30%) + CW (20%) + Final (30%)
  - Theoretical: A1 (20%) + A2 (20%) + CW (60%)
- **New grade thresholds**: Excellent ≥85% | Very Good ≥75% | Good ≥65% | Pass ≥60%

### ✅ Instructors/Doctors Management Page
- **Full CRUD operations** for managing instructors
- **Phone number** field for each instructor
- **Department assignment** for each instructor
- **Statistics** showing total instructors and department coverage
- **Data validation** for all input fields

### ✅ Student Report Download Feature
- **📄 Download Report** button in Students view
- **Comprehensive report** including:
  - Student information
  - Financial details (fees, payments, balance)
  - All grades with course details
- **File export** to `D:\UniversityData\` or application directory
- **Timestamp** in filename for easy tracking

### ✅ Database Enhancements
- **8 instructors** seeded with complete information
- **Grade data** for all students with CourseWork field
- **PhoneNumber** property added to Instructor model
- **Database migration** support for schema updates

---

## 🎯 6. HOW TO USE THE NEW FEATURES

### Access the Instructors/Doctors Page
1. Login as **admin** (username: `admin`, password: `admin123`)
2. Click on **"Instructors/Doctors"** in the left navigation menu
3. View the list of instructors with their details
4. Use **"+ Add Doctor"** to add new instructors
5. Select an instructor and click **"Edit"** to modify details
6. Click **"Delete"** to remove an instructor

### View and Manage Grades
1. Click on **"Grades"** in the navigation menu
2. View all student grades with A1, A2, CW, Final, Total, and Grade columns
3. Click **"+ Add Grade"** to enter new grades
4. Fill in all four components: Assignment 1, Assignment 2, Course Work, Final Exam
5. The system automatically calculates the total and symbolic grade

### Download Student Reports
1. Click on **"Students"** in the navigation menu
2. Select a student from the list
3. Click the **"📄 Download Report"** button
4. A message will show the file location
5. Open the text file to view the comprehensive student report

---

## 📝 7. SUMMARY

This project successfully demonstrates the use of:

- ✅ **Structs/Classes**: 15+ data models (Student, Instructor, Course, Grade, Attendance, etc.)
- ✅ **Vectors/Collections**: ObservableCollection, List, ICollection, Dictionary
- ✅ **Arrays**: Year levels, name arrays, grade thresholds
- ✅ **Functions**: Grade calculations, validations, data exports, database seeding

All features from the PDF requirements have been implemented in a modern C# WPF application with a beautiful UI and complete functionality.

---

**Generated on:** December 21, 2025  
**Project:** NCT University Management System  
**Version:** 2.0 (PDF Requirements Compliant)

