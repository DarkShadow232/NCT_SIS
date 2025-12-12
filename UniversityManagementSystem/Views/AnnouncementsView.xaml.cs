using System.Windows.Controls;
using UniversityManagementSystem.ViewModels;

namespace UniversityManagementSystem.Views;

public partial class AnnouncementsView : UserControl
{
    public AnnouncementsView()
    {
        InitializeComponent();
        DataContext = new AnnouncementsViewModel();
    }
}

