using System.Windows.Controls;
using UniversityManagementSystem.ViewModels;

namespace UniversityManagementSystem.Views;

public partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();
        DataContext = new UsersViewModel();
    }
}

