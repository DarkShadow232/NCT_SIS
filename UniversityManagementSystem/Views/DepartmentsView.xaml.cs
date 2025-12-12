using System.Windows.Controls;
using UniversityManagementSystem.ViewModels;

namespace UniversityManagementSystem.Views;

public partial class DepartmentsView : UserControl
{
    public DepartmentsView()
    {
        InitializeComponent();
        DataContext = new DepartmentsViewModel();
    }
}

