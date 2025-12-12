# NCT University Management System

<p align="center">
  <img src="logo.jpg" alt="NCT Logo" width="150"/>
</p>

<p align="center">
  <strong>New Cairo Technology University</strong><br>
  Comprehensive Academic Management Solution
</p>

---

## 📋 Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [Technology Stack](#technology-stack)
4. [System Architecture](#system-architecture)
5. [Entity Relationship Diagram (ERD)](#entity-relationship-diagram-erd)
6. [Database Schema](#database-schema)
7. [Modules Description](#modules-description)
8. [Grading System](#grading-system)
9. [Installation & Setup](#installation--setup)
10. [User Guide](#user-guide)
11. [Color Palette & Branding](#color-palette--branding)

---

## 📖 Overview

The **NCT University Management System** is a fully integrated desktop application developed in C# using WPF (Windows Presentation Foundation) with a modern, professional GUI. The system centralizes all university operations including student management, department administration, course scheduling, fee tracking, and academic grading.

### Key Highlights
- 🎓 Complete academic workflow management
- 💰 Automated fee calculation based on department
- 📊 Interactive grade distribution charts
- 🔒 Data validation and integrity checks
- 📈 Performance reports and analytics

---

## ✨ Features

### Core Modules

| Module | Description |
|--------|-------------|
| **Dashboard** | Real-time statistics, grade distribution pie chart, quick overview |
| **Students** | Full CRUD operations, department assignment, auto-generated IDs |
| **Departments** | Manage departments with unique annual fees, course/student counts |
| **Courses** | Course management with credits, year levels, department assignment |
| **Sections** | Section management with 40-student capacity, scheduling |
| **Fees** | Automatic fee calculation, payment tracking, status management |
| **Grades** | Score entry, automatic grade calculation, leniency algorithm |
| **Reports** | Performance analytics, course statistics, section utilization |

### Data Validation
- ✅ Email format validation
- ✅ Egyptian phone number format
- ✅ Student ID format (NCT + 5 digits)
- ✅ Score range validation (0-100)
- ✅ Duplicate record prevention
- ✅ Required field enforcement

---

## 🛠 Technology Stack

| Component | Technology |
|-----------|------------|
| **Framework** | .NET 8.0 |
| **UI Framework** | WPF (Windows Presentation Foundation) |
| **Architecture** | MVVM (Model-View-ViewModel) |
| **Database** | SQLite with Entity Framework Core 8.0 |
| **Charts** | LiveCharts2 (SkiaSharp) |
| **Toolkit** | CommunityToolkit.Mvvm |

---

## 🏗 System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                    Views (XAML)                      │    │
│  │  Dashboard │ Students │ Departments │ Courses │ ... │    │
│  └─────────────────────────────────────────────────────┘    │
│                           │                                  │
│                           ▼                                  │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                  ViewModels (C#)                     │    │
│  │  Data Binding │ Commands │ Business Logic           │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    BUSINESS LAYER                            │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                   Services                           │    │
│  │  GradingService │ ReportService │ ValidationService │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      DATA LAYER                              │
│  ┌─────────────────────────────────────────────────────┐    │
│  │              Entity Framework Core                   │    │
│  │         UniversityDbContext │ DbSeeder              │    │
│  └─────────────────────────────────────────────────────┘    │
│                           │                                  │
│                           ▼                                  │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                 SQLite Database                      │    │
│  │                  university.db                       │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

### Project Structure

```
UniversityManagementSystem/
├── Models/                    # Entity classes
│   ├── Student.cs
│   ├── Department.cs
│   ├── Course.cs
│   ├── Section.cs
│   ├── Grade.cs
│   └── StudentFee.cs
├── ViewModels/               # MVVM ViewModels
│   ├── BaseViewModel.cs
│   ├── MainViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── StudentsViewModel.cs
│   ├── DepartmentsViewModel.cs
│   ├── CoursesViewModel.cs
│   ├── SectionsViewModel.cs
│   ├── FeesViewModel.cs
│   ├── GradesViewModel.cs
│   └── ReportsViewModel.cs
├── Views/                    # XAML Views
│   ├── DashboardView.xaml
│   ├── StudentsView.xaml
│   ├── DepartmentsView.xaml
│   ├── CoursesView.xaml
│   ├── SectionsView.xaml
│   ├── FeesView.xaml
│   ├── GradesView.xaml
│   └── ReportsView.xaml
├── Data/                     # Database layer
│   ├── UniversityDbContext.cs
│   └── DbSeeder.cs
├── Services/                 # Business services
│   ├── GradingService.cs
│   ├── ReportService.cs
│   └── ValidationService.cs
├── Converters/               # WPF value converters
│   └── BoolConverters.cs
├── Resources/                # Styles and themes
│   ├── Colors.xaml
│   └── Styles.xaml
├── App.xaml                  # Application entry
├── MainWindow.xaml           # Main window with navigation
└── logo.jpg                  # NCT University logo
```

---

## 📊 Entity Relationship Diagram (ERD)

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           NCT UNIVERSITY ERD                                     │
└─────────────────────────────────────────────────────────────────────────────────┘

    ┌──────────────────┐          ┌──────────────────┐
    │    DEPARTMENT    │          │      COURSE      │
    ├──────────────────┤          ├──────────────────┤
    │ *Id (PK)         │──┐       │ *Id (PK)         │
    │  Code            │  │       │  Code            │
    │  Name            │  │   ┌───│  DepartmentId(FK)│
    │  HeadOfDepartment│  │   │   │  Name            │
    │  Description     │  │   │   │  Credits         │
    │  AnnualFees      │  │   │   │  YearLevel       │
    └──────────────────┘  │   │   │  Description     │
           │              │   │   └──────────────────┘
           │1             │   │            │1
           │              │   │            │
           │              │   │            │
           ▼N             │   │            ▼N
    ┌──────────────────┐  │   │   ┌──────────────────┐
    │     STUDENT      │  │   │   │     SECTION      │
    ├──────────────────┤  │   │   ├──────────────────┤
    │ *Id (PK)         │  │   │   │ *Id (PK)         │
    │  StudentId       │  │   │   │  CourseId (FK)───┘
    │  Name            │  │   │   │  Name            │
    │  Email           │◄─┘   │   │  Capacity (40)   │
    │  Phone           │      │   │  Schedule        │
    │  YearLevel       │      │   │  Room            │
    │  DepartmentId(FK)│──────┘   └──────────────────┘
    │  SectionId (FK)──│──────────────────┘
    │  EnrollmentDate  │              │
    │  IsActive        │              │
    └──────────────────┘              │
           │                          │
           │1                         │
           │                          │
           ├──────────────────────────┤
           │                          │
           ▼N                         ▼N
    ┌──────────────────┐      ┌──────────────────┐
    │   STUDENT_FEE    │      │      GRADE       │
    ├──────────────────┤      ├──────────────────┤
    │ *Id (PK)         │      │ *Id (PK)         │
    │  StudentId (FK)──│      │  StudentId (FK)──│
    │  AcademicYear    │      │  CourseId (FK)───│──────┐
    │  Amount          │      │  Assignment1     │      │
    │  AmountPaid      │      │  Assignment2     │      │
    │  Status          │      │  FinalExam       │      │
    │  AssignedDate    │      │  TotalScore      │      │
    │  LastPaymentDate │      │  SymbolicGrade   │      │
    └──────────────────┘      │  LeniencyApplied │      │
                              │  GradedDate      │      │
                              └──────────────────┘      │
                                                       │
                              ┌─────────────────────────┘
                              │
                              ▼
                       (Links to COURSE)
```

### Relationships Summary

| Relationship | Type | Description |
|--------------|------|-------------|
| Department → Student | 1:N | One department has many students |
| Department → Course | 1:N | One department offers many courses |
| Course → Section | 1:N | One course has multiple sections |
| Section → Student | 1:N | One section contains many students |
| Student → StudentFee | 1:N | One student has fees for each year |
| Student → Grade | 1:N | One student has grades for each course |
| Course → Grade | 1:N | One course has grades from many students |

---

## 🗄 Database Schema

### Department Table
```sql
CREATE TABLE Departments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Code TEXT NOT NULL,
    Name TEXT NOT NULL,
    HeadOfDepartment TEXT,
    Description TEXT,
    AnnualFees DECIMAL(18,2) DEFAULT 0
);
```

### Student Table
```sql
CREATE TABLE Students (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentId TEXT,              -- Format: NCT00001
    Name TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    Phone TEXT,
    YearLevel INTEGER CHECK(YearLevel BETWEEN 1 AND 4),
    DepartmentId INTEGER REFERENCES Departments(Id),
    SectionId INTEGER REFERENCES Sections(Id),
    EnrollmentDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN DEFAULT 1
);
```

### Course Table
```sql
CREATE TABLE Courses (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Code TEXT NOT NULL,          -- Format: CS101
    Name TEXT NOT NULL,
    Credits INTEGER CHECK(Credits BETWEEN 1 AND 6),
    YearLevel INTEGER CHECK(YearLevel BETWEEN 1 AND 4),
    DepartmentId INTEGER REFERENCES Departments(Id),
    Description TEXT
);
```

### Section Table
```sql
CREATE TABLE Sections (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    CourseId INTEGER NOT NULL REFERENCES Courses(Id),
    Capacity INTEGER DEFAULT 40,
    Schedule TEXT,
    Room TEXT
);
```

### StudentFee Table
```sql
CREATE TABLE StudentFees (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentId INTEGER NOT NULL REFERENCES Students(Id) ON DELETE CASCADE,
    AcademicYear INTEGER CHECK(AcademicYear BETWEEN 1 AND 4),
    Amount DECIMAL(18,2) NOT NULL,
    AmountPaid DECIMAL(18,2) DEFAULT 0,
    Status TEXT DEFAULT 'Pending',  -- Pending, Partial, Paid, Overdue
    AssignedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    LastPaymentDate DATETIME
);
```

### Grade Table
```sql
CREATE TABLE Grades (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentId INTEGER NOT NULL REFERENCES Students(Id) ON DELETE CASCADE,
    CourseId INTEGER NOT NULL REFERENCES Courses(Id),
    Assignment1 REAL,            -- 0-100
    Assignment2 REAL,            -- 0-100
    FinalExam REAL,              -- 0-100
    TotalScore REAL,             -- Calculated weighted score
    SymbolicGrade TEXT,          -- D, M, P, NA
    LeniencyApplied BOOLEAN DEFAULT 0,
    GradedDate DATETIME,
    UNIQUE(StudentId, CourseId)
);
```

---

## 📚 Modules Description

### 1. Dashboard Module
- **Purpose**: Provides an at-a-glance overview of the university system
- **Features**:
  - Total students, courses, sections, departments count
  - Active vs inactive student ratio
  - Interactive pie chart showing grade distribution
  - Quick navigation to other modules

### 2. Students Module
- **Purpose**: Complete student lifecycle management
- **Features**:
  - Add, edit, delete students
  - Auto-generated Student IDs (NCT format)
  - Department and section assignment
  - Automatic fee generation on enrollment
  - Search and filter by name, year, department
  - Email and phone validation

### 3. Departments Module
- **Purpose**: Manage academic departments and their fee structures
- **Features**:
  - Create departments with unique codes
  - Set annual tuition fees per department
  - View student and course counts
  - Automatic fee updates when fees change
  - Prevent deletion if students/courses assigned

### 4. Courses Module
- **Purpose**: Academic course management
- **Features**:
  - Course code validation (e.g., CS101)
  - Credit hours assignment (1-6)
  - Year level designation (1-4)
  - Department association

### 5. Sections Module
- **Purpose**: Class section and scheduling management
- **Features**:
  - 40-student capacity per section
  - Schedule and room assignment
  - Enrollment tracking
  - Capacity utilization statistics

### 6. Fees Module
- **Purpose**: Financial management and payment tracking
- **Features**:
  - Automatic fee calculation based on department
  - Payment recording (full or partial)
  - Status tracking (Pending, Partial, Paid, Overdue)
  - Summary statistics (total, collected, pending)
  - Generate fees for new academic year
  - Filter by department and status

### 7. Grades Module
- **Purpose**: Academic assessment and grade calculation
- **Features**:
  - Enter Assignment 1, Assignment 2, Final Exam scores
  - Automatic grade calculation with weighted formula
  - Leniency algorithm for borderline cases
  - Real-time grade preview
  - Batch recalculation capability

### 8. Reports Module
- **Purpose**: Analytics and performance tracking
- **Features**:
  - Course performance statistics
  - Student performance rankings
  - Section utilization rates
  - Leniency application statistics

---

## 📐 Grading System

### Grade Calculation Formula

```
Total Score = (Assignment1 × 20%) + (Assignment2 × 20%) + (FinalExam × 60%)
```

### Grade Thresholds

| Grade | Symbol | Score Range | Description |
|-------|--------|-------------|-------------|
| Distinction | D | 85 - 100 | Excellent performance |
| Merit | M | 70 - 84.99 | Very good performance |
| Pass | P | 50 - 69.99 | Satisfactory performance |
| Not Achieved | NA | 0 - 49.99 | Below passing standard |

### Leniency Algorithm

The system applies a **leniency algorithm** for students who:
1. Show improvement (Assignment2 > Assignment1)
2. Are within 2 points of the next grade boundary

**Example**: A student with 83.5 total score showing improvement would be upgraded from Merit (M) to Distinction (D).

```csharp
// Leniency conditions
bool showsImprovement = Assignment2 > Assignment1;
bool nearBoundary = score >= (threshold - 2) && score < threshold;

if (showsImprovement && nearBoundary)
{
    ApplyLeniency(); // Upgrade to next grade
}
```

---

## 🚀 Installation & Setup

### Prerequisites
- Windows 10/11
- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code with C# extension

### Installation Steps

1. **Clone or download the project**
   ```bash
   cd "C:\Users\YourUsername\Desktop\ZIAD task"
   ```

2. **Restore NuGet packages**
   ```bash
   cd UniversityManagementSystem
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

### First Run
- The database (`university.db`) is automatically created
- Sample data is seeded including:
  - 4 Departments (CS, IT, BUS, ENG)
  - 16 Courses (4 per department)
  - 32 Sections
  - 40 Students
  - Fee records
  - Sample grades

---

## 📖 User Guide

### Navigation
- Use the **sidebar menu** to navigate between modules
- Current page is highlighted with gold accent
- Click on any menu item to switch views

### Adding a Student
1. Go to **Students** module
2. Click **+ Add Student**
3. Fill in required fields (Name, Email, Department)
4. Student ID is auto-generated if left blank
5. Click **Save Changes**
6. Fees are automatically calculated based on department

### Recording Grades
1. Go to **Grades** module
2. Click **+ Add Grade**
3. Select Student and Course
4. Enter scores (0-100) for:
   - Assignment 1
   - Assignment 2
   - Final Exam
5. View calculated grade in real-time preview
6. Click **Save Grade**

### Managing Fees
1. Go to **Fees** module
2. Select a fee record
3. Click **Record Payment** to enter partial payment
4. Or click **Mark as Paid** for full payment
5. Status updates automatically

---

## 🎨 Color Palette & Branding

### NCT University Colors

| Color | Hex Code | Usage |
|-------|----------|-------|
| Navy Blue (Primary) | `#112250` | Headers, sidebar, primary buttons |
| Gold (Accent) | `#cb810d` | Highlights, hover states, badges |
| Gray (Secondary) | `#75757d` | Secondary text, borders |
| White | `#FFFFFF` | Backgrounds, cards |
| Background | `#F5F7FA` | Main background |

### Status Colors

| Status | Color | Hex |
|--------|-------|-----|
| Success/Paid | Green | `#10B981` |
| Warning/Pending | Orange | `#F59E0B` |
| Error/Overdue | Red | `#EF4444` |
| Info/Partial | Blue | `#3B82F6` |

### Grade Colors

| Grade | Color | Hex |
|-------|-------|-----|
| Distinction (D) | Green | `#10B981` |
| Merit (M) | Blue | `#3B82F6` |
| Pass (P) | Yellow | `#F59E0B` |
| Not Achieved (NA) | Red | `#EF4444` |

---

## 📄 License

This project was developed for **New Cairo Technology University** as an academic management solution.

---

## 👥 Credits

Developed with ❤️ for NCT University

**Version**: 1.0.0  
**Last Updated**: December 2024

---

<p align="center">
  <strong>New Cairo Technology University</strong><br>
  Excellence in Education
</p>
